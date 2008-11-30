using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IMemberResolver{ConstructorInfo}"/> class.
    /// </summary>
    public class ConstructorResolver : MemberResolver<ConstructorInfo>
    {
        /// <summary>
        /// Initializes the class with the default values.
        /// </summary>
        public ConstructorResolver()
        {            
        }

        /// <summary>
        /// Initializes the class using the custom method finder.
        /// </summary>
        /// <param name="getFinder">The functor that will be used to instantiate the method finder.</param>
        public ConstructorResolver(Func<IServiceContainer, 
            IMethodFinder<ConstructorInfo>> getFinder) : base(getFinder)
        {
        }

        /// <summary>
        /// Returns the constructors that belong to the <paramref name="concreteType"/>.
        /// </summary>
        /// <param name="concreteType">The type that contains the list of constructors.</param>
        /// <returns>A list of constructors that belong to the <paramref name="concreteType"/>.</returns>
        protected override IEnumerable<ConstructorInfo> GetMembers(Type concreteType)
        {
            return concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Returns the parameterless constructor in case the search fails.
        /// </summary>
        /// <param name="concreteType">The target type that contains the default constructor.</param>
        /// <returns>The default constructor.</returns>
        protected override ConstructorInfo GetDefaultResult(Type concreteType)
        {
            return concreteType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                                               new Type[0], null);
        }
    }
}