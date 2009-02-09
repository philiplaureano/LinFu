using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Configuration.Loaders;
using LinFu.IoC.Interceptors;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can dynamically configure
    /// <see cref="IServiceContainer"/> instances at runtime.
    /// </summary>
    public class Loader : Loader<IServiceContainer>
    {
        private readonly AssemblyContainerLoader _containerLoader;

        /// <summary>
        /// Initializes the loader using the default values.
        /// </summary>
        public Loader()
        {
            _containerLoader = this.CreateDefaultContainerLoader();

            // Load everything else into the container
            var hostAssembly = typeof(Loader).Assembly;
            QueuedActions.Add(container => container.LoadFrom(hostAssembly));
            
            // Make sure that the plugins are only added once
            if (!Plugins.HasElementWith(p => p is AutoPropertyInjector))
                Plugins.Add(new AutoPropertyInjector());

            if (!Plugins.HasElementWith(p => p is AutoMethodInjector))
                Plugins.Add(new AutoMethodInjector());

            if (!Plugins.HasElementWith(p => p is AutoFieldInjector))
                Plugins.Add(new AutoFieldInjector());

            // Add the initializer to the end of
            // the instantiation pipeline
            if (!Plugins.HasElementWith(p => p is InitializerPlugin))
                Plugins.Add(new InitializerPlugin());

            FileLoaders.Add(_containerLoader);
        }
     
        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAssemblyLoader"/> instance
        /// that will be used to load assemblies into memory.
        /// </summary>
        public IAssemblyLoader AssemblyLoader
        {
            get
            {
                return _containerLoader.AssemblyLoader;
            }
            set
            {
                _containerLoader.AssemblyLoader = value;
            }
        }
    }
}
