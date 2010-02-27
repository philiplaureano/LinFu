using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LinFu.AOP.Cecil.Interfaces;

namespace LinFu.AOP.Cecil
{
    public class GetSurroundingClassImplementation : IInstructionEmitter
    {
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _surroundingClassImplementation;
        private readonly MethodInfo _getSurroundingImplementationMethod;

        public GetSurroundingClassImplementation(VariableDefinition invocationInfo, 
            VariableDefinition surroundingClassImplementation, MethodInfo getSurroundingImplementationMethod)
        {
            _invocationInfo = invocationInfo;
            _surroundingClassImplementation = surroundingClassImplementation;
            _getSurroundingImplementationMethod = getSurroundingImplementationMethod;
        }

        public void Emit(CilWorker IL)
        {
            var method = IL.GetMethod();
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            var getSurroundingImplementation = module.Import(_getSurroundingImplementationMethod);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingClassImplementation);
        }
    }
}
