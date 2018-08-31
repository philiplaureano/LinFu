using Mono.Cecil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    ///     Represents a type that can modify <see cref="MethodDefinition" /> objects.
    /// </summary>
    public interface IMethodWeaver : IWeaver<MethodDefinition, TypeDefinition>
    {
    }
}