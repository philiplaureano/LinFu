using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// Represents the basic interface for creating
    /// dynamic proxy instances.
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Generates a dynamic proxy type
        /// that derives from the <paramref name="baseType"/>
        /// and implements the given <paramref name="baseInterfaces">interfaces</paramref>.
        /// </summary>
        /// <param name="baseType">The base class from which the generated dynamic proxy will be derived.</param>
        /// <param name="baseInterfaces">The list of interfaces that the generated dynamic proxy will implement.</param>
        /// <returns>A dynamic proxy type.</returns>
        Type CreateProxyType(Type baseType, IEnumerable<Type> baseInterfaces);
    }
}
