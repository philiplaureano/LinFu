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
    public class EmitAfterInvoke : IInstructionEmitter
    {
        private readonly VariableDefinition _surroundingImplementation;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _returnValue;

        public EmitAfterInvoke(VariableDefinition surroundingImplementation, VariableDefinition surroundingClassImplementation,
            VariableDefinition invocationInfo, VariableDefinition returnValue)
        {
            _surroundingImplementation = surroundingImplementation;
            _surroundingClassImplementation = surroundingClassImplementation;
            _invocationInfo = invocationInfo;
            _returnValue = returnValue;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();

            // instanceAroundInvoke.AfterInvoke(info, returnValue);
            Emit(IL, module, _surroundingImplementation, _invocationInfo, _returnValue);

            // classAroundInvoke.AfterInvoke(info, returnValue);
            Emit(IL, module, _surroundingClassImplementation, _invocationInfo, _returnValue);
        }

        private static void Emit(CilWorker IL, ModuleDefinition module,
           VariableDefinition surroundingImplementation,
           VariableDefinition invocationInfo,
           VariableDefinition returnValue)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);

            var skipPrint = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brtrue, skipPrint);

            IL.Append(skipPrint);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var aroundInvoke = module.ImportMethod<IAfterInvoke>("AfterInvoke");
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Ldloc, returnValue);
            IL.Emit(OpCodes.Callvirt, aroundInvoke);

            IL.Append(skipInvoke);
        }
    }
}
