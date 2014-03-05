using System;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// Represents an extension class that adds field interception support to a given type.
    /// </summary>
    public static class FieldInterceptionExtensions
    {
        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>        
        public static void InterceptAllFields(this IReflectionStructureVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            targetType.InterceptFields(methodFilter, f => true);
        }

        /// <summary>
        /// Adds field interception support intercepting all instance fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>        
        public static void InterceptAllInstanceFields(this IReflectionStructureVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => !f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        /// Adds field interception support intercepting all static fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>     
        public static void InterceptAllStaticFields(this IReflectionStructureVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>        
        public static void InterceptAllFields(this IReflectionVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            targetType.InterceptFields(methodFilter, f => true);
        }

        /// <summary>
        /// Adds field interception support intercepting all instance fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>        
        public static void InterceptAllInstanceFields(this IReflectionVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(f => !f.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        /// Adds field interception support intercepting all static fields on the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>     
        public static void InterceptAllStaticFields(this IReflectionVisitable targetType)
        {
            var methodFilter = GetMethodFilter();
            var fieldFilter = GetFieldFilter(actualField => actualField.IsStatic);

            targetType.InterceptFields(methodFilter, fieldFilter);
        }

        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        /// <param name="methodFilter">The filter that determines which methods on the target type will be modified to support field interception.</param>
        /// <param name="fieldFilter">The filter that determines which fields should be intercepted.</param>
        public static void InterceptFields(this IReflectionVisitable targetType,
            Func<MethodReference, bool> methodFilter,
            Func<FieldReference, bool> fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(t => true);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            targetType.WeaveWith(fieldWeaver, methodFilter);
            targetType.Accept(typeWeaver);
        }

        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        /// <param name="hostTypeFilter">The filter that determines the host types to be modified.</param>
        /// <param name="fieldFilter">The field filter that determines the fields that will be intercepted.</param>
        public static void InterceptFields(this IReflectionStructureVisitable targetType, ITypeFilter hostTypeFilter,
            IFieldFilter fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(hostTypeFilter.ShouldWeave);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            targetType.WeaveWith(fieldWeaver, m => true);
            targetType.Accept(typeWeaver);
        }

        /// <summary>
        /// Adds field interception support to the target type.
        /// </summary>
        /// <param name="targetType">The type that will be modified.</param>
        /// <param name="methodFilter">The filter that determines which methods on the target type will be modified to support field interception.</param>
        /// <param name="fieldFilter">The filter that determines which fields should be intercepted.</param>
        public static void InterceptFields(this IReflectionStructureVisitable targetType,
            Func<MethodReference, bool> methodFilter,
            Func<FieldReference, bool> fieldFilter)
        {
            var typeWeaver = new ImplementFieldInterceptionHostWeaver(t => true);
            var fieldWeaver = new InterceptFieldAccess(fieldFilter);

            targetType.WeaveWith(fieldWeaver, methodFilter);
            targetType.Accept(typeWeaver);
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