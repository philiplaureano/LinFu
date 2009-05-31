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
        /// Adds a named <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="method">method definition</paramref>.
        /// </summary>
        /// <param name="method">The <paramref name="method"/> instance which will contain the local variable.</param>
        /// <param name="variableName">The name of the local variable.</param>
        /// <param name="variableType">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</param>
        /// <returns></returns>
        public static VariableDefinition AddLocal(this MethodDefinition method, string variableName, Type variableType)
        {
            var module = method.DeclaringType.Module;
            var localType = module.Import(variableType);

            VariableDefinition newLocal = null;
            foreach (VariableDefinition local in method.Body.Variables)
            {
                // Match the variable name and type
                if (local.Name != variableName || local.VariableType != localType)
                    continue;

                newLocal = local;
            }

            // If necessary, create the local variable
            if (newLocal == null)
            {
                var body = method.Body;
                var index = body.Variables.Count;

                newLocal = new VariableDefinition(variableName, index, method, localType);

                body.Variables.Add(newLocal);
            }

            return newLocal;
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
                TypeReference parameterType = GetParameterType(method, module, type);

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

            actualReturnType = returnType.IsGenericParameter ? method.AddGenericParameter(returnType) : module.Import(returnType);
            method.ReturnType.ReturnType = actualReturnType;
        }

        /// <summary>
        /// Determines the actual parameter type of a given method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="module">The module that contains the method's host type.</param>
        /// <param name="type">The parameter type.</param>
        /// <returns>A <see cref="TypeReference"/> that describes the actual parameter type.</returns>
        private static TypeReference GetParameterType(MethodDefinition method, ModuleDefinition module, Type type)
        {
            TypeReference parameterType = null;

            if (type.ContainsGenericParameters && type.IsGenericType)
            {
                foreach (var genericParam in type.GetGenericArguments())
                {
                    method.GenericParameters.Add(new GenericParameter(genericParam.Name, method));
                }

                return module.Import(type, method);
            }

            parameterType = type.IsGenericParameter ? method.AddGenericParameter(type) : module.Import(type);

            return parameterType;
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
                var parameterCount = method.GenericParameters.Count;
                var parameters = method.GenericParameters.Cast<GenericParameter>();

                var matches = (from p in parameters
                              where p.Name == name
                              select p).Count();

                found = matches > 0;
               
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
            return methodDef.AddLocal(typeof(T));
        }

        /// <summary>
        /// Adds a named <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="methodDef">method definition</paramref>.
        /// </summary>
        /// <typeparam name="T">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</typeparam>
        /// <param name="methodDef">The <paramref name="methodDef"/> instance which will contain the local variable.</param>
        /// <param name="variableName">The name of the local variable.</param>
        /// <returns>A <see cref="VariableDefinition"/> that represents the local variable itself.</returns>        
        public static VariableDefinition AddLocal<T>(this MethodDefinition methodDef, string variableName)
        {
            return methodDef.AddLocal(variableName, typeof(T));
        }
    }
}
