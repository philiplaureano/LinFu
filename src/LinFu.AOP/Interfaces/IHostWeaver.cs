using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a weaver class that can modify its host.
    /// </summary>
    /// <typeparam name="THost">The host that holds the item to be modified.</typeparam>
    public interface IHostWeaver<THost>
    {
        /// <summary>
        /// Imports references into the target <see cref="ModuleDefinition"/> instance.
        /// </summary>
        /// <param name="module">The module that will hold the modified item.</param>
        void ImportReferences(ModuleDefinition module);

        /// <summary>
        /// Adds additional members to the host type.
        /// </summary>
        /// <param name="host">The host that holds the current item being modified.</param>
        void AddAdditionalMembers(THost host);
    }
}
