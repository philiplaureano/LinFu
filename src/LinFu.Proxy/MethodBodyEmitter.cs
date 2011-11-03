using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Extensions;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace LinFu.Proxy
{
    /// <summary>
    /// Provides the default implementation for the
    /// <see cref="IMethodBodyEmitter"/> interface.
    /// </summary>
    [Implements(typeof (IMethodBodyEmitter), LifecycleType.OncePerRequest)]
    internal class MethodBodyEmitter : IMethodBodyEmitter, IInitialize
    {
        /// <summary>
        /// Initializes the class with the default values.
        /// </summary>
        public MethodBodyEmitter()
        {
            InvocationInfoEmitter = new InvocationInfoEmitter();
        }

        /// <summary>
        /// The <see cref="IEmitInvocationInfo"/> instance that
        /// </summary>
        public IEmitInvocationInfo InvocationInfoEmitter { get; set; }

        #region IInitialize Members

        /// <summary>
        /// Initializes the MethodBodyEmitter class.
        /// </summary>
        /// <param name="source"></param>
        public void Initialize(IServiceContainer source)
        {
            InvocationInfoEmitter = (IEmitInvocationInfo) source.GetService(typeof (IEmitInvocationInfo));
        }

        #endregion

        #region IMethodBodyEmitter Members

        /// <summary>
        /// Generates a method body for the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="originalMethod">The method currently being intercepted.</param>
        /// <param name="targetMethod">The target method that will contain the new method body.</param>
        public void Emit(MethodInfo originalMethod, MethodDefinition targetMethod)
        {
            VariableDefinition invocationInfo = targetMethod.AddLocal<IInvocationInfo>();
            invocationInfo.Name = "___invocationInfo___";

            // Emit the code to generate the IInvocationInfo instance
            // and save it into the invocationInfo local variable
            if (InvocationInfoEmitter != null)
                InvocationInfoEmitter.Emit(originalMethod, targetMethod, invocationInfo);

            TypeDefinition declaringType = targetMethod.DeclaringType;
            ModuleDefinition module = declaringType.Module;
            TypeReference proxyType = module.ImportType<IProxy>();
            MethodReference getInterceptorMethod = module.ImportMethod("get_Interceptor", typeof (IProxy));
            VariableDefinition interceptor = targetMethod.AddLocal<IInterceptor>();
            VariableDefinition arguments = targetMethod.AddLocal<object[]>();

            // if (!(this is IProxy))
            ILProcessor IL = targetMethod.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);

            Instruction noImplementationFound = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Brfalse, noImplementationFound);

            Instruction endLabel = IL.Create(OpCodes.Nop);
            EmitGetInterceptorInstruction(IL, proxyType, getInterceptorMethod);
            IL.Emit(OpCodes.Stloc, interceptor);

            //If (interceptor == null)
            //    throw a not implemented exception here
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Brfalse, noImplementationFound);


            // var returnValue = interceptor.Intercept(info);
            TypeReference voidType = module.ImportType(typeof (void));
            MethodReference interceptMethod = module.ImportMethod<IInterceptor>("Intercept", typeof (IInvocationInfo));
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);

            // Save the ref arguments
            IEnumerable<ParameterDefinition> parameters = from ParameterDefinition param in targetMethod.Parameters
                                                          select param;

            // Determine the return type
            TypeReference returnType = targetMethod.ReturnType != null
                                           ? targetMethod.ReturnType
                                           : voidType;

            IL.PackageReturnValue(module, returnType);

            SaveRefArguments(IL, parameters, invocationInfo, arguments);
            IL.Emit(OpCodes.Br, endLabel);

            // This code at this point will execute if no implementation
            // is found
            IL.Append(noImplementationFound);

            ImplementNotFound(IL);

            IL.Append(endLabel);
            IL.Emit(OpCodes.Ret);
        }

        #endregion

        /// <summary>
        /// Emits the IL instructions to obtain an <see cref="IInterceptor"/> instance for the proxy type.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> responsible for emitting the method body.</param>
        /// <param name="proxyType">The proxy type.</param>
        /// <param name="getInterceptorMethod">The getter method for the interceptor.</param>
        protected virtual void EmitGetInterceptorInstruction(ILProcessor IL, TypeReference proxyType,
                                                             MethodReference getInterceptorMethod)
        {
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);
            IL.Emit(OpCodes.Callvirt, getInterceptorMethod);
        }

        /// <summary>
        /// Causes the <see cref="ILProcessor"/> to make the method throw a
        /// <see cref="NotImplementedException"/> if the method cannot be found.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> responsible for emitting the method body.</param>
        protected virtual void ImplementNotFound(ILProcessor IL)
        {
            MethodBody body = IL.Body;
            TypeDefinition declaringType = body.Method.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            // throw new NotImplementedException();
            MethodReference notImplementedConstructor = module.ImportConstructor<NotImplementedException>();
            IL.Emit(OpCodes.Newobj, notImplementedConstructor);
            IL.Emit(OpCodes.Throw);
        }

        /// <summary>
        /// Saves the ref arguments of a given method using the
        /// <paramref name="arguments"/> from the <paramref name="invocationInfo"/>
        /// object.
        /// </summary>
        /// <param name="IL">The <see cref="ILProcessor"/> that will emit the method body.</param>
        /// <param name="parameters">The parameters of the target method.</param>
        /// <param name="invocationInfo">The local variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="arguments">The local variable that will store the arguments from the <see cref="IInvocationInfo"/> instance.</param>
        private static void SaveRefArguments(ILProcessor IL, IEnumerable<ParameterDefinition> parameters,
                                             VariableDefinition invocationInfo, VariableDefinition arguments)
        {
            MethodBody body = IL.Body;
            MethodDefinition targetMethod = body.Method;
            TypeDefinition declaringType = targetMethod.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            // Save the arguments returned from the handler method
            MethodReference getArguments = module.ImportMethod<IInvocationInfo>("get_Arguments");

            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, getArguments);
            IL.Emit(OpCodes.Stloc, arguments);

            int index = 0;
            foreach (ParameterDefinition param in parameters)
            {
                if (!param.IsByRef())
                {
                    index++;
                    continue;
                }
                // Load the destination address
                IL.Emit(OpCodes.Ldarg, index + 1);

                // Load the argument value
                IL.Emit(OpCodes.Ldloc, arguments);
                IL.Emit(OpCodes.Ldc_I4, index++);
                IL.Emit(OpCodes.Ldelem_Ref);

                // Determine the actual parameter type
                var referenceType = param.ParameterType as TypeSpecification;
                if (referenceType == null)
                    continue;

                TypeReference actualParameterType = referenceType.ElementType;
                IL.Emit(OpCodes.Unbox_Any, actualParameterType);
                IL.Stind(param.ParameterType);
            }
        }
    }
}