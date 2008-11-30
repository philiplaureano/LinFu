using System.Collections.Generic;
using System.Linq;

namespace LinFu.Reflection.Plugins
{
    /// <summary>
    /// A plugin class that provides the basic implementation
    /// for plugins that work with <see cref="IAssemblyTargetLoader{TTarget}"/> instances.
    /// </summary>
    /// <typeparam name="TTarget">The target type being configured.</typeparam>
    public abstract class BaseTargetLoaderPlugin<TTarget> : BaseLoaderPlugin<TTarget>,
                                                            IInitialize<ILoader<TTarget>>
    {
        #region IInitialize<ILoader<TTarget>> Members

        /// <summary>
        /// Searches the loader for an <see cref="IAssemblyTargetLoader{T}"/>
        /// instance and uses its derived classes to initialize
        /// the assembly target loader.
        /// </summary>
        /// <param name="source">The <see cref="ILoader{TTarget}"/> instance that will hold the plugin.</param>
        public void Initialize(ILoader<TTarget> source)
        {
            // Use an existing AssemblyContainerLoader
            // instance, if possible
            IAssemblyTargetLoader<TTarget> assemblyLoader = null;

            List<IAssemblyTargetLoader<TTarget>> matches = (from currentInstance in source.FileLoaders
                                                            where currentInstance != null &&
                                                                  currentInstance is IAssemblyTargetLoader<TTarget>
                                                            select (IAssemblyTargetLoader<TTarget>) currentInstance).ToList();

            if (matches.Count > 0)
                assemblyLoader = matches[0];

            // If no matches were found,
            // create the assembly target loader
            if (matches.Count == 0)
            {
                var loader = new AssemblyTargetLoader<TTarget>();
                source.FileLoaders.Add(loader);
                assemblyLoader = loader;
            }

            if (assemblyLoader == null)
                return;

            Initialize(source, assemblyLoader);
        }

        #endregion

        /// <summary>
        /// Initializes the <paramref name="loader"/> instance
        /// with the given <paramref name="assemblyLoader"/> instance.
        /// </summary>
        /// <param name="loader">The loader being configured.</param>
        /// <param name="assemblyLoader">The assembly loader that will load the types into the loader itself.</param>
        protected abstract void Initialize(ILoader<TTarget> loader,
                                           IAssemblyTargetLoader<TTarget> assemblyLoader);
    }
}