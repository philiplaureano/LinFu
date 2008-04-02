using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.MxClone.Interfaces;
using System.Reflection;
using LinFu.MxClone.Indexes;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.MxClone
{
    [Implements(typeof(ITypeResolver), LifecycleType.OncePerRequest)]
    public class DefaultTypeResolver : ITypeResolver, IInitialize
    {
        private Dictionary<string, ITypeIndex> _indexes = new Dictionary<string, ITypeIndex>();
        #region ITypeResolver Members

        public IAssemblyCache AssemblyCache
        {
            get;
            set;
        }

        public Type Resolve(string typename, string assemblyQualifiedName)
        {
            if (AssemblyCache == null)
                return null;

            // Reused the cached results
            Assembly currentAssembly = null;
            if (!AssemblyCache.Contains(assemblyQualifiedName))
            {
                try
                {

                    currentAssembly = Assembly.Load(assemblyQualifiedName);
                }
                catch (Exception ex)
                {
                    // Ignore the error
                }
                if (currentAssembly == null)
                    return null;

                AssemblyCache.Store(assemblyQualifiedName, currentAssembly);
            }

            currentAssembly = AssemblyCache.Retrieve(assemblyQualifiedName);
            if (currentAssembly == null)
                return null;


            ITypeIndex currentIndex = null;
            if (_indexes.ContainsKey(assemblyQualifiedName))
                currentIndex = _indexes[assemblyQualifiedName];

            // Index the types in the assembly, if necessary
            if (currentIndex == null)
            {
                currentIndex = new TypeIndex(currentAssembly);
                _indexes[assemblyQualifiedName] = currentIndex;
            }

            return currentIndex.Resolve(typename);
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            AssemblyCache = container.GetService<IAssemblyCache>();
        }

        #endregion
    }
}
