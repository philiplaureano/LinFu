using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that is capable of manipulating instructions within a given method body.
    /// </summary>
    public interface IInstructionEmitter
    {
        /// <summary>
        /// Emits a set of instructions to the given <paramref name="IL">ILProcessor</paramref>.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> responsible for the target method body.</param>
        void Emit(ILProcessor IL);
    }
}