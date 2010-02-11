using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a static type that allows users to register a method replacement provider from a single location.
    /// </summary>
    public static class MethodReplacementProviderRegistry
    {
        private static readonly object _lock = new object();
        private static IMethodReplacementProvider _provider;
        private static readonly BootStrapRegistry BootStrap = BootStrapRegistry.Instance;

        /// <summary>
        /// Returns the provider that is currently attached to the registry.
        /// </summary>
        /// <param name="host">The type that is currently being intercepted.</param>
        /// <param name="info">The <see cref="IInvocationInfo"/> object that describes the invocation context.</param>
        /// <returns>A <see cref="IMethodReplacementProvider"/> that will determine the code that will be executed once a target method is called.</returns>
        public static IMethodReplacementProvider GetProvider(object host, IInvocationInfo info)
        {
            if (_provider == null)
                return null;

            return _provider.CanReplace(host, info) ? _provider : null;
        }

        /// <summary>
        /// Assigns the <paramref name="provider"/> to the MethodReplacementProvider registry.
        /// </summary>
        /// <returns>A <see cref="IMethodReplacementProvider"/> that will determine the code that will be executed once a target method is called.</returns>
        public static void SetProvider(IMethodReplacementProvider provider)
        {
            lock (_lock)
            {
                _provider = provider;
            }
        }
    }
}
