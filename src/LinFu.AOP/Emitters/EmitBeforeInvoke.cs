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
    public class EmitBeforeInvoke : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _surroundingImplementation;

        public EmitBeforeInvoke(VariableDefinition invocationInfo, VariableDefinition surroundingClassImplementation, VariableDefinition surroundingImplementation)
        {
            _invocationInfo = invocationInfo;
            _surroundingClassImplementation = surroundingClassImplementation;
            _surroundingImplementation = surroundingImplementation;
        }

        public void Emit(CilWorker IL)
        {
            var targetMethod = IL.GetMethod();
            var declaringType = targetMethod.DeclaringType;
            var module = declaringType.Module;

            var getSurroundingClassImplementation = new GetSurroundingClassImplementation(_invocationInfo,
                                                                                          _surroundingClassImplementation);
            // var classAroundInvoke = AroundInvokeRegistry.GetSurroundingImplementation(info);           
            getSurroundingClassImplementation.Emit(IL);

            // classAroundInvoke.BeforeInvoke(info);
            var skipInvoke = IL.Create(OpCodes.Nop);

            IL.EmitWriteLineIfNull("BeforeInvoke: SurroundingClassImplementation is null", _surroundingClassImplementation);
            IL.Emit(OpCodes.Ldloc, _surroundingClassImplementation);            
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingClassImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);

            IL.Append(skipInvoke);

            // if (surroundingImplementation != null) {
            if (!targetMethod.HasThis)
                return;

            var skipInvoke1 = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke1);

            var beforeInvoke1 = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke1);

            IL.Append(skipInvoke1);
            // }
        }
    }
}
