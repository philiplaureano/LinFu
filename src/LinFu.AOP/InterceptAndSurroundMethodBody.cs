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

            var getInterceptionDisabled = _module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");
            var returnValue = method.AddLocal<object>();
            var classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            var modifiableType = _module.ImportType<IModifiableType>();
            GetInterceptionDisabled(method, IL, modifiableType, _interceptionDisabled, getInterceptionDisabled);

            // Construct the InvocationInfo instance
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var targetMethod = method;
            var interceptedMethod = method;
            //var surroundingImplementation = method.AddLocal<IAroundInvoke>();
            //var surroundingClassImplementation = method.AddLocal<IAroundInvoke>();

            
            _emitter.Emit(targetMethod, interceptedMethod, _invocationInfo);


            var surroundMethodBody = new SurroundMethodBody(_module, _methodReplacementProvider, _aroundInvokeProvider,
                                                            _invocationInfo, _interceptionDisabled, returnValue);

    //        var emitProlog = new AddAroundInvokeProlog(_methodReplacementProvider,
    //_aroundInvokeProvider, surroundingImplementation, surroundingClassImplementation, _invocationInfo, _interceptionDisabled);

            //emitProlog.Emit(IL);

            surroundMethodBody.AddProlog(method, IL);

            IL.Append(skipInvocationInfo);

            

            GetClassMethodReplacementProvider(IL, method, _module, _invocationInfo, classMethodReplacementProvider);

            

            var returnType = method.ReturnType.ReturnType;
            AddImplementation(method, _module, IL,
                              oldInstructions,
                              _methodReplacementProvider,
                              classMethodReplacementProvider,
                              _interceptionDisabled, _invocationInfo, returnValue);

            // Save the return value
            TypeReference voidType = _module.Import(typeof(void));

            //var emitEpilog = new AddAroundInvokeEpilog(_interceptionDisabled, surroundingImplementation, 
            //    surroundingClassImplementation, _invocationInfo, returnValue);

            //emitEpilog.Emit(IL);

            surroundMethodBody.AddEpilog(method, IL);

            if (returnType != voidType)
                IL.Emit(OpCodes.Ldloc, returnValue);

            IL.Emit(OpCodes.Ret);
        }

        private static void GetInterceptionDisabled(MethodDefinition hostMethod,
            CilWorker IL,
            TypeReference modifiableType, VariableDefinition interceptionDisabled,
            MethodReference getInterceptionDisabled)
        {
            if (!hostMethod.HasThis)
            {
                IL.Emit(OpCodes.Ldc_I4_0);
                IL.Emit(OpCodes.Stloc, interceptionDisabled);
                return;
            }

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, getInterceptionDisabled);
            IL.Emit(OpCodes.Stloc, interceptionDisabled);
        }

        private static void GetClassMethodReplacementProvider(CilWorker IL, MethodDefinition method,
            ModuleDefinition module, VariableDefinition invocationInfo,
            VariableDefinition classMethodReplacementProvider)
        {
            var getProvider = module.Import(typeof(MethodReplacementProviderRegistry).GetMethod("GetProvider"));

            if (method.HasThis)
                IL.Emit(OpCodes.Ldarg_0);
            else
                IL.Emit(OpCodes.Ldnull);

            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Call, getProvider);
            IL.Emit(OpCodes.Stloc, classMethodReplacementProvider);
        }

        private static void AddImplementation(MethodDefinition method,
            ModuleDefinition module,
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
            InvokeMethodReplacement(executeOriginalInstructions,
                IL, method, module, methodReplacementProvider,
                classMethodReplacementProvider, invocationInfo);

            IL.Emit(OpCodes.Br, endLabel);

            #region The original instruction block
            IL.Append(executeOriginalInstructions);
            AddOriginalInstructions(IL, oldInstructions, endLabel);

            #endregion

            // Mark the end of the method body
            IL.Append(endLabel);
            SaveReturnValue(module, IL, returnType, returnValue);
        }

        private static void AddOriginalInstructions(CilWorker IL, IEnumerable<Instruction> oldInstructions, Instruction endLabel)
        {
            var originalInstructions = new List<Instruction>(oldInstructions);
            var lastInstruction = originalInstructions.LastOrDefault();

            if (lastInstruction != null && lastInstruction.OpCode == OpCodes.Ret)
            {
                // HACK: Convert the Ret instruction into a Nop
                // instruction so that the code will
                // fall through to the epilog
                lastInstruction.OpCode = OpCodes.Br;
                lastInstruction.Operand = endLabel;
            }

            RedirectReturnsToLastInstruction(originalInstructions, lastInstruction);

            // Emit the original instructions
            foreach (var instruction in originalInstructions)
            {
                IL.Append(instruction);
            }
        }
        private static void InvokeMethodReplacement(Instruction executeOriginalInstructions,
           CilWorker IL,
           MethodDefinition method,
           ModuleDefinition module,
           VariableDefinition methodReplacementProvider,
           VariableDefinition classMethodReplacementProvider,
           VariableDefinition invocationInfo)
        {
            var returnType = method.ReturnType.ReturnType;
            var methodReplacement = method.AddLocal(typeof(IInterceptor));

            GetMethodReplacementInstance(method, IL, methodReplacement, methodReplacementProvider, invocationInfo);

            var skipGetClassMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brtrue, skipGetClassMethodReplacement);

            GetMethodReplacementInstance(method, IL, methodReplacement, classMethodReplacementProvider, invocationInfo);

            IL.Append(skipGetClassMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brfalse, executeOriginalInstructions);

            // var returnValue = replacement.Intercept(info);
            InvokeInterceptor(module, IL, methodReplacement, returnType, invocationInfo);
        }

        private static void InvokeInterceptor(ModuleDefinition module, CilWorker IL,
            VariableDefinition methodReplacement, TypeReference returnType, VariableDefinition invocationInfo)
        {
            var interceptMethod = module.ImportMethod<IInterceptor>("Intercept");
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);
            IL.PackageReturnValue(module, returnType);
        }

        private static void GetMethodReplacementInstance(MethodDefinition method,
            CilWorker IL,
            VariableDefinition methodReplacement,
            VariableDefinition methodReplacementProvider, VariableDefinition invocationInfo)
        {
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var pushInstance = method.HasThis ? IL.Create(OpCodes.Ldarg_0) : IL.Create(OpCodes.Ldnull);

            var getReplacement = module.ImportMethod<IMethodReplacementProvider>("GetMethodReplacement");
            IL.Emit(OpCodes.Ldloc, methodReplacementProvider);

            var skipGetMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Brfalse, skipGetMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacementProvider);

            IL.Append(pushInstance);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, getReplacement);
            IL.Emit(OpCodes.Stloc, methodReplacement);

            IL.Append(skipGetMethodReplacement);
        }

        private static void SaveReturnValue(ModuleDefinition module, CilWorker IL,
            TypeReference returnType, VariableDefinition returnValue)
        {
            var voidType = module.ImportType(typeof(void));
            var returnTypeIsValueType = returnType != voidType && returnType.IsValueType;

            if (returnType is GenericParameter || returnTypeIsValueType)
                IL.Create(OpCodes.Box, returnType);

            if (returnType != voidType)
                IL.Create(OpCodes.Stloc, returnValue);
        }



        private static void RedirectReturnsToLastInstruction(IEnumerable<Instruction> originalInstructions, Instruction lastInstruction)
        {
            foreach (var instruction in originalInstructions)
            {
                if (instruction.OpCode != OpCodes.Ret || instruction == lastInstruction)
                    continue;

                if (lastInstruction == null)
                    continue;

                // HACK: Modify all ret instructions to call
                // the epilog after execution
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastInstruction;
            }
        }
    }
}
