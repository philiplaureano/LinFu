using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a registry that allows users to statically register <see cref="ITypeActivator"/>
    /// instances.
    /// </summary>
    public static class TypeActivatorRegistry
    {
        private static readonly object _lock = new object();
        private static ITypeActivator _activator;
        private static readonly BootStrapRegistry BootStrap = BootStrapRegistry.Instance;

        /// <summary>
        /// Obtains an activator for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <see cref="ITypeActivationContext"/> instance that describes the object to be created.</param>
        /// <returns>A method activator.</returns>
        public static ITypeActivator GetActivator(ITypeActivationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            // Use the static activator by default
            var currentActivator = _activator;

            // Use the activator attached to the target if it exists
            var host = context.Target as IActivatorHost;
            if (host != null && host.Activator != null)
                currentActivator = host.Activator;

            if (currentActivator != null && currentActivator.CanActivate(context))
                return currentActivator;

            return null;
        }

        /// <summary>
        /// Sets the <see cref="ITypeActivator"/> that will be used to 
        /// instantiate object instances.
        /// </summary>
        /// <param name="activator">The <see cref="ITypeActivator"/> that will instantiate types.</param>
        public static void SetActivator(ITypeActivator activator)
        {
            lock (_lock)
            {
                _activator = activator;
            }
        }
    }
}
