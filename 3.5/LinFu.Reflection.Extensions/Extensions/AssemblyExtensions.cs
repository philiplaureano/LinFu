using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Reflection.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] GetLoadedTypes(this Assembly currentAssembly)
        {
            Type[] loadedTypes = null;
            try
            {
                loadedTypes = currentAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                loadedTypes = ex.Types;
            }

            return loadedTypes;
        }
    }
}
