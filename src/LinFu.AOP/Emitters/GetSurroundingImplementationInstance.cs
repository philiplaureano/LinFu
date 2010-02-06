using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class GetSurroundingImplementationInstance : IInstructionEmitter
    {
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingImplementation;

        private readonly Instruction _skipGetSurroundingImplementation;

        public GetSurroundingImplementationInstance(VariableDefinition aroundInvokeProvider, VariableDefinition invocationInfo, VariableDefinition surroundingImplementation, Instruction skipGetSurroundingImplementation)
        {
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _surroundingImplementation = surroundingImplementation;
            _skipGetSurroundingImplementation = skipGetSurroundingImplementation;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();

            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, _skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingImplementation);
        }
    }
}
