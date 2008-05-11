using System;
using System.Collections.Generic;

namespace LinFu.DynamicProxy
{
    internal class ProxyCache : IProxyCache
    {
        private static readonly Dictionary<ProxyCacheEntry, Type> _cache = new Dictionary<ProxyCacheEntry, Type>();

        #region IProxyCache Members

        public bool Contains(Type baseType, params Type[] baseInterfaces)
        {
            ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
            return _cache.ContainsKey(entry);
        }

        public Type GetProxyType(Type baseType, params Type[] baseInterfaces)
        {
            ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
            return _cache[entry];
        }

        public void StoreProxyType(Type result, Type baseType, params Type[] baseInterfaces)
        {
            ProxyCacheEntry entry = new ProxyCacheEntry(baseType, baseInterfaces);
            _cache[entry] = result;
        }

        #endregion
    }
}