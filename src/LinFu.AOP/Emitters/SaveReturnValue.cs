using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class SaveReturnValue : IInstructionEmitter
    {
        private readonly TypeReference _returnType;
        private readonly VariableDefinition _returnValue;

        public SaveReturnValue(TypeReference returnType, VariableDefinition returnValue)
        {
            _returnType = returnType;
            _returnValue = returnValue;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();
            var voidType = module.ImportType(typeof(void));
            var returnTypeIsValueType = _returnType != voidType && _returnType.IsValueType;

            if (_returnType is GenericParameter || returnTypeIsValueType)
                IL.Create(OpCodes.Box, _returnType);

            if (_returnType != voidType)
                IL.Create(OpCodes.Stloc, _returnValue);
        }
    }
}
