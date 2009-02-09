using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.IoC.Configuration;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A weaver class that automatically applies a <see cref="IHostWeaver{THost}"/> 
    /// instance to a target module definition.
    /// </summary>
    [Implements(typeof(IModuleWeaver), ServiceName = "AutoModuleWeaver")]
    internal class AutoModuleWeaver : IModuleWeaver
    {
        private readonly ITypeWeaver _typeWeaver;

        /// <summary>
        /// Initializes the class with the given <paramref name="typeWeaver"/> instance.
        /// </summary>
        /// <param name="typeWeaver">The <see cref="ITypeWeaver"/> that will be used to modify the given types in the target module definition.</param>
        public AutoModuleWeaver(ITypeWeaver typeWeaver)
        {
            _typeWeaver = typeWeaver;
        }

        /// <summary>
        /// Determines whether or not the current module should be modified.
        /// </summary>
        /// <param name="item">The target <see cref="ModuleDefinition"/> instance.</param>
        /// <returns>Always returns <c>true</c>.</returns>
        public bool ShouldWeave(ModuleDefinition item)
        {
            return true;
        }

        /// <summary>
        /// Modifies the current <see cref="ModuleDefinition"/>
        /// using the given <see cref="ITypeWeaver"/> instance.
        /// </summary>
        /// <param name="item">The target module definition.</param>
        public void Weave(ModuleDefinition item)
        {
            if (_typeWeaver == null)
                return;

            _typeWeaver.AddAdditionalMembers(item);

            foreach (TypeDefinition type in item.Types)
            {
                if (!_typeWeaver.ShouldWeave(type))
                    continue;

                _typeWeaver.Weave(type);
            }
        }


        /// <summary>
        /// Adds additional dependencies to the target <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="ModuleDefinition"/> that will host the modified types.</param>
        public void ImportReferences(ModuleDefinition module)
        {
            if (_typeWeaver == null)
                return;

            _typeWeaver.ImportReferences(module);
        }

        /// <summary>
        /// Adds additional members to the target <see cref="AssemblyDefinition"/>.
        /// </summary>
        /// <remarks>This implementation does nothing.</remarks>
        /// <param name="host">The target assembly definition.</param>
        public void AddAdditionalMembers(AssemblyDefinition host)
        {
            // Do nothing
        }
    }
}
