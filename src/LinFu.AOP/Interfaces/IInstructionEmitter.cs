using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    public interface IInstructionEmitter
    {
        void Emit(CilWorker IL);
    }
}
