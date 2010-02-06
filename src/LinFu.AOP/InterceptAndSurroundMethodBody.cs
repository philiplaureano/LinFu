using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LinFu.Reflection.Emit;

namespace LinFu.AOP.Cecil
{
    public class InterceptAndSurroundMethodBody : IMethodBodyRewriter
    {
        private VariableDefinition _interceptionDisabled;

        private VariableDefinition _invocationInfo;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _aroundInvokeProvider;
        private IEmitInvocationInfo _emitter;
        private ModuleDefinition _module;

        public InterceptAndSurroundMethodBody(ModuleDefinition module)
            : this(module, new InvocationInfoEmitter())
        {
        }

        public InterceptAndSurroundMethodBody(ModuleDefinition module, IEmitInvocationInfo emitter)
        {
            _module = module;
            _emitter = emitter;
        }

        public void Rewrite(MethodDefinition method, CilWorker IL,
            IEnumerable<Instruction> oldInstructions)
        {
            _interceptionDisabled = method.AddLocal<bool>();
            _invocationInfo = method.AddLocal<IInvocationInfo>();
            _aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            _methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();


            var returnValue = method.AddLocal<object>();
            var classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            var getInterceptionDisabled = new GetInterceptionDisabled(method, _interceptionDisabled);
            getInterceptionDisabled.Emit(IL);

            // Construct the InvocationInfo instance
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var targetMethod = method;
            var interceptedMethod = method;
            _emitter.Emit(targetMethod, interceptedMethod, _invocationInfo);


            var surroundMethodBody = new SurroundMethodBody(_methodReplacementProvider, _aroundInvokeProvider,
                                                            _invocationInfo, _interceptionDisabled, returnValue);
            surroundMethodBody.AddProlog(method, IL);

            IL.Append(skipInvocationInfo);


            var getClassMethodReplacementProvider = new GetClassMethodReplacementProvider(_invocationInfo, classMethodReplacementProvider);
            getClassMethodReplacementProvider.Emit(IL);

            var returnType = method.ReturnType.ReturnType;
            AddMethodReplacementImplementation(method, IL,
                              oldInstructions,
                              _methodReplacementProvider,
                              classMethodReplacementProvider,
                              _interceptionDisabled, _invocationInfo, returnValue);

            // Save the return value
            TypeReference voidType = _module.Import(typeof(void));
            surroundMethodBody.AddEpilog(method, IL);

            if (returnType != voidType)
                IL.Emit(OpCodes.Ldloc, returnValue);

            IL.Emit(OpCodes.Ret);
        }

        private static void AddMethodReplacementImplementation(MethodDefinition method,
            CilWorker IL,
            IEnumerable<Instruction> oldInstructions,
            VariableDefinition methodReplacementProvider,
            VariableDefinition classMethodReplacementProvider,
            VariableDefinition interceptionDisabled,
            VariableDefinition invocationInfo, VariableDefinition returnValue)
        {
            var returnType = method.ReturnType.ReturnType;

            var endLabel = IL.Create(OpCodes.Nop);
            var executeOriginalInstructions = IL.Create(OpCodes.Nop);

            // Execute the method body replacement if and only if
            // interception is enabled
            IL.Emit(OpCodes.Ldloc, interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, executeOriginalInstructions);

            var invokeReplacement = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, methodReplacementProvider);
            IL.Emit(OpCodes.Brtrue, invokeReplacement);

            IL.Emit(OpCodes.Ldloc, classMethodReplacementProvider);
            IL.Emit(OpCodes.Brtrue, invokeReplacement);

            IL.Emit(OpCodes.Br, executeOriginalInstructions);
            IL.Append(invokeReplacement);

            // This is equivalent to the following code:
            // var replacement = provider.GetMethodReplacement(info);
            var invokeMethodReplacement = new InvokeMethodReplacement(executeOriginalInstructions, 
                methodReplacementProvider, classMethodReplacementProvider, invocationInfo);
            invokeMethodReplacement.Emit(IL);

            IL.Emit(OpCodes.Br, endLabel);

            #region The original instruction block
            IL.Append(executeOriginalInstructions);
            var addOriginalInstructions = new AddOriginalInstructions(oldInstructions, endLabel);
            addOriginalInstructions.Emit(IL);

            #endregion

            // Mark the end of the method body
            IL.Append(endLabel);

            var saveReturnValue = new SaveReturnValue(returnType, returnValue);
            saveReturnValue.Emit(IL);
        }       
    }
}
