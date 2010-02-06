using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class AddOriginalInstructions : IInstructionEmitter
    {
        private readonly IEnumerable<Instruction> _oldInstructions;
        private readonly Instruction _endLabel;

        public AddOriginalInstructions(IEnumerable<Instruction> oldInstructions, Instruction endLabel)
        {
            _oldInstructions = oldInstructions;
            _endLabel = endLabel;
        }

        public void Emit(CilWorker IL)
        {
            var originalInstructions = new List<Instruction>(_oldInstructions);
            var lastInstruction = originalInstructions.LastOrDefault();

            if (lastInstruction != null && lastInstruction.OpCode == OpCodes.Ret)
            {
                // HACK: Convert the Ret instruction into a Nop
                // instruction so that the code will
                // fall through to the epilog
                lastInstruction.OpCode = OpCodes.Br;
                lastInstruction.Operand = _endLabel;
            }

            foreach (var instruction in (IEnumerable<Instruction>) originalInstructions)
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
            foreach (var instruction in originalInstructions)
            {
                IL.Append(instruction);
            }
        }
    }
}
