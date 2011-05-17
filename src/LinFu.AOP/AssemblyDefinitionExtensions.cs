﻿using Mono.Cecil;

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
            AssemblyNameDefinition nameDef = sourceAssembly.Name;

            // Remove the strong name
            nameDef.PublicKey = null;
            nameDef.PublicKeyToken = null;
            nameDef.HashAlgorithm = AssemblyHashAlgorithm.None;
            nameDef.HasPublicKey = false;
        }
    }
}