using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    ///     Represents a class that can modify existing <see cref="ModuleDefinition" /> instances.
    /// </summary>
    public interface IModuleWeaver : IWeaver<ModuleDefinition, AssemblyDefinition>
    {
    }
}