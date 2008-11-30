using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents an interface for classes that store results from an
    /// <see cref="IProxyFactory"/> instance.
    /// </summary>
    public interface IProxyCache
    {
        /// <summary>
        /// Determines whether or not the cache contains an existing proxy type
        /// that is derived from the <paramref name="baseType"/> and implements
        /// the given <paramref name="baseInterfaces"/>.
        /// </summary>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        /// <returns>Returns <c>true</c> if the proxy type already exists; otherwise, it will return <c>false.</c></returns>
        bool Contains(Type baseType, params Type[] baseInterfaces);

        /// <summary>
        /// Retrieves an existing proxy type from the cache.
        /// </summary>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        /// <returns>Returns a valid <see cref="Type"/> if the type already exists; otherwise, it might return <c>null</c> or opt to throw an exception.</returns>
        Type Get(Type baseType, params Type[] baseInterfaces);

        /// <summary>
        /// Stores a proxy type in the cache.
        /// </summary>
        /// <param name="result">The proxy type to be stored.</param>
        /// <param name="baseType">The base type of the dynamically-generated proxy type.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated proxy type must implement.</param>
        void Store(Type result, Type baseType, params Type[] baseInterfaces);
    }
}
