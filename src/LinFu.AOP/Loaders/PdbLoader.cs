using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using System.Reflection;
using Mono.Cecil;
using System.IO;

namespace LinFu.AOP.Cecil.Loaders
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPdbLoader"/> interface.
    /// </summary>
    public class PdbLoader : IPdbLoader
    {
        /// <summary>
        /// Loads an assembly into memory.
        /// </summary>
        /// <param name="assemblyArray">The bytes that represent the target assembly.</param>
        /// <param name="pdbBytes">The bytes that represent the PDB file.</param>
        /// <returns>A <see cref="System.Reflection.Assembly"/> that represents the loaded assembly.</returns>
        public Assembly LoadAssembly(byte[] assemblyArray, byte[] pdbBytes)
        {
            // Load the assembly into the current application domain
            if (pdbBytes != null && pdbBytes.Length > 0)
                return Assembly.Load(assemblyArray, pdbBytes);

            return Assembly.Load(assemblyArray);
        }

        /// <summary>
        /// Loads the debug symbols from the target <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly that contains the symbols to be loaded.</param>
        public void LoadSymbols(AssemblyDefinition assembly)
        {
            var mainModule = assembly.MainModule;
            mainModule.LoadSymbols();
        }

        /// <summary>
        /// Saves the debug symbols for the  target<paramref name="assembly"/>.
        /// </summary>
        /// <param name="targetAssembly">The assembly that contains the symbols to be saved.</param>
        public void SaveSymbols(AssemblyDefinition targetAssembly)
        {
            // Update the debug symbols
            targetAssembly.MainModule.SaveSymbols();
        }
    }
}
