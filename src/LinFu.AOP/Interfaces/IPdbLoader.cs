using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;
using System.IO;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can load PDB files from disk.
    /// </summary>
    public interface IPdbLoader
    {
        /// <summary>
        /// Loads an assembly into memory.
        /// </summary>
        /// <param name="assemblyArray">The bytes that represent the target assembly.</param>
        /// <param name="pdbBytes">The bytes that represent the PDB file.</param>
        /// <returns>A <see cref="System.Reflection.Assembly"/> that represents the loaded assembly.</returns>
        Assembly LoadAssembly(byte[] assemblyArray, byte[] pdbBytes);

        /// <summary>
        /// Loads the debug symbols from the target <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly that contains the symbols to be loaded.</param>
        void LoadSymbols(AssemblyDefinition assembly);


        /// <summary>
        /// Saves the debug symbols for the  target<paramref name="assembly"/>.
        /// </summary>
        /// <param name="targetAssembly">The assembly that contains the symbols to be saved.</param>
        void SaveSymbols(AssemblyDefinition targetAssembly);
    }
}
