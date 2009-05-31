using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can provide the instructions for a given method.
    /// </summary>
    public interface IInstructionProvider
    {
        /// <summary>
        /// Determines the instructions for a given method.
        /// </summary>
        /// <param name="method">The source method that contains the instructions.</param>
        /// <returns>The set of instructions for the given method.</returns>
        IEnumerable<Instruction> GetInstructions(MethodDefinition method);
    }
}
