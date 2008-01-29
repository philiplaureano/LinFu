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
            bool shouldInjectConstructors)
        {
            AspectWeaver weaver = new AspectWeaver();
            assembly.WeaveWith(weaver);

            if (!shouldInjectConstructors)
                return;

            ConstructorCrossCutter crosscutter = new ConstructorCrossCutter();
            assembly.WeaveWith(crosscutter);
        }
        public static void InjectAspectFramework(this AssemblyDefinition assembly)
        {
            assembly.InjectAspectFramework(true);
        }
    }
}
