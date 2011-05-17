﻿using System.Collections.Generic;
using System.Linq;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an instruction emitter that adds the original method instructions to a given method body.
    /// </summary>
    public class AddOriginalInstructions : IInstructionEmitter
    {
        private readonly Instruction _endLabel;
        private readonly IEnumerable<Instruction> _oldInstructions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOriginalInstructions"/> class.
        /// </summary>
        /// <param name="oldInstructions">The original method instructions.</param>
        /// <param name="endLabel">The instruction label that marks the end of the method body.</param>
        public AddOriginalInstructions(IEnumerable<Instruction> oldInstructions, Instruction endLabel)
        {
            _oldInstructions = oldInstructions;
            _endLabel = endLabel;
        }

        #region IInstructionEmitter Members

        /// <summary>
        /// Adds the original instructions to a given method body.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> responsible for the target method body.</param>
        public void Emit(ILProcessor IL)
        {
            var originalInstructions = new List<Instruction>(_oldInstructions);
            Instruction lastInstruction = originalInstructions.LastOrDefault();

            if (lastInstruction != null && lastInstruction.OpCode == OpCodes.Ret)
            {
                // HACK: Convert the Ret instruction into a Nop
                // instruction so that the code will
                // fall through to the epilog
                lastInstruction.OpCode = OpCodes.Br;
                lastInstruction.Operand = _endLabel;
            }

            foreach (Instruction instruction in (IEnumerable<Instruction>) originalInstructions)
            {
                if (instruction.OpCode != OpCodes.Ret || instruction == lastInstruction)
                    continue;

                if (lastInstruction == null)
                    continue;

                // HACK: Modify all ret instructions to call
                // the epilog after execution
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastInstruction;
            }

            // Emit the original instructions
            foreach (Instruction instruction in originalInstructions)
            {
                IL.Append(instruction);
            }
        }

        #endregion
    }
}