using System;
using System.Linq;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    ///     Represents an extension class that adds field interception support to a given type.
    /// </summary>
    public static class FieldInterceptionExtensions
    {
        /// <summary>
        ///     Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllFields(this AssemblyDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            targetType.InterceptFields(methodFilter, f => true);
        }

        /// <summary>
        ///     Adds field interception support intercepting all instance fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllInstanceFields(this AssemblyDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => !f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        ///     Adds field interception support intercepting all static fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllStaticFields(this AssemblyDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        ///     Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllFields(this TypeDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            targetType.InterceptFields(methodFilter, f => true);
        }

        /// <summary>
        ///     Adds field interception support intercepting all instance fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllInstanceFields(this TypeDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => !f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        ///     Adds field interception support intercepting all static fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        public static void InterceptAllStaticFields(this TypeDefinition targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(actualField => actualField.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        ///     Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        /// <param name="methodFilter">
        ///     The filter that determines which methods on the target type will be modified to support
        ///     field interception.
        /// </param>
        /// <param name="fieldFilter">The filter that determines which fields should be intercepted.</param>
        public static void InterceptFields(this TypeDefinition targetType,
            Func<MethodReference, bool> methodFilter,
            Func<FieldReference, bool> fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(t => true);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            typeWeaver.Weave(targetType);
            var targetMethods = targetType.Methods.Where(m => methodFilter(m)).ToArray();
            foreach (var method in targetMethods)
            {
                fieldWeaver.Rewrite(method, method.GetILGenerator(), method.Body.Instructions.ToArray());
            }
        }

        /// <summary>
        ///     Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetAssembly">The type that will be modified.</param>
        /// <param name="hostTypeFilter">The filter that determines the host types to be modified.</param>
        /// <param name="fieldFilter">The field filter that determines the fields that will be intercepted.</param>
        public static void InterceptFields(this AssemblyDefinition targetAssembly, ITypeFilter hostTypeFilter,
            IFieldFilter fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(hostTypeFilter.ShouldWeave);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            var module = targetAssembly.MainModule;
            var targetTypes = module.Types.Where(hostTypeFilter.ShouldWeave).ToArray();
            foreach (var type in targetTypes)
            {
                typeWeaver.Weave(type);
                foreach (var method in type.Methods.Where(m => m.HasBody))
                {
                    fieldWeaver.Rewrite(method, method.GetILGenerator(), method.Body.Instructions.ToArray());
                }
            }
        }

        /// <summary>
        ///     Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetAssembly">The type that will be modified.</param>
        /// <param name="methodFilter">
        ///     The filter that determines which methods on the target type will be modified to support
        ///     field interception.
        /// </param>
        /// <param name="fieldFilter">The filter that determines which fields should be intercepted.</param>
        public static void InterceptFields(this AssemblyDefinition targetAssembly,
            Func<MethodReference, bool> methodFilter,
            Func<FieldReference, bool> fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(t => t.IsByReference && !t.IsValueType);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            var module = targetAssembly.MainModule;
            var targetTypes = module.Types.Where(t => t.Methods.Any(m => methodFilter(m))).ToArray();
            foreach (var type in targetTypes)
            {
                typeWeaver.Weave(type);
                foreach (var method in type.Methods.Where(m => m.HasBody))
                {
                    fieldWeaver.Rewrite(method, method.GetILGenerator(), method.Body.Instructions.ToArray());
                }
            }
        }

        private static Func<MethodReference, bool> GetMethodFilter()
        {
            return m => true;
        }

        private static Func<FieldReference, bool> GetFieldFilter(Func<FieldDefinition, bool> fieldFilter)
        {
            return field =>
            {
                var actualField = field.Resolve();
                var fieldType = actualField.FieldType;
                var module = fieldType.Module;

                var moduleName = module != null ? module.Name : string.Empty;
                if (moduleName.StartsWith("LinFu.AOP"))
                    return false;

                return fieldFilter(actualField);
            };
        }
    }
}