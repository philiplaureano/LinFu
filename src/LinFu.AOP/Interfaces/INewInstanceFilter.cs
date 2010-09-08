using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that determines the constructor calls that will be intercepted by the postweaver.
    /// </summary>
    public interface INewInstanceFilter
    {
        /// <summary>
        /// Determines whether or not a particular constructor call should be intercepted by the postweaver.
        /// </summary>
        /// <param name="currentConstructor">The constructor used to instantiate the current instance.</param>
        /// <param name="concreteType">The concrete type that contains the new instance call.</param>
        /// <param name="hostMethod">The host method that contains the new operator call.</param>
        /// <returns>Returns <c>true</c> if the new operator call should be intercepted; otherwise, it should return <c>false</c>.</returns>
        bool ShouldWeave(MethodReference currentConstructor, TypeReference concreteType, MethodReference hostMethod);
    }
}
