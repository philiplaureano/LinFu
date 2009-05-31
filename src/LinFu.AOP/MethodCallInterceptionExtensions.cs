using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an extension class that adds method call interception support to the Mono.Cecil object model.
    /// </summary>
    public static class MethodCallInterceptionExtensions
    {
        /// <summary>
        /// Modifies the current <paramref name="target"/> to support third-party method call interception.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="typeFilter">The filter that will determine the target types that will be modified.</param>
        /// <param name="hostMethodFilter">The filter that will determine the methods that will be modified on the target type.</param>
        /// <param name="methodCallFilter">The filter that will determine which third-party methods will be intercepted on the target type.</param>
        public static void InterceptMethodCalls(this IReflectionStructureVisitable target, Func<TypeReference, bool> typeFilter, Func<MethodReference, bool> hostMethodFilter, Func<MethodReference, bool> methodCallFilter)
        {
            var rewriter = new InterceptMethodCalls(hostMethodFilter, methodCallFilter);
            target.Accept(new ImplementMethodReplacementHost(typeFilter));
            target.WeaveWith(rewriter, hostMethodFilter);            
        }

        /// <summary>
        /// Modifies the current <paramref name="target"/> to support third-party method call interception.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="typeFilter">The filter that will determine the target types that will be modified.</param>
        /// <param name="hostMethodFilter">The filter that will determine the methods that will be modified on the target type.</param>
        /// <param name="methodCallFilter">The filter that will determine which third-party methods will be intercepted on the target type.</param>
        public static void InterceptMethodCalls(this IReflectionVisitable target, Func<TypeReference, bool> typeFilter, Func<MethodReference, bool> hostMethodFilter, Func<MethodReference, bool> methodCallFilter)
        {
            var rewriter = new InterceptMethodCalls(hostMethodFilter, methodCallFilter);
            target.Accept(new ImplementMethodReplacementHost(typeFilter));
            target.WeaveWith(rewriter, hostMethodFilter);
        }
    }
}