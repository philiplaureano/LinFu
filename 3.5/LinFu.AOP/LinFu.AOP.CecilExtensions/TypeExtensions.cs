using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;

namespace LinFu.AOP.CecilExtensions
{
    public static class TypeExtensions
    {
        public static TypeReference ImportInto(this Type type, ModuleDefinition module)
        {
            return module.ImportType(type);
        }
    }
}
