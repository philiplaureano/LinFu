using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Adds helper methods to the <see cref="TypeDefinition"/> class.
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        /// Applies a <see cref="IMethodWeaver"/> instance to all methods
        /// within the given <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The target module.</param>
        /// <param name="weaver">The <see cref="ITypeWeaver"/> instance that will modify the methods in the given target type.</param>
        public static void WeaveWith(this TypeDefinition targetType, IMethodWeaver weaver)
        {
            var module = targetType.Module;
            var targetMethods = from MethodDefinition method in targetType.Methods
                              where weaver.ShouldWeave(method)
                              select method;

            // Modify the host module
            weaver.ImportReferences(module);

            // Add any additional members to the target type
            weaver.AddAdditionalMembers(targetType);

            foreach (var item in targetMethods)
            {
                weaver.Weave(item);
            }
        }

        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        /// <param name="methodFilter">The filter that determines which methods on the target type will be modified to support field interception.</param>
        public static void InterceptFields(this TypeDefinition targetType, Func<MethodReference, bool> methodFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(t => true);
            var fieldWeaver = new InterceptFieldAccess();

            targetType.WeaveWith(fieldWeaver, methodFilter);
            targetType.Accept(typeWeaver);
        }
    }
}
