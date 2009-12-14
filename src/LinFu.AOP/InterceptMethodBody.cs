using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a method rewriter type that adds interception capabilities to any given method body.
    /// </summary>
    public class InterceptMethodBody : BaseMethodRewriter
    {
        private readonly Func<MethodReference, bool> _methodFilter;

        private VariableDefinition _interceptionDisabled;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _classMethodReplacementProvider;

        private VariableDefinition _aroundInvokeProvider;
        private VariableDefinition _surroundingImplementation;
        private VariableDefinition _surroundingClassImplementation;
        private VariableDefinition _returnValue;

        private MethodReference _getInterceptionDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptMethodBody"/> class.
        /// </summary>
        /// <param name="methodFilter">The method filter that will determine the methods with the method bodies that will be intercepted.</param>
        public InterceptMethodBody(Func<MethodReference, bool> methodFilter)
        {
            _methodFilter = methodFilter;
        }

        /// <summary>
        /// Determines whether or not the given method should be modified.
        /// </summary>
        /// <param name="targetMethod">The target method.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not a method should be rewritten.</returns>
        protected override bool ShouldRewrite(MethodDefinition targetMethod)
        {
            return _methodFilter(targetMethod);
        }

        /// <summary>
        /// Rewrites the instructions in the target method body.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected override void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            var modifiableType = module.ImportType<IModifiableType>();
            _getInterceptionDisabled = module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");


            // Determine whether or not the method should be intercepted
            GetInterceptionDisabled(method, IL, modifiableType);

            // Construct the InvocationInfo instance
            GetInvocationInfo(method, IL);

            GetClassMethodReplacementProvider(IL, method, module);

            _surroundingImplementation = method.AddLocal<IAroundInvoke>();
            _surroundingClassImplementation = method.AddLocal<IAroundInvoke>();

            AddProlog(method, IL);

            var returnType = method.ReturnType.ReturnType;
            AddMethodBodyImplementation(method, module, IL, oldInstructions);

            // Save the return value
            TypeReference voidType = module.Import(typeof(void));

            AddEpilog(IL, module, returnType);

            if (returnType != voidType)
                IL.Emit(OpCodes.Ldloc, _returnValue);

            IL.Emit(OpCodes.Ret);
        }

        private void GetInterceptionDisabled(MethodDefinition method, CilWorker IL, TypeReference modifiableType)
        {
            _interceptionDisabled = method.AddLocal<bool>();

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldc_I4_0);
                IL.Emit(OpCodes.Stloc, _interceptionDisabled);
                return;
            }

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, _getInterceptionDisabled);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);
        }

        private void GetInvocationInfo(MethodDefinition method, CilWorker IL)
        {
            _invocationInfo = method.AddLocal<IInvocationInfo>();
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var emitter = new InvocationInfoEmitter();
            emitter.Emit(method, method, _invocationInfo);

            IL.Append(skipInvocationInfo);
        }

        private void AddMethodBodyImplementation(MethodDefinition method, ModuleDefinition module, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var returnType = method.ReturnType.ReturnType;
            _returnValue = method.AddLocal<object>();

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
            InvokeMethodReplacement(executeOriginalInstructions, IL, method, module);
            IL.Emit(OpCodes.Br, endLabel);

            #region The original instruction block
            IL.Append(executeOriginalInstructions);
            AddOriginalInstructions(IL, oldInstructions, endLabel);

            #endregion

            // Mark the end of the method body
            IL.Append(endLabel);
            SaveReturnValue(module, IL, returnType);
        }        

        private void InvokeMethodReplacement(Instruction executeOriginalInstructions, CilWorker IL, MethodDefinition method, ModuleDefinition module)
        {
            var returnType = method.ReturnType.ReturnType;
            var methodReplacement = method.AddLocal(typeof(IInterceptor));
            
            GetMethodReplacementInstance(method, IL, methodReplacement, _methodReplacementProvider);            

            var skipGetClassMethodReplacement = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brtrue, skipGetClassMethodReplacement);
           
            GetMethodReplacementInstance(method, IL, methodReplacement, _classMethodReplacementProvider);

            IL.Append(skipGetClassMethodReplacement);
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Brfalse, executeOriginalInstructions);

            // var returnValue = replacement.Intercept(info);
            InvokeInterceptor(module, IL, methodReplacement, returnType);
        }

        private void InvokeInterceptor(ModuleDefinition module, CilWorker IL, VariableDefinition methodReplacement, TypeReference returnType)
        {
            var interceptMethod = module.ImportMethod<IInterceptor>("Intercept");
            IL.Emit(OpCodes.Ldloc, methodReplacement);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);
            IL.PackageReturnValue(module, returnType);
        }

        private void GetMethodReplacementInstance(MethodDefinition method, CilWorker IL,  VariableDefinition methodReplacement, VariableDefinition methodReplacementProvider)
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
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, getReplacement);            
            IL.Emit(OpCodes.Stloc, methodReplacement);

            IL.Append(skipGetMethodReplacement);
        }

        private void SaveReturnValue(ModuleDefinition module, CilWorker IL, TypeReference returnType)
        {
            var voidType = module.ImportType(typeof(void));
            var returnTypeIsValueType = returnType != voidType && returnType.IsValueType;

            if (returnType is GenericParameter || returnTypeIsValueType)
                IL.Create(OpCodes.Box, returnType);

            if (returnType != voidType)
                IL.Create(OpCodes.Stloc, _returnValue);
        }

        private void AddEpilog(CilWorker IL, ModuleDefinition module, TypeReference returnType)
        {
            var skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            EmitAfterInvoke(IL, module, skipEpilog);

            // }
            IL.Append(skipEpilog);
        }

        protected virtual void AddProlog(MethodDefinition method, CilWorker IL)
        {
            var skipProlog = IL.Create(OpCodes.Nop);
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();

            if (method.HasThis)
            {
                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, modifiableType);
                IL.Emit(OpCodes.Brfalse, skipProlog);
            }

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipProlog);

            // var provider = this.MethodReplacementProvider;
            GetMethodReplacementProvider(method, module, IL);
            GetAroundInvokeProvider(method, module, IL);

            // if (aroundInvokeProvider != null ) {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            GetSurroundingImplementationInstance(module, IL, skipGetSurroundingImplementation);

            // }

            IL.Append(skipGetSurroundingImplementation);

            EmitBeforeInvoke(module, IL);

            IL.Append(skipProlog);
        }

        private void GetClassMethodReplacementProvider(CilWorker IL, MethodDefinition method, ModuleDefinition module)
        {
            _classMethodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();
            var getProvider = module.Import(typeof(MethodReplacementProviderRegistry).GetMethod("GetProvider"));

            if (method.HasThis)
                IL.Emit(OpCodes.Ldarg_0);
            else
                IL.Emit(OpCodes.Ldnull);

            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getProvider);
            IL.Emit(OpCodes.Stloc, _classMethodReplacementProvider);
        }

        private void EmitAfterInvoke(CilWorker IL, ModuleDefinition module, Instruction skipEpilog)
        {
            // instanceAroundInvoke.AfterInvoke(info, returnValue);
            EmitAfterInvoke(IL, module, _surroundingImplementation);

            // classAroundInvoke.AfterInvoke(info, returnValue);
            EmitAfterInvoke(IL, module, _surroundingClassImplementation);
        }

        private void EmitAfterInvoke(CilWorker IL, ModuleDefinition module, VariableDefinition surroundingImplementation)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var aroundInvoke = module.ImportMethod<IAfterInvoke>("AfterInvoke");
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Ldloc, _returnValue);
            IL.Emit(OpCodes.Callvirt, aroundInvoke);

            IL.Append(skipInvoke);
        }

        private void EmitBeforeInvoke(ModuleDefinition module, CilWorker IL)
        {
            var targetMethod = IL.GetMethod();

            // var classAroundInvoke = AroundInvokeRegistry.GetSurroundingImplementation(info);           
            GetSurroundingClassImplementation(module, IL);

            // classAroundInvoke.BeforeInvoke(info);
            EmitBeforeInvoke(IL, module, _surroundingClassImplementation);

            // if (surroundingImplementation != null) {
            if (targetMethod.HasThis)
                EmitBeforeInvoke(IL, module, _surroundingImplementation);
            // }
        }

        private void GetSurroundingClassImplementation(ModuleDefinition module, CilWorker IL)
        {
            var getSurroundingImplementationMethod =
                typeof(AroundInvokeRegistry).GetMethod("GetSurroundingImplementation");

            var getSurroundingImplementation = module.Import(getSurroundingImplementationMethod);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingClassImplementation);
        }

        private void EmitBeforeInvoke(CilWorker IL, ModuleDefinition module, VariableDefinition surroundingImplementation)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);

            IL.Append(skipInvoke);
        }

        private void GetSurroundingImplementationInstance(ModuleDefinition module, CilWorker IL, Instruction skipGetSurroundingImplementation)
        {
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingImplementation);
        }

        private void GetAroundInvokeProvider(MethodDefinition method, ModuleDefinition module, CilWorker IL)
        {
            // var aroundInvokeProvider = this.AroundInvokeProvider;
            _aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            var getAroundInvokeProvider = module.ImportMethod<IModifiableType>("get_AroundInvokeProvider");

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
                return;
            }

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
        }

        private void GetMethodReplacementProvider(MethodDefinition method, ModuleDefinition module, CilWorker IL)
        {
            _methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
                return;
            }

            var getProvider = module.ImportMethod<IMethodReplacementHost>("get_MethodReplacementProvider");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getProvider);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
        }

        protected virtual void AddOriginalInstructions(CilWorker IL, IEnumerable<Instruction> oldInstructions, Instruction endLabel)
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
