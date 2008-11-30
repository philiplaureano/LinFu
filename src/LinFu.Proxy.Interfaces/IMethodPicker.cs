using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents a class that determines which methods should be proxied
    /// by a given proxy factory.
    /// </summary>
    public interface IMethodPicker
    {
        /// <summary>
        /// Determines which methods should be proxied
        /// by a given proxy factory.
        /// </summary>
        /// <param name="baseType">The base class of the proxy type currently being generated.</param>
        /// <param name="baseInterfaces">The list of interfaces that the proxy must implement.</param>
        /// <returns>A list of <see cref="MethodInfo"/> objects that can be proxied.</returns>
        IEnumerable<MethodInfo> ChooseProxyMethodsFrom(Type baseType, IEnumerable<Type> baseInterfaces);
    }
}
