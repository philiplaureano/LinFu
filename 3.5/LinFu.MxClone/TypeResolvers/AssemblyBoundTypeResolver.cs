using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;

namespace LinFu.MxClone
{
    internal class AssemblyBoundTypeResolver : ITypeResolver
    {

        public Assembly TargetAssembly { get; set; }
        public AssemblyBoundTypeResolver(Assembly targetAssembly)
        {
            TargetAssembly = targetAssembly;
        }
        #region ITypeResolver Members

        public Type Resolve(string typename, string assemblyQualifiedName)
        {
            if (assemblyQualifiedName != TargetAssembly.FullName)
                return null;

            Type result = null;
            try
            {
                var matches = (from t in TargetAssembly.GetLoadedTypes()
                               where t.Name == typename
                               select t).ToList();

                if (matches.Count > 0)
                    result = matches.First();

            }
            catch
            {
                // Ignore the error
            }

            return result;
        }

        #endregion
    }
}
