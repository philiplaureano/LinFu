using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A class that extends <see cref="AssemblyDefinition"/> instances.
    /// </summary>
    public static class AssemblyDefinitionExtensions
    {
        /// <summary>
        /// Removes the strong-name signature from the <paramref name="sourceAssembly"/>.
        /// </summary>
        /// <param name="sourceAssembly"></param>
        public static void RemoveStrongName(this AssemblyDefinition sourceAssembly)
        {
            var nameDef = sourceAssembly.Name;

            // Remove the strong name
            nameDef.PublicKey = null;
            nameDef.PublicKeyToken = null;
            nameDef.HashAlgorithm = AssemblyHashAlgorithm.None;
            nameDef.Flags = ~AssemblyFlags.PublicKey;
            nameDef.HasPublicKey = false;
        }
    }
}
