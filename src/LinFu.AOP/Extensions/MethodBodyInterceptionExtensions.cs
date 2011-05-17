using System;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// Represents an extension class that adds method body interception support to the Mono.Cecil object model.
    /// </summary>
    public static class MethodBodyInterceptionExtensions
    {
        /// <summary>
        /// Intercepts all method bodies on the target item.
        /// </summary>
        /// <param name="target">The target to be modified.</param>
        public static void InterceptAllMethodBodies(this object target)
        {
            target.InterceptMethodBody(m => true);
        }

        /// <summary>
        /// Intercepts all method bodies on the target item.
        /// </summary>
        /// <param name="target">The target to be modified.</param>
        /// <param name="methodFilter">The method filter that will determine the methods that will be modified.</param>
        public static void InterceptMethodBody(this object target, IMethodFilter methodFilter)
        {
            target.InterceptMethodBody(methodFilter.ShouldWeave);
        }

        /// <summary>
        /// Intercepts all method bodies on the target item.
        /// </summary>
        /// <param name="target">The target to be modified.</param>
        /// <param name="methodFilter">The method filter that will determine the methods that will be modified.</param>
        public static void InterceptMethodBody(this object target,
                                               Func<MethodReference, bool> methodFilter)
        {
            Func<TypeReference, bool> typeFilter = GetTypeFilter();
            target.Accept(new ImplementModifiableType(typeFilter));

            var interceptMethodBody = new InterceptMethodBody(methodFilter);
            target.WeaveWith(interceptMethodBody, methodFilter);
        }

        private static Func<TypeReference, bool> GetTypeFilter()
        {
            return type =>
                       {
                           TypeDefinition actualType = type.Resolve();
                           if (actualType.IsValueType || actualType.IsInterface)
                               return false;

                           return actualType.IsClass;
                       };
        }
    }
}