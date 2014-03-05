using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Extensions
{
    /// <summary>
    /// Represents an extension class that adds helper methods to the <see cref="MethodDefinition"/> type.
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        /// <summary>
        /// Adds a local variable to the given method.
        /// </summary>
        /// <param name="methodDef">The target method.</param>
        /// <param name="localType">The variable type.</param>
        /// <returns>A local variable definition.</returns>
        public static VariableDefinition AddLocal(this MethodDefinition methodDef, Type localType)
        {
            var declaringType = methodDef.DeclaringType;
            var module = declaringType.Module;
            var variableType = module.Import(localType);
            var result = new VariableDefinition(variableType);

            methodDef.Body.Variables.Add(result);

            return result;
        }
    }
}