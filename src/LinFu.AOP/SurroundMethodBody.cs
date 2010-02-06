using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class SurroundMethodBody
    {
        private ModuleDefinition _module;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _aroundInvokeProvider;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _surroundingImplementation;
        private VariableDefinition _surroundingClassImplementation;
        private VariableDefinition _interceptionDisabled;
        private VariableDefinition _returnValue;

        public SurroundMethodBody(ModuleDefinition module, 
            VariableDefinition methodReplacementProvider, 
            VariableDefinition aroundInvokeProvider, 
            VariableDefinition invocationInfo,
            VariableDefinition interceptionDisabled, 
            VariableDefinition returnValue)
        {
            _module = module;
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _interceptionDisabled = interceptionDisabled;
            _returnValue = returnValue;
        }

        public void AddProlog(MethodDefinition method, CilWorker IL)
        {
            _surroundingImplementation = method.AddLocal<IAroundInvoke>();
            _surroundingClassImplementation = method.AddLocal<IAroundInvoke>();

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
            GetMethodReplacementProvider(method, module, IL, _methodReplacementProvider);
            GetAroundInvokeProvider(method, module, IL, _aroundInvokeProvider);

            // if (aroundInvokeProvider != null ) {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            GetSurroundingImplementationInstance(module, IL, skipGetSurroundingImplementation,
                _aroundInvokeProvider, _invocationInfo, _surroundingImplementation);

            // }

            IL.Append(skipGetSurroundingImplementation);

            EmitBeforeInvoke(module, IL, _surroundingClassImplementation, _surroundingImplementation, _invocationInfo);

            IL.Append(skipProlog);
        }

        public void AddEpilog(MethodDefinition method, CilWorker IL)
        {
            var skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            EmitAfterInvoke(IL, _module, _surroundingImplementation, _surroundingClassImplementation, _invocationInfo, _returnValue);

            // }
            IL.Append(skipEpilog);
        }

        private static void EmitBeforeInvoke(ModuleDefinition module, CilWorker IL,
            VariableDefinition surroundingClassImplementation,
            VariableDefinition surroundingImplementation, VariableDefinition invocationInfo)
        {
            var targetMethod = IL.GetMethod();

            // var classAroundInvoke = AroundInvokeRegistry.GetSurroundingImplementation(info);           
            GetSurroundingClassImplementation(module, IL, invocationInfo, surroundingClassImplementation);

            // classAroundInvoke.BeforeInvoke(info);
            EmitBeforeInvoke(IL, module, surroundingClassImplementation, invocationInfo);

            // if (surroundingImplementation != null) {
            if (targetMethod.HasThis)
                EmitBeforeInvoke(IL, module, surroundingImplementation, invocationInfo);
            // }
        }

        private static void GetSurroundingImplementationInstance(ModuleDefinition module,
    CilWorker IL, Instruction skipGetSurroundingImplementation,
    VariableDefinition aroundInvokeProvider, VariableDefinition invocationInfo, VariableDefinition surroundingImplementation)
        {
            IL.Emit(OpCodes.Ldloc, aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, surroundingImplementation);
        }

        private static void GetAroundInvokeProvider(MethodDefinition method, ModuleDefinition module,
            CilWorker IL, VariableDefinition aroundInvokeProvider)
        {
            // var aroundInvokeProvider = this.AroundInvokeProvider;
            var getAroundInvokeProvider = module.ImportMethod<IModifiableType>("get_AroundInvokeProvider");

            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, aroundInvokeProvider);
                return;
            }

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, aroundInvokeProvider);
        }

        private static void GetMethodReplacementProvider(MethodDefinition method,
            ModuleDefinition module, CilWorker IL, VariableDefinition methodReplacementProvider)
        {
            if (!method.HasThis)
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stloc, methodReplacementProvider);
                return;
            }

            var getProvider = module.ImportMethod<IMethodReplacementHost>("get_MethodReplacementProvider");
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, getProvider);
            IL.Emit(OpCodes.Stloc, methodReplacementProvider);
        }

        private static void GetSurroundingClassImplementation(ModuleDefinition module, CilWorker IL,
    VariableDefinition invocationInfo, VariableDefinition surroundingClassImplementation)
        {
            var getSurroundingImplementationMethod =
                typeof(AroundInvokeRegistry).GetMethod("GetSurroundingImplementation");

            var getSurroundingImplementation = module.Import(getSurroundingImplementationMethod);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Call, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, surroundingClassImplementation);
        }

        private static void EmitBeforeInvoke(CilWorker IL, ModuleDefinition module,
            VariableDefinition surroundingImplementation, VariableDefinition invocationInfo)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);

            IL.Append(skipInvoke);
        }

        private static void EmitAfterInvoke(CilWorker IL, ModuleDefinition module,
           VariableDefinition surroundingImplementation,
           VariableDefinition invocationInfo,
           VariableDefinition returnValue)
        {
            var skipInvoke = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipInvoke);

            var aroundInvoke = module.ImportMethod<IAfterInvoke>("AfterInvoke");
            IL.Emit(OpCodes.Ldloc, surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Ldloc, returnValue);
            IL.Emit(OpCodes.Callvirt, aroundInvoke);

            IL.Append(skipInvoke);
        }

        private static void EmitAfterInvoke(CilWorker IL, ModuleDefinition module,
            VariableDefinition surroundingImplementation,
            VariableDefinition surroundingClassImplementation,
            VariableDefinition invocationInfo,
            VariableDefinition returnValue)
        {
            // instanceAroundInvoke.AfterInvoke(info, returnValue);
            EmitAfterInvoke(IL, module, surroundingImplementation, invocationInfo, returnValue);

            // classAroundInvoke.AfterInvoke(info, returnValue);
            EmitAfterInvoke(IL, module, surroundingClassImplementation, invocationInfo, returnValue);
        }
    }
}
