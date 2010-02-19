using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public static class MethodDefinitionExtensions
    {
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
