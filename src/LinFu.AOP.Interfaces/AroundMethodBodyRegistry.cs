using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a registry class that handles all class-level interception operations for all modified types.
    /// </summary>
    public static class AroundMethodBodyRegistry
    {
        private static readonly List<IAroundInvokeProvider> _providers = new List<IAroundInvokeProvider>();
        private static readonly object _lock = new object();
        private static readonly BootStrapRegistry _registry = BootStrapRegistry.Instance;

        /// <summary>
        /// Obtains the <see cref="IAroundInvoke"/> instance for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <see cref="IInvocationInfo"/> instance that describes the current method call.</param>
        /// <returns>An <see cref="IAroundInvoke"/> instance that will be used to wrap a method call or method body.</returns>
        public static IAroundInvoke GetSurroundingImplementation(IInvocationInfo context)
        {
            var resultList = (from p in _providers
                              where p != null
                              let aroundInvoke = p.GetSurroundingImplementation(context)
                              where aroundInvoke != null
                              select aroundInvoke).ToList();

            if (resultList.Count == 0)
                return null;

            return new CompositeAroundInvoke(resultList);
        }

        /// <summary>
        /// Adds an <see cref="IAroundInvokeProvider"/> to the list of provider instances.
        /// </summary>
        /// <param name="provider">The <see cref="IAroundInvokeProvider"/> instance.</param>
        public static void AddProvider(IAroundInvokeProvider provider)
        {
            lock (_lock)
            {
                _providers.Add(provider);
            }
        }

        /// <summary>
        /// Clears the list of <see cref="IAroundInvokeProvider"/> instances.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _providers.Clear();
            }
        }
    }
}
