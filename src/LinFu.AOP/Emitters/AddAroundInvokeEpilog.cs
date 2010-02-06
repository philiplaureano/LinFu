using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class AddAroundInvokeEpilog : IInstructionEmitter
    {
        private readonly VariableDefinition _interceptionDisabled;
        private readonly VariableDefinition _surroundingImplementation;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _returnValue;

        public AddAroundInvokeEpilog(VariableDefinition interceptionDisabled, VariableDefinition surroundingImplementation, 
            VariableDefinition surroundingClassImplementation, VariableDefinition invocationInfo, VariableDefinition returnValue)
        {
            _interceptionDisabled = interceptionDisabled;
            _surroundingImplementation = surroundingImplementation;
            _surroundingClassImplementation = surroundingClassImplementation;
            _invocationInfo = invocationInfo;
            _returnValue = returnValue;
        }

        public void Emit(CilWorker IL)
        {
            var skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            var emitAfterInvoke = new EmitAfterInvoke(_surroundingImplementation, 
                _surroundingClassImplementation, _invocationInfo, _returnValue);

            emitAfterInvoke.Emit(IL);

            // }
            IL.Append(skipEpilog);
        }
    }
}
