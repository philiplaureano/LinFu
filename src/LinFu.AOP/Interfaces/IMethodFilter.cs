using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that determines which host methods should be modified for method call interception.
    /// </summary>
    public interface IMethodFilter 
    {
        /// <summary>
        /// Determines whether or not a particular method should be modified.
        /// </summary>
        /// <param name="targetMethod">The target method to be modified.</param>
        /// <returns>Returns <c>true</c> if the method should be modified; otherwise, it will return <c>false</c>.</returns>
        bool ShouldWeave(MethodReference targetMethod);
    }
}
