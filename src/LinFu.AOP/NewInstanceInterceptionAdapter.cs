using System;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an adapter class that maps <see cref="INewInstanceFilter"/> instances to 
    /// functors.
    /// </summary>
    public class NewInstanceInterceptionAdapter : INewInstanceFilter
    {
        private readonly Func<MethodReference, TypeReference, MethodReference, bool> _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewInstanceInterceptionAdapter"/> class.
        /// </summary>
        /// <param name="filter">The filter that determines which instances will be intercepted.</param>
        public NewInstanceInterceptionAdapter(Func<MethodReference, TypeReference, MethodReference, bool> filter)
        {
            _filter = filter;
        }


        /// <summary>
        /// Determines whether or not a particular constructor call should be intercepted by the postweaver.
        /// </summary>
        /// <param name="currentConstructor">The constructor used to instantiate the current instance.</param>
        /// <param name="concreteType">The concrete type that contains the new instance call.</param>
        /// <param name="hostMethod">The host method that contains the new operator call.</param>
        /// <returns>Returns <c>true</c> if the new operator call should be intercepted; otherwise, it should return <c>false</c>.</returns>
        public bool ShouldWeave(MethodReference currentConstructor, TypeReference concreteType,
            MethodReference hostMethod)
        {
            return _filter(currentConstructor, concreteType, hostMethod);
        }
    }
}