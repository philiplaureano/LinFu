﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace LinFu.AOP.Cecil
{
    internal class InterceptMethodCalls : InstructionSwapper
    {
        private readonly IMethodCallFilter _callFilter;
        private VariableDefinition _aroundInvokeProvider;
        private TypeReference _hostInterfaceType;
        private VariableDefinition _interceptionDisabled;
        private MethodReference _invocationInfoCtor;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _returnValue;
        private MethodReference _stackCtor;

        #region Method References

        private MethodReference _canReplace;
        private MethodReference _getProvider;
        private MethodReference _getReplacement;
        private MethodReference _getStaticProvider;
        private MethodReference _intercept;
        private MethodReference _popMethod;
        private MethodReference _pushMethod;
        private MethodReference _toArray;

        #endregion

        #region Local Variables

        private VariableDefinition _canReplaceFlag;
        private VariableDefinition _currentArgument;
        private VariableDefinition _currentArguments;
        private VariableDefinition _instanceProvider;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _parameterTypes;
        private VariableDefinition _replacement;
        private VariableDefinition _staticProvider;
        private VariableDefinition _target;
        private VariableDefinition _typeArguments;

        #endregion

        public InterceptMethodCalls(Func<MethodReference, bool> hostMethodFilter,
                                    Func<MethodReference, bool> methodCallFilter)
        {
            _callFilter = new MethodCallFilterAdapter(hostMethodFilter, methodCallFilter);
        }

        public InterceptMethodCalls(IMethodCallFilter callFilter)
        {
            _callFilter = callFilter;
        }

        public override void ImportReferences(ModuleDefinition module)
        {
            var types = new[]
                            {
                                typeof (object),
                                typeof (MethodBase),
                                typeof (StackTrace),
                                typeof (Type[]),
                                typeof (Type[]),
                                typeof (Type),
                                typeof (object[])
                            };

            _invocationInfoCtor = module.ImportConstructor<InvocationInfo>(types);
            _stackCtor = module.ImportConstructor<Stack<object>>(new Type[0]);

            _pushMethod = module.ImportMethod<Stack<object>>("Push");
            _popMethod = module.ImportMethod<Stack<object>>("Pop");
            _toArray = module.ImportMethod<Stack<object>>("ToArray");
            _getProvider = module.ImportMethod<IMethodReplacementHost>("get_MethodCallReplacementProvider");
            _getStaticProvider = module.ImportMethod("GetProvider", typeof (MethodCallReplacementProviderRegistry));

            _canReplace = module.ImportMethod<IMethodReplacementProvider>("CanReplace");
            _getReplacement = module.ImportMethod<IMethodReplacementProvider>("GetMethodReplacement");
            _hostInterfaceType = module.ImportType<IMethodReplacementHost>();
            _intercept = module.ImportMethod<IInterceptor>("Intercept");
        }

        public override void AddLocals(MethodDefinition hostMethod)
        {
            MethodBody body = hostMethod.Body;
            body.InitLocals = true;

            _currentArguments = hostMethod.AddLocal<Stack<object>>("__arguments");
            _currentArgument = hostMethod.AddLocal<object>("__currentArgument");
            _parameterTypes = hostMethod.AddLocal<Type[]>("__parameterTypes");
            _typeArguments = hostMethod.AddLocal<Type[]>("__typeArguments");
            _invocationInfo = hostMethod.AddLocal<IInvocationInfo>("___invocationInfo");

            _target = hostMethod.AddLocal<object>("__target");
            _replacement = hostMethod.AddLocal<IInterceptor>("__interceptor");
            _canReplaceFlag = hostMethod.AddLocal<bool>("__canReplace");

            _staticProvider = hostMethod.AddLocal<IMethodReplacementProvider>("__staticProvider");
            _instanceProvider = hostMethod.AddLocal<IMethodReplacementProvider>("__instanceProvider");
            _interceptionDisabled = hostMethod.AddLocal<bool>();

            _methodReplacementProvider = hostMethod.AddLocal<IMethodReplacementProvider>();
            _aroundInvokeProvider = hostMethod.AddLocal<IAroundInvokeProvider>();
            _returnValue = hostMethod.AddLocal<object>();
        }

        protected override void Replace(Instruction oldInstruction, MethodDefinition hostMethod,
                                        ILProcessor IL)
        {
            var targetMethod = (MethodReference) oldInstruction.Operand;

            Instruction callOriginalMethod = IL.Create(OpCodes.Nop);
            TypeReference returnType = targetMethod.ReturnType;
            Instruction endLabel = IL.Create(OpCodes.Nop);
            ModuleDefinition module = hostMethod.DeclaringType.Module;

            // Create the stack that will hold the method arguments
            IL.Emit(OpCodes.Newobj, _stackCtor);
            IL.Emit(OpCodes.Stloc, _currentArguments);

            // Make sure that the argument stack doesn't show up in
            // any of the other interception routines
            IgnoreLocal(IL, _currentArguments, module);

            SaveInvocationInfo(IL, targetMethod, module, returnType);

            var getInterceptionDisabled = new GetInterceptionDisabled(hostMethod, _interceptionDisabled);
            getInterceptionDisabled.Emit(IL);

            var surroundMethodBody = new SurroundMethodBody(_methodReplacementProvider, _aroundInvokeProvider,
                                                            _invocationInfo, _interceptionDisabled, _returnValue,
                                                            typeof (AroundInvokeMethodCallRegistry),
                                                            "AroundMethodCallProvider");

            surroundMethodBody.AddProlog(IL);
            // Use the MethodReplacementProvider attached to the
            // current host instance
            Replace(IL, oldInstruction, targetMethod, hostMethod, endLabel, callOriginalMethod);

            IL.Append(endLabel);

            surroundMethodBody.AddEpilog(IL);
        }

        private void IgnoreLocal(ILProcessor IL, VariableDefinition targetVariable, ModuleDefinition module)
        {
            IL.Emit(OpCodes.Ldloc, targetVariable);

            MethodReference addInstance = module.Import(typeof (IgnoredInstancesRegistry).GetMethod("AddInstance"));
            IL.Emit(OpCodes.Call, addInstance);
        }

        private void Replace(ILProcessor IL, Instruction oldInstruction, MethodReference targetMethod,
                             MethodDefinition hostMethod, Instruction endLabel, Instruction callOriginalMethod)
        {
            TypeReference returnType = targetMethod.ReturnType;
            ModuleDefinition module = hostMethod.DeclaringType.Module;
            if (!hostMethod.IsStatic)
                GetInstanceProvider(IL);


            Instruction pushInstance = hostMethod.HasThis ? IL.Create(OpCodes.Ldarg_0) : IL.Create(OpCodes.Ldnull);

            // If all else fails, use the static method replacement provider
            IL.Append(pushInstance);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Call, _getStaticProvider);
            IL.Emit(OpCodes.Stloc, _staticProvider);

            Instruction restoreArgumentStack = IL.Create(OpCodes.Nop);

            Instruction callReplacement = IL.Create(OpCodes.Nop);
            Instruction useStaticProvider = IL.Create(OpCodes.Nop);

            #region Use the instance method replacement provider

            IL.Emit(OpCodes.Ldloc, _instanceProvider);
            IL.Emit(OpCodes.Brfalse, useStaticProvider);


            EmitCanReplace(IL, hostMethod, _instanceProvider);
            IL.Emit(OpCodes.Ldloc, _canReplaceFlag);
            IL.Emit(OpCodes.Brfalse, useStaticProvider);

            EmitGetMethodReplacement(IL, hostMethod, _instanceProvider);


            IL.Emit(OpCodes.Ldloc, _replacement);
            IL.Emit(OpCodes.Brtrue, callReplacement);

            #endregion

            IL.Append(useStaticProvider);
            // if (!MethodReplacementProvider.CanReplace(info))
            //      CallOriginalMethod();
            EmitCanReplace(IL, hostMethod, _staticProvider);

            IL.Emit(OpCodes.Ldloc, _canReplaceFlag);
            IL.Emit(OpCodes.Brfalse, restoreArgumentStack);

            EmitGetMethodReplacement(IL, hostMethod, _staticProvider);

            IL.Append(callReplacement);

            // if (replacement == null)
            //      CallOriginalMethod();
            IL.Emit(OpCodes.Ldloc, _replacement);
            IL.Emit(OpCodes.Brfalse, restoreArgumentStack);

            EmitInterceptorCall(IL);

            IL.PackageReturnValue(module, returnType);

            IL.Emit(OpCodes.Br, endLabel);

            IL.Append(restoreArgumentStack);

            // Reconstruct the method arguments if the interceptor
            // cannot be found

            // Push the target instance
            ReconstructMethodArguments(IL, targetMethod);

            // Mark the CallOriginalMethod instruction label
            IL.Append(callOriginalMethod);

            // Call the original method
            IL.Append(oldInstruction);
        }

        private void GetInstanceProvider(ILProcessor IL)
        {
            Instruction skipInstanceProvider = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, _hostInterfaceType);
            IL.Emit(OpCodes.Brfalse, skipInstanceProvider);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, _hostInterfaceType);
            IL.Emit(OpCodes.Callvirt, _getProvider);
            IL.Emit(OpCodes.Stloc, _instanceProvider);

            IL.Emit(OpCodes.Ldloc, _instanceProvider);
            IL.Emit(OpCodes.Brtrue, skipInstanceProvider);

            IL.Append(skipInstanceProvider);
        }

        private void ReconstructMethodArguments(ILProcessor IL, MethodReference targetMethod)
        {
            if (targetMethod.HasThis)
                IL.Emit(OpCodes.Ldloc, _target);

            // Push the arguments back onto the stack
            foreach (ParameterReference param in targetMethod.Parameters)
            {
                IL.Emit(OpCodes.Ldloc, _currentArguments);
                IL.Emit(OpCodes.Callvirt, _popMethod);
                IL.Emit(OpCodes.Unbox_Any, param.ParameterType);
            }
        }

        private void SaveInvocationInfo(ILProcessor IL, MethodReference targetMethod, ModuleDefinition module,
                                        TypeReference returnType)
        {
            // If the target method is an instance method, then the remaining item on the stack
            // will be the target object instance


            // Put all the method arguments into the argument stack
            foreach (ParameterReference param in targetMethod.Parameters)
            {
                // Save the current argument
                TypeReference parameterType = param.ParameterType;
                if (parameterType.IsValueType || parameterType is GenericParameter)
                    IL.Emit(OpCodes.Box, parameterType);

                IL.Emit(OpCodes.Stloc, _currentArgument);
                IL.Emit(OpCodes.Ldloc, _currentArguments);
                IL.Emit(OpCodes.Ldloc, _currentArgument);
                IL.Emit(OpCodes.Callvirt, _pushMethod);
            }


            // Static methods will always have a null reference as the target
            if (!targetMethod.HasThis)
                IL.Emit(OpCodes.Ldnull);

            // Box the target, if necessary
            TypeReference declaringType = targetMethod.GetDeclaringType();
            if (targetMethod.HasThis && (declaringType.IsValueType || declaringType is GenericParameter))
                IL.Emit(OpCodes.Box, declaringType);

            // Save the target
            IL.Emit(OpCodes.Stloc, _target);
            IL.Emit(OpCodes.Ldloc, _target);

            // Push the current method
            IL.PushMethod(targetMethod, module);

            // Push the stack trace
            PushStackTrace(IL, module);

            TypeReference systemType = module.Import(typeof (Type));

            // Save the parameter types
            int parameterCount = targetMethod.Parameters.Count;
            IL.Emit(OpCodes.Ldc_I4, parameterCount);
            IL.Emit(OpCodes.Newarr, systemType);
            IL.Emit(OpCodes.Stloc, _parameterTypes);

            IL.SaveParameterTypes(targetMethod, module, _parameterTypes);
            IL.Emit(OpCodes.Ldloc, _parameterTypes);

            // Save the type arguments
            int genericParameterCount = targetMethod.GenericParameters.Count;
            IL.Emit(OpCodes.Ldc_I4, genericParameterCount);
            IL.Emit(OpCodes.Newarr, systemType);
            IL.Emit(OpCodes.Stloc, _typeArguments);
            IL.PushGenericArguments(targetMethod, module, _typeArguments);
            IL.Emit(OpCodes.Ldloc, _typeArguments);

            // Push the return type
            IL.PushType(returnType, module);

            // Save the method arguments
            IL.Emit(OpCodes.Ldloc, _currentArguments);
            IL.Emit(OpCodes.Callvirt, _toArray);

            IL.Emit(OpCodes.Newobj, _invocationInfoCtor);
            IL.Emit(OpCodes.Stloc, _invocationInfo);

            IgnoreLocal(IL, _invocationInfo, module);
        }

        private void PushStackTrace(ILProcessor IL, ModuleDefinition module)
        {
            IL.PushStackTrace(module);
        }

        private void EmitInterceptorCall(ILProcessor IL)
        {
            // var result = replacement.Intercept(info);
            IL.Emit(OpCodes.Ldloc, _replacement);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, _intercept);
        }

        private void EmitCanReplace(ILProcessor IL, IMethodSignature hostMethod, VariableDefinition provider)
        {
            Instruction skipGetProvider = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, provider);
            IL.Emit(OpCodes.Brfalse, skipGetProvider);

            IL.Emit(OpCodes.Ldloc, provider);

            // Push the host instance
            Instruction pushInstance = hostMethod.HasThis ? IL.Create(OpCodes.Ldarg_0) : IL.Create(OpCodes.Ldnull);
            IL.Append(pushInstance);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, _canReplace);

            IL.Emit(OpCodes.Stloc, _canReplaceFlag);
            IL.Append(skipGetProvider);
        }

        private void EmitGetMethodReplacement(ILProcessor IL, IMethodSignature hostMethod, VariableDefinition provider)
        {
            // var replacement = MethodReplacementProvider.GetReplacement(info);
            IL.Emit(OpCodes.Ldloc, provider);

            // Push the host instance
            Instruction pushInstance = hostMethod.HasThis ? IL.Create(OpCodes.Ldarg_0) : IL.Create(OpCodes.Ldnull);
            IL.Append(pushInstance);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, _getReplacement);
            IL.Emit(OpCodes.Stloc, _replacement);
        }

        protected override bool ShouldReplace(Instruction oldInstruction, MethodDefinition hostMethod)
        {
            // Intercept the call and callvirt instructions
            OpCode opCode = oldInstruction.OpCode;
            if (opCode != OpCodes.Callvirt && opCode != OpCodes.Call)
                return false;

            var targetMethod = (MethodReference) oldInstruction.Operand;
            TypeReference declaringType = targetMethod.DeclaringType;


            //return _hostMethodFilter(hostMethod) && _methodCallFilter(targetMethod);
            return _callFilter.ShouldWeave(hostMethod.DeclaringType, hostMethod, targetMethod);
        }
    }
}