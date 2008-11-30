using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.Proxy.Interfaces;
using LinFu.IoC.Configuration;

namespace LinFu.Proxy
{
    /// <summary>
    /// Represents the default class implementation for the
    /// <see cref="IMethodPicker"/> interface.
    /// </summary>
    [Implements(typeof(IMethodPicker), LifecycleType.OncePerRequest)]
    public class MethodPicker : IMethodPicker
    {
        /// <summary>
        /// Determines which methods can be proxied from 
        /// the given <paramref name="baseType"/> and <paramref name="baseInterfaces"/>. 
        /// </summary>
        /// <remarks>By default, only public virtual methods will be proxied.</remarks>
        /// <param name="baseType">The base class of the proxy type currently being generated.</param>
        /// <param name="baseInterfaces">The list of interfaces that the proxy must implement.</param>
        /// <returns>A list of <see cref="MethodInfo"/> objects that can be proxied.</returns>
        public IEnumerable<MethodInfo> ChooseProxyMethodsFrom(Type baseType, IEnumerable<Type> baseInterfaces)
        {
            var results = new HashSet<MethodInfo>();

            var baseMethods = from method in baseType.GetMethods()
                              where method.IsPublic && method.IsVirtual && !method.IsFinal
                              select method;

            // Add the virtual methods defined
            // in the base type
            foreach (var method in baseMethods)
            {
                if (!results.Contains(method))
                    results.Add(method);
            }

            var interfaceMethods = from currentInterface in baseInterfaces
                                   from method in currentInterface.GetMethods()
                                   where method.IsPublic && method.IsVirtual &&
                                         !method.IsFinal && !results.Contains(method)
                                   select method;

            // Add the virtual methods defined
            // in the interface types
            foreach (var method in interfaceMethods)
            {
                results.Add(method);
            }

            return results;
        }
    }
}
