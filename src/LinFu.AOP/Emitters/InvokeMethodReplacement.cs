using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodDefinitionExtensions=LinFu.AOP.Cecil.Extensions.MethodDefinitionExtensions;

namespace LinFu.AOP.Cecil
{
    public class InvokeMethodReplacement : IInstructionEmitter
    {
        private readonly Instruction _executeOriginalInstructions;
        private readonly VariableDefinition _methodReplacementProvider;
        private readonly VariableDefinition _classMethodReplacementProvider;
        private readonly VariableDefinition _invocationInfo;

        public InvokeMethodReplacement(Instruction executeOriginalInstructions, VariableDefinition methodReplacementProvider, VariableDefinition classMethodReplacementProvider, VariableDefinition invocationInfo)
        {
            _executeOriginalInstructions = executeOriginalInstructions;
            _methodReplacementProvider = methodReplacementProvider;
            _classMethodReplacementProvider = classMethodReplacementProvider;
            _invocationInfo = invocationInfo;
        }

        public void Emit(CilWorker IL)
        {
            var module = IL.GetModule();
            var method = IL.GetMethod();
            var returnType = method.ReturnType.ReturnType;
            var methodReplacement = MethodDefinitionExtensions.AddLocal(method, typeof(IInterceptor));

            GetMethodReplacementInstance(method, IL, methodReplacement, _methodReplacementProvider, _invocationInfo);

            var skipGetClassMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brtrue, skipGetClassMethodReplacement);

            GetMethodReplacementInstance(method, IL, methodReplacement, _classMethodReplacementProvider, _invocationInfo);

            IL.Append(skipGetClassMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brfalse, _executeOriginalInstructions);

            // var returnValue = replacement.Intercept(info);
            InvokeInterceptor(module, IL, methodReplacement, returnType, _invocationInfo);
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
    }
}
