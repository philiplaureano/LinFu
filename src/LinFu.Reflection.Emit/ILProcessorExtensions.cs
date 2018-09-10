using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// Represents a set of helper methods that make it easier to work with IL in Cecil
    /// </summary>
   public static class ILProcessorExtensions
    {
        private static readonly Dictionary<string, OpCode> StindMap = new Dictionary<string, OpCode>();

        static ILProcessorExtensions()
        {
            StindMap["Bool&"] = OpCodes.Stind_I1;
            StindMap["Int8&"] = OpCodes.Stind_I1;
            StindMap["Uint8&"] = OpCodes.Stind_I1;

            StindMap["Int16&"] = OpCodes.Stind_I2;
            StindMap["Uint16&"] = OpCodes.Stind_I2;

            StindMap["Uint32&"] = OpCodes.Stind_I4;
            StindMap["Int32&"] = OpCodes.Stind_I4;

            StindMap["IntPtr"] = OpCodes.Stind_I4;
            StindMap["Uint64&"] = OpCodes.Stind_I8;
            StindMap["Int64&"] = OpCodes.Stind_I8;
            StindMap["Float32&"] = OpCodes.Stind_R4;
            StindMap["Float64&"] = OpCodes.Stind_R8;
        }
        
        /// <summary>
        ///     Emits the proper Stind (store indirect) IL instruction for the <paramref name="currentType" />.
        /// </summary>
        /// <param name="IL">The target IL Processor that will emit the IL.</param>
        /// <param name="currentType">The type of data being stored.</param>
        public static void Stind(this ILProcessor IL, TypeReference currentType)
        {
            var typeName = currentType.Name;
            var opCode = OpCodes.Nop;
            if (!currentType.IsValueType && !typeName.EndsWith("&"))
                opCode = OpCodes.Stind_Ref;

            opCode = !StindMap.ContainsKey(typeName) ? OpCodes.Stind_Ref : StindMap[typeName];

            IL.Emit(opCode);
        }
        
        /// <summary>
        ///     Pushes the stack trace of the currently executing method onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="framesToSkip">The number of stack frames to skip.</param>
        public static void PushStackTrace(this ILProcessor IL, ModuleDefinition module, int framesToSkip = 0)
        {
            var stackTraceConstructor =
                typeof (StackTrace).GetConstructor(new[] {typeof (int), typeof (bool)});
            var stackTraceCtor = module.Import(stackTraceConstructor);

            var addDebugSymbols = OpCodes.Ldc_I4_1;
            IL.Emit(OpCodes.Ldc_I4, framesToSkip);
            IL.Emit(addDebugSymbols);
            IL.Emit(OpCodes.Newobj, stackTraceCtor);
        }

        /// <summary>
        /// Pushes a null value onto the stack.
        /// </summary>
        /// <param name="IL">The IL processor that will push the null values onto the stack.</param>
        /// <param name="numberOfNullsToPush">The number of values to push onto the stack.</param>
        public static void LdNull(this ILProcessor IL, int numberOfNullsToPush = 1)
        {
            for (var i = 0; i < numberOfNullsToPush; i++)
            {
                IL.Emit(OpCodes.Ldnull);
            }
        }

        /// <summary>
        ///     Emits a Console.WriteLine call to using the current ILProcessor that will only be called if the contents
        ///     of the target variable are null at runtime.
        /// </summary>
        /// <param name="IL">The target ILProcessor.</param>
        /// <param name="text">The text that will be written to the console.</param>
        /// <param name="targetVariable">The target variable that will be checked for null at runtime.</param>
        public static void EmitWriteLineIfNull(this ILProcessor IL, string text, VariableDefinition targetVariable)
        {
            var skipWrite = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, targetVariable);
            IL.Emit(OpCodes.Brtrue, skipWrite);
            IL.EmitWriteLine(text);
            IL.Append(skipWrite);
        }

        /// <summary>
        ///     Emits a Console.WriteLine call using the current ILProcessor.
        /// </summary>
        /// <param name="IL">The target ILProcessor.</param>
        /// <param name="text">The text that will be written to the console.</param>
        public static void EmitWriteLine(this ILProcessor IL, string text)
        {
            var body = IL.Body;
            var method = body.Method;
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            var writeLineMethod = typeof (Console).GetMethod("WriteLine",
                BindingFlags.Public | BindingFlags.Static, null,
                new[] {typeof (string)}, null);
            IL.Emit(OpCodes.Ldstr, text);
            IL.Emit(OpCodes.Call, module.Import(writeLineMethod));
        }

        /// <summary>
        ///     Converts the return value of a method into the <paramref name="returnType">target type</paramref>.
        ///     If the target type is void, then the value will simply be popped from the stack.
        /// </summary>
        /// <param name="il">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="returnType">The method return type itself.</param>
        public static void PackageReturnValue(this ILProcessor il, ModuleDefinition module, TypeReference returnType)
        {
            if (returnType.FullName == "System.Void")
            {
                il.Emit(OpCodes.Pop);
                return;
            }

            il.Emit(OpCodes.Unbox_Any, module.Import(returnType));
        }

        /// <summary>
        /// Pushes a field onto the stack
        /// </summary>
        /// <param name="il">The IL generator that will emit the necessary instructions.</param>
        /// <param name="field">The target field.</param>
        /// <param name="module">The module that contains the target field.</param>
        public static void PushField(this ILProcessor il, FieldReference field, ModuleDefinition module)
        {
            il.Emit(OpCodes.Ldfld, field);
        }
        
        /// <summary>
        ///     Pushes the current <paramref name="method" /> onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="method">The method that represents the <see cref="MethodInfo" /> that will be pushed onto the stack.</param>
        /// <param name="module">The module that contains the host method.</param>
        public static void PushMethod(this ILProcessor IL, MethodReference method, ModuleDefinition module)
        {
            var getMethodFromHandle = module.ImportMethod<MethodBase>("GetMethodFromHandle",
                typeof (RuntimeMethodHandle),
                typeof (RuntimeTypeHandle));

            var declaringType = method.DeclaringType;

            // Instantiate the generic type before determining
            // the current method
            if (declaringType.GenericParameters.Count > 0)
            {
                var genericType = new GenericInstanceType(declaringType);
                foreach (var parameter in declaringType.GenericParameters)
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
        /// Gets the current module that contains the IL processor.
        /// </summary>
        /// <param name="IL">The IL processor itself.</param>
        /// <returns>The module that contains the IL processor.</returns>
        public static ModuleDefinition GetModule(this ILProcessor IL)
        {
            return IL.Body.Method.DeclaringType.Module;
        }
        
        
        /// <summary>
        ///     Stores the <paramref name="param">current parameter value</paramref>
        ///     into the array of method <paramref name="arguments" />.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="arguments">The local variable that will store the method arguments.</param>
        /// <param name="index">The array index that indicates where the parameter value will be stored in the array of arguments.</param>
        /// <param name="param">The current argument value being stored.</param>
        private static void PushParameter(this ILProcessor IL, int index, VariableDefinition arguments,
            ParameterDefinition param)
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

        /// <summary>
        ///     Saves the current method signature of a method into an array
        ///     of <see cref="System.Type" /> objects. This can be used to determine the
        ///     signature of methods with generic type parameters or methods that have
        ///     parameters that are generic parameters specified by the type itself.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="method">The target method whose generic type arguments (if any) will be saved into the local variable .</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="parameterTypes">The local variable that will store the current method signature.</param>
        public static void SaveParameterTypes(this ILProcessor IL, MethodReference method, ModuleDefinition module,
            VariableDefinition parameterTypes)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle",
                BindingFlags.Public | BindingFlags.Static);
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
        ///     Pushes a <paramref name="Type" /> instance onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="type">The type that represents the <see cref="Type" /> that will be pushed onto the stack.</param>
        /// <param name="module">The module that contains the host method.</param>
        public static void PushType(this ILProcessor IL, TypeReference type, ModuleDefinition module)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle",
                typeof (RuntimeTypeHandle));

            // Instantiate the generic type before pushing it onto the stack
            var declaringType = GetDeclaringType(type);

            IL.Emit(OpCodes.Ldtoken, declaringType);
            IL.Emit(OpCodes.Call, getTypeFromHandle);
        }

        /// <summary>
        ///     Pushes the arguments of a method onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="method">The target method.</param>
        /// <param name="arguments">The <see cref="VariableDefinition">local variable</see> that will hold the array of arguments.</param>
        public static void PushArguments(this ILProcessor IL, IMethodSignature method, ModuleDefinition module,
            VariableDefinition arguments)
        {
            var objectType = module.ImportType(typeof (object));
            var parameterCount = method.Parameters.Count;
            IL.Emit(OpCodes.Ldc_I4, parameterCount);
            IL.Emit(OpCodes.Newarr, objectType);
            IL.Emit(OpCodes.Stloc, arguments);

            if (parameterCount == 0)
                return;

            var index = 0;
            foreach (var param in method.Parameters)
            {
                IL.PushParameter(index++, arguments, param);
            }
        }

        /// <summary>
        ///     Gets the declaring type for the target method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <returns>The declaring type.</returns>
        public static TypeReference GetDeclaringType(this MethodReference method)
        {
            var declaringType = method.DeclaringType;
            return GetDeclaringType(declaringType);
        }

        /// <summary>
        ///     Obtains the declaring type for a given type reference.
        /// </summary>
        /// <param name="declaringType">The declaring ty pe.</param>
        /// <returns>The actual declaring type.</returns>
        private static TypeReference GetDeclaringType(TypeReference declaringType)
        {
            // Instantiate the generic type before determining
            // the current method
            if (declaringType.GenericParameters.Count <= 0)
                return declaringType;

            var genericType = new GenericInstanceType(declaringType);
            foreach (var parameter in declaringType.GenericParameters)
            {
                genericType.GenericArguments.Add(parameter);
            }

            declaringType = genericType;
            return declaringType;
        }

        /// <summary>
        ///     Saves the generic type arguments that were used to construct the method.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor" /> that will be used to create the instructions.</param>
        /// <param name="method">
        ///     The target method whose generic type arguments (if any) will be saved into the
        ///     <paramref name="typeArguments">local variable</paramref>.
        /// </param>
        /// <param name="module">The module that contains the host method.</param>
        /// <param name="typeArguments">The local variable that will store the resulting array of <see cref="Type" /> objects.</param>
        public static void PushGenericArguments(this ILProcessor IL, IGenericParameterProvider method,
            ModuleDefinition module, VariableDefinition typeArguments)
        {
            var getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle",
                BindingFlags.Public | BindingFlags.Static);
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
    }
}