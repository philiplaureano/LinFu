using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Proxy.Interfaces;
using LinFu.IoC.Configuration;

namespace LinFu.Proxy
{
    /// <summary>
    /// Provides the default implementation for the 
    /// <see cref="IExtractInterfaces"/> interface.
    /// </summary>
    [Implements(typeof(IExtractInterfaces), LifecycleType.OncePerRequest)]
    public class InterfaceExtractor : IExtractInterfaces
    {
        /// <summary>
        /// Determines which interfaces a given type should implement.
        /// </summary>
        /// <param name="currentType">The base type that holds the list of interfaces to implement.</param>
        /// <param name="interfaceList">The list of interfaces already being implemented. </param>
        public void GetInterfaces(Type currentType, HashSet<Type> interfaceList)
        {
            var interfaces = currentType.GetInterfaces();
            if (interfaces == null || interfaces.Length == 0)
                return;

            foreach (Type current in interfaces)
            {
                if (interfaceList.Contains(current))
                    continue;

                interfaceList.Add(current);
                GetInterfaces(current, interfaceList);
            }
        }
    }
}
