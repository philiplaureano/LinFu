using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using LinFu.MxClone.Interfaces;
using LinFu.Reflection.Extensions;

namespace LinFu.MxClone.Indexes
{
    public class TypeIndex : ITypeIndex
    {
        private readonly Dictionary<string, Type> _index = new Dictionary<string, Type>();
        private readonly Assembly _targetAssembly;
        public TypeIndex(Assembly targetAssembly)
        {
            _targetAssembly = targetAssembly;

            if (targetAssembly == null)
                return;

            CreateIndexFrom(targetAssembly);
        }
        public Type Resolve(string typename)
        {
            if (_index.ContainsKey(typename))
                return _index[typename];

            return null;
        }
        private void CreateIndexFrom(Assembly targetAssembly)
        {
            // Index the types associated with this assembly
            foreach (var currentType in targetAssembly.GetLoadedTypes())
            {
                if (currentType.IsAbstract)
                    continue;

                _index[currentType.Name] = currentType;
            }
        }
    }
}
