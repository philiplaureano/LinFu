using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that can dynamically intercept method calls.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Intercepts a method call using the given
        /// <see cref="IInvocationInfo"/> instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that will 
        /// contain all the necessary information associated with a 
        /// particular method call.</param>
        /// <returns>The return value of the target method. If the return type of the target
        /// method is <see cref="Void"/>, then the return value will be ignored.</returns>
        object Intercept(IInvocationInfo info);
    }
}