using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.MxClone.Extensions
{
    public static class CompositeTypeResolverExtensions
    {
        public static void AddAssembly(this CompositeTypeResolver resolver, Assembly assembly)
        {
            resolver.Resolvers.Add(new AssemblyBoundTypeResolver(assembly));
        }
    }
}
