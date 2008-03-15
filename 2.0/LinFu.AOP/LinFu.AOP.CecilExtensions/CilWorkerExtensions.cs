using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace LinFu.AOP.CecilExtensions
{
    public static class CilWorkerExtensions
    {
        public static void AppendInstructions(this CilWorker IL, IEnumerable<Instruction> instructions)
        {
            foreach (Instruction current in instructions)
            {
                IL.Append(current);
            }
        }

    }
}
