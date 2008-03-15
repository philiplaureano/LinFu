using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.CecilExtensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static void WeaveWith(this AssemblyDefinition assembly, IWeaver<TypeDefinition> typeWeaver)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (typeWeaver == null)
                throw new ArgumentNullException("typeWeaver");

            ModuleDefinition module = assembly.MainModule;
            typeWeaver.ImportReferences(module);
            var matches = (from TypeDefinition t in module.Types
                           where t != null && t.Name != "<Module>"
                           && typeWeaver.ShouldWeave(t)
                           select t).ToList();

            matches.ForEach(match => typeWeaver.Weave(match));
        }
        public static void WeaveWith(this AssemblyDefinition assembly,
            Predicate<TypeDefinition> typeFilter, IMethodWeaver methodWeaver)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (typeFilter == null)
                throw new ArgumentNullException("typeFilter");

            if (methodWeaver == null)
                throw new ArgumentNullException("methodWeaver");

            ModuleDefinition module = assembly.MainModule;
            var matches = (from TypeDefinition t in module.Types
                           where t != null && t.Name != "<Module>"
                           && typeFilter(t)
                           select t).ToList();

            matches.ForEach(type => type.WeaveWith(methodWeaver));
        }
        public static void WeaveWith(this AssemblyDefinition assembly, IMethodWeaver methodWeaver)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (methodWeaver == null)
                throw new ArgumentNullException("methodWeaver");

            ModuleDefinition module = assembly.MainModule;
            var matches = (from TypeDefinition t in module.Types
                           where t != null && t.Name != "<Module>"
                           && t.IsClass
                           select t).ToList();

            matches.ForEach(type => type.WeaveWith(methodWeaver));
        }
        public static void Save(this AssemblyDefinition assembly, string filename)
        {
            AssemblyFactory.SaveAssembly(assembly, filename);
        }
    }
}
