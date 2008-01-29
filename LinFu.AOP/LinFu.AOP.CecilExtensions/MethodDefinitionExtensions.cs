using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.CecilExtensions
{
    public static class MethodDefinitionExtensions
    {
        public static VariableDefinition AddLocal(this MethodDefinition methodDef, Type localType)
        {
            TypeReference declaringType = methodDef.DeclaringType;
            ModuleDefinition module = declaringType.Module;
            TypeReference variableType = module.Import(localType);
            VariableDefinition result = new VariableDefinition(variableType);

            methodDef.Body.Variables.Add(result);

            return result;
        }
    }
}
