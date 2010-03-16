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
    /// <summary>
    /// Represents an instruction emitter that saves the return value from a given method call.
    /// </summary>
    public class SaveReturnValue : IInstructionEmitter
    {
        private readonly TypeReference _returnType;
        private readonly VariableDefinition _returnValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveReturnValue"/> class.
        /// </summary>
        /// <param name="returnType">The return type.</param>
        /// <param name="returnValue">The return value.</param>
        public SaveReturnValue(TypeReference returnType, VariableDefinition returnValue)
        {
            _returnType = returnType;
            _returnValue = returnValue;
        }

        /// <summary>
        /// Saves the return value from a given method call.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> pointing to the target method body.</param>
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
