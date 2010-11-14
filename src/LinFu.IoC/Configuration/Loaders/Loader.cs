using System;
using System.Reflection;
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
        private IAssemblyLoader<Assembly> _assemblyLoader;

        /// <summary>
        /// Initializes the loader using the default values.
        /// </summary>
        public Loader() 
        {
            Func<AssemblyContainerLoader> getContainerLoader = () => this.CreateDefaultContainerLoader();
            var containerLoader = getContainerLoader();
            Initialize(containerLoader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Loader"/> class.
        /// </summary>
        /// <param name="getContainerLoader">The factory method that will create the loader itself.</param>
        public Loader(Func<AssemblyContainerLoader> getContainerLoader)
        {
            var containerLoader = getContainerLoader();
            Initialize(containerLoader);
        }
        /// <summary>
        /// Initializes the target with the default settings.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader that will load the assemblies into the loader itself.</param>
        public Loader(IAssemblyLoader<Assembly> assemblyLoader)
        {
            _assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Gets or sets the value indicating the <see cref="IAssemblyLoader"/> instance
        /// that will be used to load assemblies into memory.
        /// </summary>
        public IAssemblyLoader<Assembly> AssemblyLoader
        {
            get { return _assemblyLoader; }
            set { _assemblyLoader = value; }
        }

        /// <summary>
        /// Initializes the loader with the default configuration.
        /// </summary>
        /// <param name="containerLoader">The container loader instance.</param>
        private void Initialize(AssemblyContainerLoader containerLoader)
        {
            _assemblyLoader = containerLoader.AssemblyLoader;

            // Load everything else into the container
            Assembly hostAssembly = typeof(Loader).Assembly;
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

            FileLoaders.Add(containerLoader);
        }
    }
}