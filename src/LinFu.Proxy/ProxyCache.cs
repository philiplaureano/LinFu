using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.Proxy.Interfaces;

namespace LinFu.Proxy
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IProxyCache"/> interface.
    /// </summary>
    [Implements(typeof(IProxyCache), LifecycleType.OncePerRequest)]
    public class ProxyCache : IProxyCache
    {
        private static readonly Dictionary<ProxyCacheEntry, Type> _cache = new Dictionary<ProxyCacheEntry, Type>(new ProxyCacheEntry.EqualityComparer());

        /// <summary>
        /// Determines whether or not the cache contains an existing proxy type
        /// that is derived from the <paramref name="baseType"/> and implements
        /// the given <paramref name="baseInterfaces"/>.
        /// </summary>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        /// <returns>Returns <c>true</c> if the proxy type already exists; otherwise, it will return <c>false.</c></returns>
        public bool Contains(Type baseType, params Type[] baseInterfaces)
        {
            var entry = new ProxyCacheEntry(baseType, baseInterfaces);
            return _cache.ContainsKey(entry);
        }

        /// <summary>
        /// Retrieves an existing proxy type from the cache.
        /// </summary>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        /// <returns>Returns a valid <see cref="Type"/> if the type already exists; otherwise, it might return <c>null</c> or opt to throw an exception.</returns>
        public Type Get(Type baseType, params Type[] baseInterfaces)
        {
            var entry = new ProxyCacheEntry(baseType, baseInterfaces);
            return _cache[entry];
        }

        /// <summary>
        /// Stores a proxy type in the cache.
        /// </summary>
        /// <param name="result">The proxy type to be stored.</param>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        public void Store(Type result, Type baseType, params Type[] baseInterfaces)
        {
            var entry = new ProxyCacheEntry(baseType, baseInterfaces);

            lock (_cache)
            {
                _cache[entry] = result;
            }
        }
    }
}
