using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Adds helper methods to the <see cref="ModuleDefinition"/> class.
    /// </summary>
    public static class ModuleDefinitionExtensions
    {
        /// <summary>
        /// Applies a <see cref="ITypeWeaver"/> instance to all types
        /// within the given <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The target module.</param>
        /// <param name="weaver">The <see cref="ITypeWeaver"/> instance that will modify the types within the given module.</param>
        public static void WeaveWith(this ModuleDefinition module, ITypeWeaver weaver)
        {
            var targetTypes = from TypeDefinition type in module.Types
                              where weaver.ShouldWeave(type)
                              select type;

            // Modify the host module
            weaver.ImportReferences(module);
            weaver.AddAdditionalMembers(module);
            
            foreach(var item in targetTypes)
            {
                weaver.Weave(item);
            }
        }
    }
}
