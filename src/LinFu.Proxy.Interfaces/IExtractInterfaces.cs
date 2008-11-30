using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Proxy.Interfaces
{
    /// <summary>
    /// A class that is responsible for determining
    /// which interfaces a given type should implement.
    /// </summary>
    public interface IExtractInterfaces
    {
        /// <summary>
        /// Determines which interfaces a given type should implement.
        /// </summary>
        /// <param name="currentType">The base type that holds the list of interfaces to implement.</param>
        /// <param name="interfaces">The list of interfaces already being implemented. </param>
        void GetInterfaces(Type currentType, HashSet<Type> interfaces);
    }
}
