using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that extends the <see cref="CilWorker"/> class
    /// with helper methods that make it easier to save
    /// information about the method currently being implemented.
    /// </summary>
    public static class CilWorkerExtensions
    {
        private static readonly Dictionary<string, OpCode> stindMap = new Dictionary<string, OpCode>();

        static CilWorkerExtensions()
        {
            stindMap["Bool&"] = OpCodes.Stind_I1;
            stindMap["Int8&"] = OpCodes.Stind_I1;
            stindMap["Uint8&"] = OpCodes.Stind_I1;

            stindMap["Int16&"] = OpCodes.Stind_I2;
            stindMap["Uint16&"] = OpCodes.Stind_I2;

            stindMap["Uint32&"] = OpCodes.Stind_I4;
            stindMap["Int32&"] = OpCodes.Stind_I4;

            stindMap["IntPtr"] = OpCodes.Stind_I4;
            stindMap["Uint64&"] = OpCodes.Stind_I8;
            stindMap["Int64&"] = OpCodes.Stind_I8;
            stindMap["Float32&"] = OpCodes.Stind_R4;
            stindMap["Float64&"] = OpCodes.Stind_R8;
        }
        /// <summary>
        /// Pushes the current <paramref name="method"/> onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="method">The method that represents the <see cref="MethodInfo"/> that will be pushed onto the stack.</param>
        /// <param name="module">The module that contains the host method.</param>
        public static void PushMethod(this CilWorker IL, MethodReference method, ModuleDefinition module)
        {
            var getMethodFromHandle = module.ImportMethod<MethodBase>("GetMethodFromHandle",
                typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle));

            var declaringType = method.DeclaringType;

            // Instantiate the generic type before determining
            // the current method
            if (declaringType.GenericParameters.Count > 0)
            {
                var genericType = new GenericInstanceType(declaringType);
                foreach (GenericParameter parameter in declaringType.GenericParameters)
                {
                    genericType.GenericArguments.Add(parameter);
                }

                declaringType = genericType;
            }


            IL.Emit(OpCodes.Ldtoken, method);
            IL.Emit(OpCodes.Ldtoken, declaringType);
            IL.Emit(OpCodes.Call, getMethodFromHandle);
        }
        /// <summary>
        /// Pushes the arguments of a method onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="method">The target method.</param>
        /// <param name="arguments">The <see cref="VariableDefinition">local variable</see> that will hold the array of arguments.</param>
        public static void PushArguments(this CilWorker IL, IMethodSignature method, ModuleDefinition module, VariableDefinition arguments)
        {
            var objectType = module.ImportType(typeof(object));
            int parameterCount = method.Parameters.Count;
            IL.Emit(OpCodes.Ldc_I4, parameterCount);
            IL.Emit(OpCodes.Newarr, objectType);
            IL.Emit(OpCodes.Stloc, arguments);

            if (parameterCount == 0)
                return;

            var index = 0;
            foreach (ParameterDefinition param in method.Parameters)
            {
                IL.PushParameter(index++, arguments, param);
            }
        }
        

        /// <summary>
        /// Pushes the stack trace of the currently executing method onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        public static void PushStackTrace(this CilWorker IL, ModuleDefinition module)
        {
            var stackTraceConstructor = typeof(StackTrace).GetConstructor(new Type[] { typeof(int), typeof(bool) });
            var stackTraceCtor = module.Import(stackTraceConstructor);

            var addDebugSymbols = OpCodes.Ldc_I4_0;
            IL.Emit(OpCodes.Ldc_I4_1);
            IL.Emit(addDebugSymbols);
            IL.Emit(OpCodes.Newobj, stackTraceCtor);
        }

        /// <summary>
        /// Saves the generic type arguments that were used to construct the method.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="method">The target method whose generic type arguments (if any) will be saved into the <paramref name="typeArguments">local variable</paramref>.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="typeArguments">The local variable that will store the resulting array of <see cref="Type"/> objects.</param>
        public static void PushGenericArguments(this CilWorker IL, IGenericParameterProvider method,
            ModuleDefinition module, VariableDefinition typeArguments)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            var genericParameterCount = method.GenericParameters.Count;

            var genericParameters = method.GenericParameters;
            for (var index = 0; index < genericParameterCount; index++)
            {
                var current = genericParameters[index];

                IL.Emit(OpCodes.Ldloc, typeArguments);
                IL.Emit(OpCodes.Ldc_I4, index);
                IL.Emit(OpCodes.Ldtoken, current);
                IL.Emit(OpCodes.Call, getTypeFromHandle);
                IL.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Saves the current method signature of a method into an array
        /// of <see cref="System.Type"/> objects. This can be used to determine the
        /// signature of methods with generic type parameters or methods that have
        /// parameters that are generic parameters specified by the type itself.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="method">The target method whose generic type arguments (if any) will be saved into the local variable .</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="parameterTypes">The local variable that will store the current method signature.</param>
        public static void SaveParameterTypes(this CilWorker IL, MethodDefinition method, ModuleDefinition module,
             VariableDefinition parameterTypes)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            var parameterCount = method.Parameters.Count;
            for (var index = 0; index < parameterCount; index++)
            {
                var current = method.Parameters[index];
                IL.Emit(OpCodes.Ldloc, parameterTypes);
                IL.Emit(OpCodes.Ldc_I4, index);
                IL.Emit(OpCodes.Ldtoken, current.ParameterType);
                IL.Emit(OpCodes.Call, getTypeFromHandle);
                IL.Emit(OpCodes.Stelem_Ref);
            }
        }

        /// <summary>
        /// Converts the return value of a method into the <paramref name="returnType">target type</paramref>.
        /// If the target type is void, then the value will simply be popped from the stack.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="returnType">The method return type itself.</param>
        public static void PackageReturnValue(this CilWorker IL, ModuleDefinition module, TypeReference returnType)
        {
            var voidType = module.ImportType(typeof(void));
            if (returnType == voidType)
            {
                IL.Emit(OpCodes.Pop);
                return;
            }

            IL.Emit(OpCodes.Unbox_Any, returnType);
        }

        /// <summary>
        /// Emits the proper Stind (store indirect) IL instruction for the <paramref name="currentType"/>.
        /// </summary>
        /// <param name="IL">The target <see cref="CilWorker"/> that will emit the IL.</param>
        /// <param name="currentType">The type of data being stored.</param>
        public static void Stind(this CilWorker IL, TypeReference currentType)
        {
            string typeName = currentType.Name;
            var opCode = OpCodes.Nop;
            if (!currentType.IsValueType && !typeName.EndsWith("&"))
                opCode = OpCodes.Stind_Ref;

            opCode = !stindMap.ContainsKey(typeName) ? OpCodes.Stind_Ref : stindMap[typeName];

            IL.Emit(opCode);
        }
        /// <summary>
        /// Stores the <paramref name="param">current parameter value</paramref>
        /// into the array of method <paramref name="arguments"/>.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will be used to create the instructions.</param>
        /// <param name="arguments">The local variable that will store the method arguments.</param>
        /// <param name="index">The array index that indicates where the parameter value will be stored in the array of arguments.</param>
        /// <param name="param">The current argument value being stored.</param>
        private static void PushParameter(this CilWorker IL, int index, VariableDefinition arguments, ParameterDefinition param)
        {            
            var parameterType = param.ParameterType;
            IL.Emit(OpCodes.Ldloc, arguments);
            IL.Emit(OpCodes.Ldc_I4, index);

            // Zero out the [out] parameters
            if (param.IsOut || param.IsByRef())
            {
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Stelem_Ref);
                return;
            }

            IL.Emit(OpCodes.Ldarg, param);

            if (parameterType.IsValueType || parameterType is GenericParameter)
                IL.Emit(OpCodes.Box, param.ParameterType);

            IL.Emit(OpCodes.Stelem_Ref);
        }
    }
}
