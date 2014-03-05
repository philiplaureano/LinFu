using System.Collections.Generic;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an instruction emitter that adds method body replacement support to a given method body.
    /// </summary>
    public class AddMethodReplacementImplementation : IInstructionEmitter
    {
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly VariableDefinition _interceptionDisabled;
        private readonly VariableDefinition _invocationInfo;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly IEnumerable<Instruction> _oldInstructions;
        private readonly VariableDefinition _returnValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMethodReplacementImplementation"/> class.
        /// </summary>
        /// <param name="parameters">The set of parameters that describe the target method body.</param>
        public AddMethodReplacementImplementation(IMethodBodyRewriterParameters parameters)
        {
            _oldInstructions = parameters.OldInstructions;
            _interceptionDisabled = parameters.InterceptionDisabled;
            _methodReplacementProvider = parameters.MethodReplacementProvider;
            _classMethodReplacementProvider = parameters.ClassMethodReplacementProvider;
            _invocationInfo = parameters.InvocationInfo;
            _returnValue = parameters.ReturnValue;
        }


        /// <summary>
        /// Adds method body interception to the target method.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> pointing to the target method body.</param>
        public void Emit(CilWorker IL)
        {
            var method = IL.GetMethod();
            var returnType = method.ReturnType.ReturnType;

            var endLabel = IL.Create(OpCodes.Nop);
            var executeOriginalInstructions = IL.Create(OpCodes.Nop);

            // Execute the method body replacement if and only if
            // interception is enabled
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, executeOriginalInstructions);

            var invokeReplacement = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, _methodReplacementProvider);
            IL.Emit(OpCodes.Brtrue, invokeReplacement);

            IL.Emit(OpCodes.Ldloc, _classMethodReplacementProvider);
            IL.Emit(OpCodes.Brtrue, invokeReplacement);

            IL.Emit(OpCodes.Br, executeOriginalInstructions);
            IL.Append(invokeReplacement);

            // This is equivalent to the following code:
            // var replacement = provider.GetMethodReplacement(info);
            var invokeMethodReplacement = new InvokeMethodReplacement(executeOriginalInstructions,
                _methodReplacementProvider,
                _classMethodReplacementProvider, _invocationInfo);
            invokeMethodReplacement.Emit(IL);

            IL.Emit(OpCodes.Br, endLabel);


            IL.Append(executeOriginalInstructions);
            var addOriginalInstructions = new AddOriginalInstructions(_oldInstructions, endLabel);
            addOriginalInstructions.Emit(IL);


            // Mark the end of the method body
            IL.Append(endLabel);

            var saveReturnValue = new SaveReturnValue(returnType, _returnValue);
            saveReturnValue.Emit(IL);
        }
    }
}