using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IInstructionProvider"/> class.
    /// </summary>
    internal class InstructionProvider : IInstructionProvider
    {
        /// <summary>
        /// Determines the instructions for a given method.
        /// </summary>
        /// <param name="method">The source method that contains the instructions.</param>
        /// <returns>The set of instructions for the given method.</returns>
        public IEnumerable<Instruction> GetInstructions(MethodDefinition method)
        {
            // Save the old instructions
            var oldInstructions = new LinkedList<Instruction>();
            foreach (Instruction instruction in method.Body.Instructions)
            {
                oldInstructions.AddLast(instruction);
            }

            return oldInstructions;
        }
    }
}
