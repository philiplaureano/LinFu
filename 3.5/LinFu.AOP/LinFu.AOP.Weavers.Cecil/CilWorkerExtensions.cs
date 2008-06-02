using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.CecilExtensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Weavers.Cecil
{
    public static class CilWorkerExtensions
    {
        public static void PushMethod(this CilWorker IL, MethodDefinition method, ModuleDefinition module,
            Queue<Instruction> prolog)
        {
            MethodReference getMethodFromHandle = module.ImportMethod<MethodBase>("GetMethodFromHandle", typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle));
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
            prolog.Enqueue(IL.Create(OpCodes.Call, getMethodFromHandle));
        }
        public static void PushArguments(this CilWorker IL, ModuleDefinition module, IMethodSignature methodDef,
            VariableDefinition arguments, Queue<Instruction> prolog)
        {
            var objectType = module.ImportType(typeof(object));
            int parameterCount = methodDef.Parameters.Count;
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, parameterCount));
            prolog.Enqueue(IL.Create(OpCodes.Newarr, objectType));
            prolog.Enqueue(IL.Create(OpCodes.Stloc, arguments));

            if (parameterCount == 0)
                return;

            foreach (ParameterDefinition param in methodDef.Parameters)
            {
                int index = param.Sequence - 1;
                TypeReference parameterType = param.ParameterType;
                prolog.Enqueue(IL.Create(OpCodes.Ldloc, arguments));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldarg, param));

                if (parameterType.IsValueType || parameterType is GenericParameter)
                    prolog.Enqueue(IL.Create(OpCodes.Box, param.ParameterType));

                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }
        public static void PushInstance(this CilWorker IL, MethodDefinition method, Queue<Instruction> prolog)
        {
            OpCode opCode = method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0;
            prolog.Enqueue(IL.Create(opCode));
        }

        public static void PushStackTrace(this CilWorker IL, Queue<Instruction> prolog,
            ModuleDefinition module)
        {
            ConstructorInfo stackTraceConstructor = typeof(StackTrace).GetConstructor(new Type[] { typeof(int), typeof(bool) });
            Debug.Assert(stackTraceConstructor != null);

            MethodReference stackTraceCtor = module.Import(stackTraceConstructor);

            OpCode addDebugSymbols = OpCodes.Ldc_I4_0;
            prolog.Enqueue(IL.Create(OpCodes.Ldc_I4_1));
            prolog.Enqueue(IL.Create(addDebugSymbols));
            prolog.Enqueue(IL.Create(OpCodes.Newobj, stackTraceCtor));
        }

        public static void PushGenericArguments(this CilWorker IL, IGenericParameterProvider methodDef,
            ModuleDefinition module, Queue<Instruction> prolog, VariableDefinition typeArguments,
            int genericParameterCount)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);

            GenericParameterCollection genericParameters = methodDef.GenericParameters;
            for (int index = 0; index < genericParameterCount; index++)
            {
                GenericParameter current = genericParameters[index];

                prolog.Enqueue(IL.Create(OpCodes.Ldloc, typeArguments));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldtoken, current));
                prolog.Enqueue(IL.Create(OpCodes.Call, getTypeFromHandle));
                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }
        public static void SaveParameterTypes(this CilWorker IL, MethodDefinition methodDef, ModuleDefinition module,
             VariableDefinition parameterTypes, Queue<Instruction> prolog)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            int parameterCount = methodDef.Parameters.Count;
            for (int index = 0; index < parameterCount; index++)
            {
                ParameterDefinition current = methodDef.Parameters[index];
                prolog.Enqueue(IL.Create(OpCodes.Ldloc, parameterTypes));
                prolog.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                prolog.Enqueue(IL.Create(OpCodes.Ldtoken, current.ParameterType));
                prolog.Enqueue(IL.Create(OpCodes.Call, getTypeFromHandle));
                prolog.Enqueue(IL.Create(OpCodes.Stelem_Ref));
            }
        }

        public static void PackageReturnValue(this CilWorker IL, ModuleDefinition module, Queue<Instruction> prolog, TypeReference returnType)
        {
            var voidType = module.ImportType(typeof(void));
            if (returnType == voidType)
            {
                prolog.Enqueue(IL.Create(OpCodes.Pop));
                return;
            }

            prolog.Enqueue(IL.Create(OpCodes.Unbox_Any, returnType));
        }
    }
}
