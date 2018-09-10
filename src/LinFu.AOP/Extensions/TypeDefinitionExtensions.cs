using System;
using System.Linq;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    ///     Adds helper methods to the <see cref="TypeDefinition" /> class.
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        ///     Applies a <see cref="IMethodWeaver" /> instance to all methods
        ///     within the given <paramref name="targetType" />.
        /// </summary>
        /// <param name="targetType">The target module.</param>
        /// <param name="weaver">The <see cref="ITypeWeaver" /> instance that will modify the methods in the given target type.</param>
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
                weaver.Weave(item);
        }

        /// <summary>
        /// Modifies all the methods in an assembly using the given method weaver.
        /// </summary>
        /// <param name="targetAssembly">The target assembly to be modified.</param>
        /// <param name="weaver">The weaver that will perform the modification.</param>
        public static void WeaveWith(this AssemblyDefinition targetAssembly, IMethodWeaver weaver)
        {
            Func<MethodReference, bool> defaultFilter = m => m.IsDefinition;
            WeaveWith(targetAssembly, weaver, defaultFilter);
        }

        /// <summary>
        /// Modifies all the types in an assembly using the given type weaver.
        /// </summary>
        /// <param name="targetAssembly">The target assembly to be modified.</param>
        /// <param name="weaver">The weaver that will perform the modification.</param>
        public static void WeaveWith(this AssemblyDefinition targetAssembly, ITypeWeaver weaver)
        {
            WeaveWith(targetAssembly, weaver, _ => true);
        }

        /// <summary>
        /// Modifies the chosen types in an assembly using the given type weaver. 
        /// </summary>
        /// <param name="targetAssembly">The target assembly to be modified</param>
        /// <param name="weaver">The type weaver that will be used to modify the selected types.</param>
        /// <param name="typeFilter">The filter that will determine which types should be modified.</param>
        public static void WeaveWith(this AssemblyDefinition targetAssembly, ITypeWeaver weaver,
            Func<TypeReference, bool> typeFilter)
        {
            var module = targetAssembly.MainModule;
            var types = module.Types.Where(t => typeFilter(t) && t.IsDefinition && weaver.ShouldWeave(t)).ToArray();
            foreach (var type in types)
            {
                weaver.Weave(type);
            }
        }
        
        /// <summary>
        /// Modifies the chosen types in an assembly using the given method weaver.
        /// </summary>
        /// <param name="targetAssembly">The target assembly that will be modified.</param>
        /// <param name="weaver">The method weaver that will be used to rewrite all the target methods.</param>
        /// <param name="typeFilter">The predicate that will determine which types will be modified.</param>
        public static void WeaveWith(this AssemblyDefinition targetAssembly, IMethodWeaver weaver,
            Func<TypeReference, bool> typeFilter)
        {
            var module = targetAssembly.MainModule;
            var types = module.Types.Where(t => typeFilter(t) && t.IsDefinition).ToArray();

            var methods = types.SelectMany(t => t.Methods.Where(m => m.HasBody && weaver.ShouldWeave(m))).ToArray();
            foreach (var method in methods)
            {
                weaver.Weave(method);
            }
        }

        /// <summary>
        /// Modifies the chosen types in an assembly using the given method weaver. 
        /// </summary>
        /// <param name="targetAssembly">The target assembly that will be modified.</param>
        /// <param name="weaver">The method weaver that will be used to rewrite all the target methods.</param>
        /// <param name="methodFilter">The predicate that will determine which methods will be modified.</param>
        public static void WeaveWith(this AssemblyDefinition targetAssembly, IMethodWeaver weaver,
            Func<MethodReference, bool> methodFilter)
        {
            var module = targetAssembly.MainModule;
            var types = module.Types.Where(t => !t.IsInterface && !t.IsValueType).ToArray();

            var methods = types.SelectMany(t => t.Methods.Where(m => m.HasBody && weaver.ShouldWeave(m) && methodFilter(m))).ToArray();
            foreach (var method in methods)
            {
                weaver.Weave(method);
            }
        }
        
        /// <summary>
        /// Modifies the current type using the given method weaver.
        /// </summary>
        /// <param name="targetType">The target type to be modified.</param>
        /// <param name="weaver">The method weaver that will modify the selected methods.</param>
        /// <param name="methodFilter">The method filter that will determine which methods should be modified.</param>
        public static void WeaveWith(this TypeDefinition targetType, IMethodWeaver weaver,
            Func<MethodReference, bool> methodFilter)
        {
            var methods = targetType.Methods.Where(m => m.HasBody && weaver.ShouldWeave(m) && methodFilter(m))
                .ToArray();
            foreach (var method in methods)
            {
                weaver.Weave(method);
            }
        }
    }
}