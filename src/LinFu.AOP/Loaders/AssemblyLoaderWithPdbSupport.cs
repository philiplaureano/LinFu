using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using System.Reflection;
using System.IO;

namespace LinFu.AOP.Cecil.Loaders
{
    /// <summary>
    /// Represents an <see cref="AssemblyLoader"/> class that adds support for loading PDB files into memory every time an assembly is loaded into memory.
    /// </summary>
    public class AssemblyLoaderWithPdbSupport : AssemblyLoader
    {
        /// <summary>
        /// Initializes a new instance of the AssemblyLoaderWithPdbSupport class.
        /// </summary>
        /// <param name="loader">The <see cref="IAssemblyLoader"/> that will perform the actual load operation.</param>
        public AssemblyLoaderWithPdbSupport(IAssemblyLoader loader)
        {
            AssemblyLoader = loader;
        }

        /// <summary>
        /// Loads the target assembly (and its corresponding PDB file) into memory.
        /// </summary>
        /// <param name="assemblyFile">The full path and filename of the assembly to load.</param>
        /// <returns>A loaded <see cref="Assembly"/> instance.</returns>
        public sealed override System.Reflection.Assembly Load(string assemblyFile)
        {
            if (AssemblyLoader == null)
                throw new NullReferenceException("The AssemblyLoader property cannot be null");

            string pdbFile = string.Format("{0}.pdb", Path.GetFileNameWithoutExtension(assemblyFile));            

            // Check to see if the symbols file is available
            bool hasSymbols = File.Exists(pdbFile);
            var pdbFileExists = File.Exists(pdbFile);
            Assembly result = null;

            var pdbTempFileName = Path.GetTempFileName();
            var assemblyBackupFile = Path.GetTempFileName();

            try
            {                
                // Save the old PDB file
                if (pdbFileExists)
                    File.Copy(pdbFile, pdbTempFileName, true);

                // Save the assembly file                
                File.Copy(assemblyFile, assemblyBackupFile, true);

                // Do the actual load operation
                result = AssemblyLoader.Load(assemblyFile);
            }
            finally
            {
                RemoveTemporaryFiles(assemblyFile, pdbFile, pdbTempFileName, assemblyBackupFile);
            }

            return result;
        }

        /// <summary>
        /// Removes the temporary backup files that were created during the load operation.
        /// </summary>
        /// <param name="assemblyFile">The full path and location of the original assembly file.</param>
        /// <param name="pdbFile">The full path and location of the original PDB file.</param>
        /// <param name="pdbTempFileName">The full path and location of the temporary pdb file.</param>
        /// <param name="assemblyBackupFile">The full path and location of the backup assembly file.</param>
        private static void RemoveTemporaryFiles(string assemblyFile, string pdbFile, string pdbTempFileName, string assemblyBackupFile)
        {
            if (File.Exists(pdbTempFileName))
            {
                File.Copy(pdbTempFileName, pdbFile, true);
                File.Delete(pdbTempFileName);
            }

            if (!File.Exists(assemblyBackupFile))
                return;
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAssemblyLoader"/> instance that will be used to load assemblies into memory.
        /// </summary>
        public virtual IAssemblyLoader AssemblyLoader
        {
            get;
            set;
        }        
    }
}
