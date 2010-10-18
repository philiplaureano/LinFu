using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Resolvers
{
    /// <summary>
    /// A <see cref="MethodFinder{TMethod}"/> type that uses a <see cref="IServiceContainer"/>
    /// instance to find a method with the most resolvable parameters.
    /// </summary>
    /// <typeparam name="TMethod">The method type that will be searched.</typeparam>
    public class MethodFinderFromContainer<TMethod> : MethodFinder<TMethod>, IMethodFinderWithContainer<TMethod>,
                                                      IInitialize
        where TMethod : MethodBase
    {
        #region IInitialize Members

        /// <summary>
        /// Initializes the target with the host container.
        /// </summary>
        /// <param name="container">The host <see cref="IServiceContainer"/> instance.</param>
        public void Initialize(IServiceContainer container)
        {
            Container = container;
        }

        #endregion

        #region IMethodFinderWithContainer<TMethod> Members

        /// <summary>
        /// Gets the value indicating the service container that will be used in the
        /// method search.
        /// </summary>
        public IServiceContainer Container { get; private set; }

        #endregion

        /// <summary>
        /// Examines a <see cref="ConstructorInfo"/> instance
        /// and determines if it can be instantiated with the services embedded in
        /// the target <paramref name="container"/>.
        /// </summary>
        /// <param name="fuzzyItem">The <see cref="FuzzyItem{T}"/> that represents the constructor to be examined.</param>
        /// <param name="container">The container that contains the services that will be used to instantiate the target type.</param>
        /// <param name="maxIndex">Indicates the index that 
        /// marks the point where the user-supplied arguments begin.</param>
        private static void CheckParameters(IFuzzyItem<TMethod> fuzzyItem,
                                            IServiceContainer container, int maxIndex)
        {
            TMethod constructor = fuzzyItem.Item;
            int currentIndex = 0;
            foreach (ParameterInfo param in constructor.GetParameters())
            {
                if (currentIndex == maxIndex)
                    break;

                Type parameterType = param.ParameterType;
                var criteria = new Criteria<TMethod> {Type = CriteriaType.Critical, Weight = 1};

                // The type must either be an existing service
                // or a list of services that can be created from the container
                Func<IServiceContainer, bool> predicate = parameterType.MustExistInContainer()
                    .Or(parameterType.ExistsAsServiceArray())
                    .Or(parameterType.ExistsAsEnumerableSetOfServices());

                criteria.Predicate = currentConstructor => predicate(container);
                fuzzyItem.Test(criteria);

                currentIndex++;
            }
        }

        /// <summary>
        /// Adds additional <see cref="ICriteria{T}"/> to the fuzzy search list.
        /// </summary>
        /// <param name="methods">The list of methods to rank.</param>
        /// <param name="finderContext">The <see cref="IMethodFinderContext"/> that describes the target method.</param>        
        protected override void Rank(IList<IFuzzyItem<TMethod>> methods, IMethodFinderContext finderContext)
        {
            IEnumerable<object> additionalArguments = finderContext.Arguments ?? new object[0];
            List<Type> argumentTypes = (from argument in additionalArguments
                                        let argumentType = argument == null ? typeof (object) : argument.GetType()
                                        select argumentType).ToList();

            int argumentCount = argumentTypes.Count;
            foreach (var fuzzyItem in methods)
            {
                if (fuzzyItem.Confidence < 0)
                    continue;

                // Check the constructor for any 
                // parameter types that might not exist
                // in the container and eliminate the
                // constructor as a candidate match if
                // that parameter type cannot be found
                TMethod constructor = fuzzyItem.Item;
                ParameterInfo[] parameters = constructor.GetParameters();
                int parameterCount = parameters.Length;
                int maxRelativeIndex = parameterCount - argumentCount;

                CheckParameters(fuzzyItem, Container, maxRelativeIndex);
            }
        }
    }
}