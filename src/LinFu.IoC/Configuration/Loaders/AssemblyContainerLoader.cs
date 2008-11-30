using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that loads configuration information
    /// from a given assembly.
    /// </summary>
    public class AssemblyContainerLoader : AssemblyTargetLoader<IServiceContainer>
    {
    }
}