using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a registry class that bootstraps components into memory when the application starts.
    /// </summary>
    public sealed class BootStrapRegistry
    {
        private readonly IList<IBootStrappedComponent> _components = new List<IBootStrappedComponent>();

        private BootStrapRegistry()
        {
            Initialize();
        }

        /// <summary>
        /// Gets the value indicating the BootStrapRegistry instance.
        /// </summary>
        public static BootStrapRegistry Instance
        {
            get
            {
                return NestedLoader.Instance;
            }
        }

        /// <summary>
        /// Initializes the BootStrapRegistry.
        /// </summary>
        private void Initialize()
        {
            lock (_components)
            {
                _components.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
                foreach(var component in _components)
                {
                    component.Initialize();
                }
            }
        }

        /// <summary>
        /// Returns the list of components that have been initialized by the bootstrapper.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBootStrappedComponent> GetComponents()
        {
            return _components;
        }

        private class NestedLoader
        {
            internal static readonly BootStrapRegistry Instance = new BootStrapRegistry();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NestedLoader()
            {
            }
        }

    }
}
