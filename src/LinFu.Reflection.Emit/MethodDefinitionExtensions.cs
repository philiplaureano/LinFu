using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that extends the <see cref="MethodDefinition"/>
    /// class with features similar to the features in the
    /// System.Reflection.Emit namespace.
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        /// <summary>
        /// Returns the <see cref="CilWorker"/> instance
        /// associated with the body of the <paramref name="method">target method</paramref>.
        /// </summary>
        /// <param name="method">The target method to be modified.</param>
        /// <returns>The <see cref="CilWorker"/> instance that points to the instructions of the method body.</returns>
        public static CilWorker GetILGenerator(this MethodDefinition method)
        {
            return method.Body.CilWorker;
        }
        
        /// <summary>
        /// Adds a <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="methodDef">method definition</paramref>.
        /// </summary>
        /// <param name="methodDef">The <paramref name="methodDef"/> instance which will contain the local variable.</param>
        /// <param name="localType">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</param>
        /// <returns>A <see cref="VariableDefinition"/> that represents the local variable itself.</returns>
        public static VariableDefinition AddLocal(this MethodDefinition methodDef, Type localType)
        {
            var declaringType = methodDef.DeclaringType;
            var module = declaringType.Module;
            var variableType = module.Import(localType);
            var result = new VariableDefinition(variableType);

            methodDef.Body.Variables.Add(result);

            return result;
        }

        /// <summary>
        /// Adds a set of parameter types to the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="parameterTypes">The list of types that describe the method signature.</param>
        public static void AddParameters(this MethodDefinition method, Type[] parameterTypes)
        {
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            // Build the parameter list
            foreach (var type in parameterTypes)
            {
                var parameterType = type.IsGenericParameter ? method.AddGenericParameter(type) :
                    module.Import(type);

                var param = new ParameterDefinition(parameterType);
                method.Parameters.Add(param);
            }
        }

        /// <summary>
        /// Assigns the <paramref name="returnType"/> for the target method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="returnType">The <see cref="System.Type"/> instance that describes the return type.</param>
        public static void SetReturnType(this MethodDefinition method, Type returnType)
        {
            var declaringType = method.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            TypeReference actualReturnType;

            if (returnType.IsGenericType && returnType.ContainsGenericParameters)
            {
                SetReturnTypeWithOpenGenericParameters(method, returnType, module);
                return;
            }

            if (returnType.IsGenericParameter)
            {
                actualReturnType = method.AddGenericParameter(returnType);
            }
            else
            {
                actualReturnType = module.Import(returnType);
            }

            method.ReturnType.ReturnType = actualReturnType;
        }

        /// <summary>
        /// Assigns a return type with an open generic parameter to a target method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="returnType">The return type with the open generic parameters.</param>
        /// <param name="module">The host module.</param>
        private static void SetReturnTypeWithOpenGenericParameters(MethodDefinition method, Type returnType, ModuleDefinition module)
        {
            TypeReference actualReturnType = null;
            var typeArgumentNames = from t in returnType.GetGenericArguments()
                                    select t.Name;

            // Add any missing type arguments in the host method
            foreach(var name in typeArgumentNames)
            {
                bool found = false;
                foreach(GenericParameter param in method.GenericParameters)
                {
                    if (param.Name != name)
                        continue;

                    found = true;
                    break;
                }

                if (found)
                    continue;

                method.GenericParameters.Add(new GenericParameter(name, method));
            }

            actualReturnType = module.Import(returnType, method);
            method.ReturnType.ReturnType = actualReturnType;
        }

        /// <summary>
        /// Adds a generic parameter type to the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <returns>A <see cref="TypeReference"/> that represents the generic parameter type.</returns>
        public static TypeReference AddGenericParameter(this MethodDefinition method, Type parameterType)
        {

            // Check if the parameter type already exists
            var matches = (from GenericParameter p in method.GenericParameters
                          where p.Name == parameterType.Name
                          select p).ToList();

            // Reuse the existing parameter
            if (matches.Count > 0)
                return matches[0];

            var parameter = new GenericParameter(parameterType.Name, method);
            method.GenericParameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// Adds a <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="methodDef">method definition</paramref>.
        /// </summary>
        /// <typeparam name="T">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</typeparam>
        /// <param name="methodDef">The <paramref name="methodDef"/> instance which will contain the local variable.</param>        
        /// <returns>A <see cref="VariableDefinition"/> that represents the local variable itself.</returns>        
        public static VariableDefinition AddLocal<T>(this MethodDefinition methodDef)
        {
            return methodDef.AddLocal(typeof (T));
        }
    }
}
