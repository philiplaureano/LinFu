using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Proxy
{
    /// <summary>
    /// Represents a cached proxy type.
    /// </summary>
    internal class ProxyCacheEntry
    {
        public Type BaseType;
        public Type[] Interfaces;

        internal ProxyCacheEntry(Type baseType, Type[] interfaces)
        {
            BaseType = baseType;
            Interfaces = interfaces;            
        }

        internal class EqualityComparer : IEqualityComparer<ProxyCacheEntry>
        {
            public bool Equals(ProxyCacheEntry x, ProxyCacheEntry y)
            {
                // Match the base t ypes
                if (y.BaseType != x.BaseType)
                    return false;

                // If two types have the same base class and
                // no interface, then we have a match
                if (x.Interfaces.Length == 0 && y.Interfaces.Length == 0)
                    return true;

                // If one set of interfaces is null and the other one is not
                // null, then there is no match
                if ((x.Interfaces == null && y.Interfaces != null) ||
                    (y.Interfaces == null && x.Interfaces != null))
                    return false;
                
                // Initialize both interface lists and 
                // set them up for comparison
                var interfaceList = new HashSet<Type>();
                var targetList = new List<Type>();

                if (x.Interfaces != null && x.Interfaces.Length > 0)
                    targetList.AddRange(x.Interfaces);

                if (y.Interfaces != null)
                    interfaceList = new HashSet<Type>(y.Interfaces);

                // The length of the interfaces must match
                if (interfaceList.Count != targetList.Count)
                    return false;

                foreach (var current in targetList)
                {
                    if (!interfaceList.Contains(current))
                        return false;
                }

                return true;
            }

            public int GetHashCode(ProxyCacheEntry obj)
            {
                var extractor = new InterfaceExtractor();
                var types = new HashSet<Type>(obj.Interfaces);
                extractor.GetInterfaces(obj.BaseType, types);

                // HACK: Calculate the hash code
                // by XORing all the types together
                var baseType = obj.BaseType;
                int result = baseType.GetHashCode();
                foreach(var type in types)
                {
                    result ^= type.GetHashCode();
                }

                return result;
            }
        }
    }
}
