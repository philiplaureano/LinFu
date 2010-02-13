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
        /// Modifies the current <paramref name="target"/> to support third-party method call interception for all method calls made inside the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        public static void InterceptAllMethodCalls(this IReflectionStructureVisitable target)
        {
            Func<TypeReference, bool> typeFilter = type =>
            {
                var actualType = type.Resolve();
                return !actualType.IsValueType && !actualType.IsInterface;
            };

            var hostMethodFilter = GetHostMethodFilter();
            Func<MethodReference, bool> methodCallFilter = m => true;

            InterceptMethodCalls(target, typeFilter, hostMethodFilter, methodCallFilter);
        }

        private static Func<MethodReference, bool> GetHostMethodFilter()
        {
            return method =>
                       {
                           var actualMethod = method.Resolve();
                           var methodName = actualMethod.Name;
                           return actualMethod.HasBody && methodName != ".ctor" && methodName != ".cctor";
                       };
        }

        /// <summary>
        /// Modifies the current <paramref name="target"/> to support third-party method call interception for all method calls made inside the target.
        /// </summary>
        /// <param name="target">The target object.</param>
        public static void InterceptAllMethodCalls(this IReflectionVisitable target)
        {
            Func<TypeReference, bool> typeFilter = type =>
            {
                var actualType = type.Resolve();
                return !actualType.IsValueType && !actualType.IsInterface;
            };

            var hostMethodFilter = GetHostMethodFilter();
            Func<MethodReference, bool> methodCallFilter = m => true;

            InterceptMethodCalls(target, typeFilter, hostMethodFilter, methodCallFilter);
        }

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
            target.Accept(new ImplementModifiableType(typeFilter));
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
            target.Accept(new ImplementModifiableType(typeFilter));
            target.WeaveWith(rewriter, hostMethodFilter);
        }
    }
}