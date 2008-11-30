using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can choose a member that best matches
    /// the services currently available in a given <see cref="IServiceContainer"/> instance.
    /// </summary>
    /// <typeparam name="TMember">The member type that will be searched.</typeparam>
    public abstract class MemberResolver<TMember> : IMemberResolver<TMember>
        where TMember : MethodBase
    {
        private readonly Func<IServiceContainer, IMethodFinder<TMember>> _getFinder;

        /// <summary>
        /// The default constructor for the <see cref="MemberResolver{TMember}"/> class.
        /// </summary>
        protected MemberResolver()
        {
            _getFinder = container=>container.GetService<IMethodFinder<TMember>>();
        }

        /// <summary>
        /// Initializes the class with a <paramref name="getFinder">functor</paramref>
        /// that will be used to instantiate the method finder that will be used in the search.
        /// </summary>
        /// <param name="getFinder">The functor that will be used to instantiate the method finder.</param>
        protected MemberResolver(Func<IServiceContainer, IMethodFinder<TMember>> getFinder)
        {
            _getFinder = getFinder;
        }

        /// <summary>
        /// Uses the <paramref name="container"/> to determine which member to use from
        /// the <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the member values that will be used to invoke the members.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to evaluate the best match to use to invoke the target member.</param>
        /// <returns>A member instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        public TMember ResolveFrom(Type concreteType, IServiceContainer container,
            params object[] additionalArguments)
        {
            var constructors = GetMembers(concreteType);
            if (constructors == null)
                return null;

            var resolver = GetMethodFinder(container);
            TMember bestMatch = resolver.GetBestMatch(constructors, additionalArguments);

            // If all else fails, find the
            // default constructor and use it as the
            // best match by default
            if (bestMatch == null)
            {
                var defaultResult = GetDefaultResult(concreteType);

                bestMatch = defaultResult;
            }

            Debug.Assert(bestMatch != null);
            return bestMatch;
        }

        /// <summary>
        /// Determines the <see cref="IMethodFinder{T}"/> that will be used
        /// in the method search.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        protected virtual IMethodFinder<TMember> GetMethodFinder(IServiceContainer container)
        {
            return _getFinder(container);
        }

        /// <summary>
        /// The method used to retrieve the default result if no
        /// other alternative is found.
        /// </summary>
        /// <param name="concreteType">The target type that contains the default member.</param>
        /// <returns>The default member result.</returns>
        protected abstract TMember GetDefaultResult(Type concreteType);

        /// <summary>
        /// Lists the members associated with the <paramref name="concreteType"/>.
        /// </summary>
        /// <param name="concreteType">The target type that contains the type members.</param> 
        /// <returns>A list of members that belong to the concrete type.</returns>
        protected abstract IEnumerable<TMember> GetMembers(Type concreteType);
    }
}
