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
    public class AddAroundInvokeProlog : IInstructionEmitter
    {
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly VariableDefinition _aroundInvokeProvider;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingImplementation;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _interceptionDisabled;

        public AddAroundInvokeProlog(VariableDefinition methodReplacementProvider, VariableDefinition aroundInvokeProvider, VariableDefinition invocationInfo, VariableDefinition surroundingImplementation, VariableDefinition surroundingClassImplementation, VariableDefinition interceptionDisabled)
        {
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _surroundingImplementation = surroundingImplementation;
            _surroundingClassImplementation = surroundingClassImplementation;
            _interceptionDisabled = interceptionDisabled;
        }

        public void Emit(CilWorker IL)
        {
            var method = IL.GetMethod();

            var getProvider = new GetMethodReplacementProvider(_methodReplacementProvider);


            var skipProlog = IL.Create(OpCodes.Nop);
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();

            var emitBeforeInvoke = new EmitBeforeInvoke(_invocationInfo, _surroundingClassImplementation,
                                                        _surroundingImplementation);

            if (method.HasThis)
            {
                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, modifiableType);
                IL.Emit(OpCodes.Brfalse, skipProlog);
            }

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipProlog);

            // var provider = this.MethodReplacementProvider;
            getProvider.Emit(IL);

            var getAroundInvokeProvider = new GetAroundInvokeProvider(_aroundInvokeProvider);
            getAroundInvokeProvider.Emit(IL);

            // if (aroundInvokeProvider != null ) {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            var getSurroundingImplementationInstance = new GetSurroundingImplementationInstance(_aroundInvokeProvider,
                                                                                                _invocationInfo,
                                                                                                _surroundingImplementation,
                                                                                                skipGetSurroundingImplementation);
            getSurroundingImplementationInstance.Emit(IL);

            // }

            IL.Append(skipGetSurroundingImplementation);

            emitBeforeInvoke.Emit(IL);

            IL.Append(skipProlog);
        }
    }
}
