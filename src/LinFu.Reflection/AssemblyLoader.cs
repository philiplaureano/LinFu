using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that loads assemblies into memory
    /// from disk.
    /// </summary>
    public class AssemblyLoader : IAssemblyLoader
    {
        #region IAssemblyLoader Members

        /// <summary>
        /// Loads the target assembly into memory.
        /// </summary>
        /// <param name="assemblyFile">The full path and filename of the assembly to load.</param>
        /// <returns>A loaded <see cref="Assembly"/> instance.</returns>
        public virtual Assembly Load(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }

        #endregion
    }
}