using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that loads assemblies into memory
    /// from disk.
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Loads the target assembly into memory.
        /// </summary>
        /// <param name="assemblyFile">The full path and filename of the assembly to load.</param>
        /// <returns>A loaded <see cref="Assembly"/> instance.</returns>
        Assembly Load(string assemblyFile);
    }
}