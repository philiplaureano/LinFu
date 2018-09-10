using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    ///     Represents a type that can modify method bodies.
    /// </summary>
    public interface IMethodBodyRewriter
    {
        /// <summary>
        ///     Rewrites a target method using the given ILProcessor.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The ILProcessor that will be used to rewrite the target method.</param>
        /// <param name="oldInstructions">The original instructions from the target method body.</param>
        void Rewrite(MethodDefinition method, ILProcessor IL, IEnumerable<Instruction> oldInstructions);
    }
}