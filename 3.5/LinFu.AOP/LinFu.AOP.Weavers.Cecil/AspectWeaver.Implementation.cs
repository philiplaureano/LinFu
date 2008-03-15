using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Reflection;
using LinFu.AOP.Interfaces;
using LinFu.AOP.CecilExtensions;
using System.Diagnostics;

namespace LinFu.AOP.Weavers.Cecil
{
    public partial class AspectWeaver
    {
        private TypeReference _methodReplacementRegistry;
        private TypeReference _systemType;
        private TypeReference _methodInfoType;
        private TypeReference _voidType;
        private TypeReference _objectType;
        private TypeReference _modifiableType;
        private TypeReference _aroundInvokeType;

        private MethodReference _getClassMethodReplacementProvider;
        private MethodReference _getMethodReplacementProvider;
        private MethodReference _getTypeFromHandle;
        private MethodReference _getMethodFromHandle;
        private MethodReference _getAroundInvokeProvider;
        private MethodReference _contextCtor;
        private MethodReference _getSurroundingImplementation;
        private MethodReference _getInstanceBasedSurroundingImplementation;
        private MethodReference _getMethodReplacement;
        private MethodReference _beforeInvoke;
        private MethodReference _afterInvoke;
        private MethodReference _invokeReplacement;
        private MethodReference _isEnabled;
        private MethodReference _canReplace;

        private VariableDefinition _parameterTypes;
        private VariableDefinition _typeArguments;
        private VariableDefinition _currentMethod;
        private VariableDefinition _arguments;
        private VariableDefinition _context;
        private VariableDefinition _methodReplacement;
        private VariableDefinition _aroundInvoke;
        private VariableDefinition _returnValue;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _aroundInvokeArray;
        private VariableDefinition _aroundInvokeProvider;

        private void BuildInvocationContext(MethodDefinition methodDef, ModuleDefinition module, Queue<Instruction> prolog, CilWorker IL)
        {
            #region Initialize the InvocationInfo constructor arguments
            // Type[] typeArguments = new Type[genericTypeCount];
            int genericParameterCount = methodDef.GenericParameters.Count;
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, genericParameterCount));
            prolog.Enqueue(IL.Create(OpCodes.Newarr, _systemType));
            prolog.Enqueue(IL.Create(OpCodes.Stloc, _typeArguments));

            // Push the generic type arguments onto the stack
            if (genericParameterCount > 0)
                PushGenericArguments(methodDef, IL, prolog, genericParameterCount);

            // object[] arguments = new object[argumentCount];            
            PushArguments(methodDef, prolog, IL);

            // object target = this;
            PushInstance(methodDef, prolog, IL);

            PushCurrentMethod(methodDef, prolog, IL);

            prolog.Enqueue(IL.Create(OpCodes.Stloc, _currentMethod));

            // MethodInfo targetMethod = currentMethod as MethodInfo;
            prolog.Enqueue(IL.Create(OpCodes.Ldloc, _currentMethod));
            prolog.Enqueue(IL.Create(OpCodes.Isinst, _methodInfoType));

            // Get the current stack trace
            PushStackTrace(prolog, IL, module);

            // Push the type arguments back onto the stack
            prolog.Enqueue(IL.Create(OpCodes.Ldloc, _typeArguments));

            // Save the parameter types
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, methodDef.Parameters.Count));
            prolog.Enqueue(IL.Create(OpCodes.Newarr, _systemType));
            prolog.Enqueue(IL.Create(OpCodes.Stloc, _parameterTypes));
            SaveParameterTypes(methodDef, IL, prolog);
            prolog.Enqueue(IL.Create(OpCodes.Ldloc, _parameterTypes));

            // Save the return type
            TypeReference returnType = methodDef.ReturnType.ReturnType;
            prolog.Enqueue(IL.Create(OpCodes.Ldtoken, returnType));
            prolog.Enqueue(IL.Create(OpCodes.Call, _getTypeFromHandle));

            // Push the arguments back onto the stack
            prolog.Enqueue(IL.Create(OpCodes.Ldloc, _arguments));
            #endregion


            prolog.Enqueue(IL.Create(OpCodes.Newobj, _contextCtor));
            prolog.Enqueue(IL.Create(OpCodes.Stloc, _context));
        }
        private void PushCurrentMethod(MethodDefinition method, Queue<Instruction> prolog, CilWorker IL)
        {
            TypeReference declaringType = method.DeclaringType;

            // Instantiate the generic type before determining
            // the current method
            if (declaringType.GenericParameters.Count > 0)
            {
                GenericInstanceType genericType = new GenericInstanceType(declaringType);
                foreach (GenericParameter parameter in declaringType.GenericParameters)
                {
                    genericType.GenericArguments.Add(parameter);
                }

                declaringType = genericType;
            }

            prolog.Enqueue(IL.Create(OpCodes.Ldtoken, method));
            prolog.Enqueue(IL.Create(OpCodes.Ldtoken, declaringType));
            prolog.Enqueue(IL.Create(OpCodes.Call, _getMethodFromHandle));
        }
        private static void PushStackTrace(Queue<Instruction> prolog, CilWorker IL, ModuleDefinition module)
        {
            ConstructorInfo stackTraceConstructor = typeof(StackTrace).GetConstructor(new Type[] { typeof(int), typeof(bool) });
            Debug.Assert(stackTraceConstructor != null);

            MethodReference stackTraceCtor = module.Import(stackTraceConstructor);

            OpCode addDebugSymbols = OpCodes.Ldc_I4_0;
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4_1));
            prolog.Enqueue(IL.Create(addDebugSymbols));
            prolog.Enqueue(IL.Create(OpCodes.Newobj, stackTraceCtor));
        }
        private static void PushInstance(MethodDefinition method, Queue<Instruction> prolog, CilWorker IL)
        {
            OpCode opCode = method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0;
            prolog.Enqueue(IL.Create(opCode));
        }
        private void SaveParameterTypes(MethodDefinition methodDef, CilWorker IL, Queue<Instruction> prolog)
        {
            int parameterCount = methodDef.Parameters.Count;
            for (int index = 0; index < parameterCount; index++)
            {
                ParameterDefinition current = methodDef.Parameters[index];
                prolog.Enqueue(IL.Create(OpCodes.Ldloc, _parameterTypes));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldtoken, current.ParameterType));
                prolog.Enqueue(IL.Create(OpCodes.Call, _getTypeFromHandle));
                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }
        private void PushGenericArguments(IGenericParameterProvider methodDef, CilWorker IL, Queue<Instruction> prolog, int genericParameterCount)
        {
            GenericParameterCollection genericParameters = methodDef.GenericParameters;

            for (int index = 0; index < genericParameterCount; index++)
            {
                GenericParameter current = genericParameters[index];

                prolog.Enqueue(IL.Create(OpCodes.Ldloc, _typeArguments));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldtoken, current));
                prolog.Enqueue(IL.Create(OpCodes.Call, _getTypeFromHandle));
                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }
        private void PackageReturnType(CilWorker IL, Queue<Instruction> prolog, TypeReference returnType)
        {
            if (returnType == _voidType)
            {
                prolog.Enqueue(IL.Create(OpCodes.Pop));
                return;
            }

            prolog.Enqueue(IL.Create(OpCodes.Unbox_Any, returnType));
        }
        private void PushArguments(IMethodSignature methodDef, Queue<Instruction> prolog, CilWorker IL)
        {
            int parameterCount = methodDef.Parameters.Count;
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, parameterCount));
            prolog.Enqueue(IL.Create(OpCodes.Newarr, _objectType));
            prolog.Enqueue(IL.Create(OpCodes.Stloc, _arguments));


            if (parameterCount == 0)
                return;

            foreach (ParameterDefinition param in methodDef.Parameters)
            {
                int index = param.Sequence - 1;
                TypeReference parameterType = param.ParameterType;
                prolog.Enqueue(IL.Create(OpCodes.Ldloc, _arguments));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldarg, param));

                if (parameterType.IsValueType || parameterType is GenericParameter)
                    prolog.Enqueue(IL.Create(OpCodes.Box, param.ParameterType));

                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }
        public override void ImportReferences(ModuleDefinition module)
        {
            #region Type References
            _methodReplacementRegistry = module.Import(typeof(MethodReplacementRegistry));
            _systemType = module.Import(typeof(Type));
            _methodInfoType = module.Import(typeof(MethodInfo));
            _voidType = module.Import(typeof(void));
            _objectType = module.Import(typeof(object));
            _modifiableType = module.Import(typeof(IModifiableType));
            _aroundInvokeType = module.Import(typeof(IAroundInvoke));
            #endregion
            #region Method References
            _getClassMethodReplacementProvider = module.ImportMethod("GetProvider", typeof(MethodReplacementRegistry));
            _getMethodReplacement = module.ImportMethod<IMethodReplacementProvider>("GetMethodReplacement");
            _getMethodReplacementProvider = module.ImportMethod<IModifiableType>("get_MethodReplacementProvider");

            _getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            _getMethodFromHandle = module.ImportMethod<MethodBase>("GetMethodFromHandle", typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle));

            _getSurroundingImplementation = module.ImportMethod("GetSurroundingImplementation", typeof(AroundInvokeRegistry), BindingFlags.Public | BindingFlags.Static);

            _getInstanceBasedSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            _getAroundInvokeProvider = module.ImportMethod <IModifiableType>("get_AroundInvokeProvider");
            _beforeInvoke = module.ImportMethod<IAroundInvoke>("BeforeInvoke", BindingFlags.Public | BindingFlags.Instance);
            _afterInvoke = module.ImportMethod<IAroundInvoke>("AfterInvoke", BindingFlags.Public | BindingFlags.Instance);

            _invokeReplacement = module.ImportMethod<IMethodReplacement>("Invoke", BindingFlags.Public | BindingFlags.Instance);
            _isEnabled = module.Import(typeof(IModifiableType).GetMethod("get_IsInterceptionEnabled", BindingFlags.Public | BindingFlags.Instance));

            Type[] types = new Type[] { typeof(object), 
                                        typeof(MethodInfo), 
                                        typeof(StackTrace), 
                                        typeof(Type[]), 
                                        typeof(Type[]), 
                                        typeof(Type), 
                                        typeof(object[]) };

            // InvocationContext context = new InvocationContext(...);
            _contextCtor = module.ImportConstructor<InvocationContext>(types);
            _canReplace = module.Import(typeof(IMethodReplacementProvider).GetMethod("CanReplace", BindingFlags.Public | BindingFlags.Instance));
            #endregion
        }
        private void GetAroundInvoke(MethodDefinition method, Queue<Instruction> instructions, CilWorker IL)
        {
            int arraySize = 3;
            instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, arraySize));
            instructions.Enqueue(IL.Create(OpCodes.Newarr, _aroundInvokeType));
            instructions.Enqueue(IL.Create(OpCodes.Stloc, _aroundInvokeArray));

            // Get the class-based IAroundInvoke instance
            int classBasedIndex = 0;
            int instanceBasedIndex = 1;
            int providerIndex = 2;

            // aroundBehaviors[classBasedIndex] = AroundInvokeRegistry.GetSurroundingImplementation(context);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeArray));
            instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, classBasedIndex));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Call, _getSurroundingImplementation));
            instructions.Enqueue(IL.Create(OpCodes.Stelem_Ref));

            if (!method.IsStatic)
            {
                Instruction skipInstanceBasedAroundInvoke = IL.Create(OpCodes.Nop);

                PushInstance(method, instructions, IL);
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInstanceBasedAroundInvoke));

                // aroundInvokeProvider = this.AroundInvokeProvider;
                PushInstance(method, instructions, IL);
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _getAroundInvokeProvider));
                instructions.Enqueue(IL.Create(OpCodes.Stloc, _aroundInvokeProvider));

                // if (aroundInvokeProvider != null) {
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeProvider));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInstanceBasedAroundInvoke));

                // aroundBehaviors[instanceBasedIndex] = 
                //          aroundInvokeProvider.GetSurroundingImplementation(context);
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeArray));
                instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, instanceBasedIndex));
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeProvider));
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _getInstanceBasedSurroundingImplementation));
                instructions.Enqueue(IL.Create(OpCodes.Stelem_Ref));
                // }
                instructions.Enqueue(skipInstanceBasedAroundInvoke);
            }

            // HACK: Notify the method replacement provider
            // before and after the replacement executes to prevent 
            // every method replacement from infinitely looping on
            // itself

            // this is equivalent to: 
            // aroundBehaviors[providerIndex] = methodReplacementProvider as IAroundInvoke;
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeArray));
            instructions.Enqueue(IL.Create(OpCodes.Ldc_I4, providerIndex));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
            instructions.Enqueue(IL.Create(OpCodes.Isinst, _aroundInvokeType));
            instructions.Enqueue(IL.Create(OpCodes.Stelem_Ref));


            ModuleDefinition module = method.DeclaringType.Module;
            TypeReference enumerableType = module.Import(typeof(IEnumerable<IAroundInvoke>));

            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _aroundInvokeArray));
            instructions.Enqueue(IL.Create(OpCodes.Castclass, enumerableType));

            ConstructorInfo compositeCtorInfo = typeof(CompositeAroundInvoke)
                .GetConstructor(new Type[] { typeof(IEnumerable<IAroundInvoke>) });

            MethodReference compositeConstructor = module.Import(compositeCtorInfo);

            // aroundInvoke = new CompositeAopHost(aroundBehaviors);
            instructions.Enqueue(IL.Create(OpCodes.Newobj, compositeConstructor));
            instructions.Enqueue(IL.Create(OpCodes.Castclass, _aroundInvokeType));

            instructions.Enqueue(IL.Create(OpCodes.Stloc, _aroundInvoke));
        }

        private void GetMethodReplacement(MethodDefinition method, Queue<Instruction> instructions, CilWorker IL)
        {
            Instruction skipClassLevelMethodReplacement = IL.Create(OpCodes.Nop);
            if (!method.IsStatic)
            {
                // Get the method replacement provider attached to the current instance
                Instruction skipInstanceReplacement = IL.Create(OpCodes.Nop);
                PushInstance(method, instructions, IL);
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInstanceReplacement));

                // IModifiableType type = this as IModifiableType;
                // if (type != null) {
                PushInstance(method, instructions, IL);
                instructions.Enqueue(IL.Create(OpCodes.Isinst, _modifiableType));

                //     IMethodReplacementProvider provider = type.MethodReplacementProvider;
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _getMethodReplacementProvider));
                instructions.Enqueue(IL.Create(OpCodes.Stloc, _methodReplacementProvider));
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInstanceReplacement));

                //     if (provider.CanReplace(context)) {
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _canReplace));
                instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInstanceReplacement));

                //         methodReplacement = provider.GetMethodReplacement(context);
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
                instructions.Enqueue(IL.Create(OpCodes.Callvirt, _getMethodReplacement));
                instructions.Enqueue(IL.Create(OpCodes.Stloc, _methodReplacement));

                // Ignore the class-level method replacement if there is an 
                // instance-level replacement available 
                instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacement));
                instructions.Enqueue(IL.Create(OpCodes.Brtrue, skipClassLevelMethodReplacement));
                //      }
                // }
                instructions.Enqueue(skipInstanceReplacement);
            }

            // Get the class-level method replacement provider

            // IMethodReplacementProvider provider = MethodReplacementRegistry.GetProvider(context);
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Call, _getClassMethodReplacementProvider));
            instructions.Enqueue(IL.Create(OpCodes.Stloc, _methodReplacementProvider));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
            instructions.Enqueue(IL.Create(OpCodes.Brfalse, skipClassLevelMethodReplacement));

            // if (provider != null) {         
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodReplacementProvider));
            instructions.Enqueue(IL.Create(OpCodes.Ldloc, _context));
            instructions.Enqueue(IL.Create(OpCodes.Callvirt, _getMethodReplacement));

            //    methodReplacement = provier.GetMethodReplacement(context);
            instructions.Enqueue(IL.Create(OpCodes.Stloc, _methodReplacement));

            // }
            instructions.Enqueue(skipClassLevelMethodReplacement);
        }

    }
}
