using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using LinFu.Proxy.Interfaces;
using LinFu.IoC.Configuration;
using Mono.Cecil;
using GenericParameterAttributes=Mono.Cecil.GenericParameterAttributes;

namespace LinFu.Proxy
{
    /// <summary>
    /// Represents the default implementation of the
    /// <see cref="IMethodBuilder"/> interface.
    /// </summary>
    [Implements(typeof(IMethodBuilder), LifecycleType.OncePerRequest, ServiceName = "ProxyMethodBuilder")]
    internal class ProxyMethodBuilder : IMethodBuilder, IInitialize
    {
        /// <summary>
        /// Initializes the <see cref="ProxyMethodBuilder"/> class with the default property values.
        /// </summary>
        public ProxyMethodBuilder()
        {
            Emitter = new MethodBodyEmitter();
        }
        /// <summary>
        /// Creates a method that matches the signature defined in the
        /// <paramref name="method"/> parameter.
        /// </summary>
        /// <param name="targetType">The type that will host the new method.</param>
        /// <param name="method">The method from which the signature will be derived.</param>
        public MethodDefinition CreateMethod(TypeDefinition targetType, MethodInfo method)
        {
            #region Match the method signature
            var module = targetType.Module;            
            var methodName = method.Name;

            // If the method is a member defined on an interface type,
            // we need to rename the method to avoid
            // any naming conflicts in the type itself
            if (method.DeclaringType.IsInterface)
            {
                var parentName = method.DeclaringType.FullName;
                
                // Rename the parent type to its fully qualified name
                // if it is a generic type
                methodName = string.Format("{0}.{1}", parentName, methodName);
            }

            var baseAttributes = Mono.Cecil.MethodAttributes.Virtual | 
                Mono.Cecil.MethodAttributes.HideBySig;

            var attributes = default(Mono.Cecil.MethodAttributes);

            #region Match the visibility of the target method

            if (method.IsFamilyOrAssembly)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.FamORAssem;

            if (method.IsFamilyAndAssembly)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.FamANDAssem;

            if (method.IsPublic)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.Public;
            
            #endregion

            // Build the list of parameter types
            var parameterTypes = (from param in method.GetParameters()
                                  let type = param.ParameterType
                                  let importedType = type                                  
                                  select importedType).ToArray();
            
            
            //Build the list of generic parameter types
            var genericParameterTypes = method.GetGenericArguments();

            var newMethod = targetType.DefineMethod(methodName, attributes,
                                                    method.ReturnType, parameterTypes,genericParameterTypes);

            newMethod.Body.InitLocals = true;
            newMethod.ImplAttributes = Mono.Cecil.MethodImplAttributes.IL | Mono.Cecil.MethodImplAttributes.Managed;
            newMethod.HasThis = true;                       

            // Match the generic type arguments
            var typeArguments = method.GetGenericArguments();

            if (typeArguments != null || typeArguments.Length > 0)
                MatchGenericArguments(newMethod, typeArguments);

            var originalMethodRef = module.Import(method);
            newMethod.Overrides.Add(originalMethodRef);

            #endregion            

            // Define the method body
            if (Emitter != null)
                Emitter.Emit(method, newMethod);

            return newMethod;
        }

        /// <summary>
        /// The <see cref="IMethodBodyEmitter"/> instance that will be
        /// responsible for generating the method body.
        /// </summary>
        public IMethodBodyEmitter Emitter
        {
            get; set;
        }

        /// <summary>
        /// Matches the generic parameters of <paramref name="newMethod">a target method</paramref>
        /// </summary>
        /// <param name="newMethod">The generic method that contains the generic type arguments.</param>
        /// <param name="typeArguments">The array of <see cref="Type"/> objects that describe the generic parameters for the current method.</param>
        private static void MatchGenericArguments(MethodDefinition newMethod, ICollection<Type> typeArguments)
        {
            foreach (var argument in typeArguments)
            {
                newMethod.AddGenericParameter(argument);
            }
        }

        /// <summary>
        /// Initializes the class with the <paramref name="source"/> container.
        /// </summary>
        /// <param name="source">The <see cref="IServiceContainer"/> instance that will create the class instance.</param>
        public void Initialize(IServiceContainer source)
        {
            Emitter = (IMethodBodyEmitter)source.GetService(typeof (IMethodBodyEmitter));
        }
    }
}
