using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;

namespace LinFu.AOP.Weavers.Cecil
{
    public static class AssemblyDefinitionExtensions
    {
        public static void InjectAspectFramework(this AssemblyDefinition assembly, 
            IMethodFilter methodFilter,
            bool shouldInjectConstructors)
        {
            AspectWeaver weaver = new AspectWeaver();
            assembly.WeaveWith(weaver);
            weaver.MethodFilter = methodFilter;
            if (!shouldInjectConstructors)
                return;

            ConstructorCrossCutter crosscutter = new ConstructorCrossCutter();
            assembly.WeaveWith(crosscutter);
        }
        public static void InjectAspectFramework(this AssemblyDefinition assembly,
            bool shouldInjectConstructors)
        {
            InjectAspectFramework(assembly, null, shouldInjectConstructors);
        }
        public static void InjectAspectFramework(this AssemblyDefinition assembly)
        {
            assembly.InjectAspectFramework(true);
        }
    }
}
