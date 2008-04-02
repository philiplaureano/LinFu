using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using System.Reflection;
using Simple.IoC.Loaders;

namespace LinFu.MxClone
{
    [Implements(typeof(IAssemblyCache), LifecycleType.Singleton)]
    public class DefaultAssemblyCache : IAssemblyCache
    {
        private readonly Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();
        #region IAssemblyCache Members

        public bool Contains(string assemblyQualifiedName)
        {
            return _cache.ContainsKey(assemblyQualifiedName);
        }

        public Assembly Retrieve(string assemblyQualifiedName)
        {
            return _cache[assemblyQualifiedName];
        }

        public void Store(string assemblyQualifiedName, Assembly assembly)
        {
            _cache[assemblyQualifiedName] = assembly;
        }

        #endregion
    }
}
