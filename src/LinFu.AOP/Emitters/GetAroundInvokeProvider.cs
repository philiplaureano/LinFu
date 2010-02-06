using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class GetAroundInvokeProvider : IInstructionEmitter
    {
        private readonly VariableDefinition _aroundInvokeProvider;

        public GetAroundInvokeProvider(VariableDefinition aroundInvokeProvider)
        {
            _aroundInvokeProvider = aroundInvokeProvider;
        }

        public void Emit(CilWorker IL)
        {
            var method = IL.GetMethod();
            var module = IL.GetModule();

            // var aroundInvokeProvider = this.AroundInvokeProvider;
            var getAroundInvokeProvider = module.ImportMethod<IModifiableType>("get_AroundInvokeProvider");

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
                return;
            }

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
        }
    }
}
