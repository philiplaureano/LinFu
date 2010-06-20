using System.Collections.Generic;
using System.Linq;

namespace LinFu.Reflection.Plugins
{
    /// <summary>
    /// A plugin class that provides the basic implementation
    /// for plugins that work with <see cref="IAssemblyTargetLoader{TTarget}"/> instances.
    /// </summary>
    /// <typeparam name="TTarget">The target type being configured.</typeparam>
    /// <typeparam name="TAssembly">The assembly type.</typeparam>
    /// <typeparam name="TType">The input type.</typeparam>
    public abstract class BaseTargetLoaderPlugin<TTarget, TAssembly, TType> : BaseLoaderPlugin<TTarget>,
                                                            IInitialize<ILoader<TTarget>>
    {
        private readonly ITypeExtractor<TAssembly, TType> _typeExtractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTargetLoaderPlugin{TTarget,TAssembly,TType}"/> class.
        /// </summary>
        /// <param name="typeExtractor">The type extractor that will pull the types out of the current assembly.</param>
        protected BaseTargetLoaderPlugin(ITypeExtractor<TAssembly, TType> typeExtractor)
        {
            _typeExtractor = typeExtractor;
        }

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
            IAssemblyTargetLoader<TTarget, TAssembly, TType> assemblyLoader = null;

            var matches = (from currentInstance in source.FileLoaders
                                                            where currentInstance != null &&
                                                                  currentInstance is IAssemblyTargetLoader<TTarget, TAssembly, TType>
                                                            select (IAssemblyTargetLoader<TTarget, TAssembly, TType>) currentInstance).ToList();

            if (matches.Count > 0)
                assemblyLoader = matches[0];

            // If no matches were found,
            // create the assembly target loader
            if (matches.Count == 0)
            {
                var loader = new AssemblyTargetLoader<TTarget, TAssembly, TType>(_typeExtractor);
                source.FileLoaders.Add(loader);
                assemblyLoader = loader;
            }

            if (assemblyLoader == null)
                return;

            Initialize(source, assemblyLoader);
        }

        /// <summary>
        /// Initializes the <paramref name="loader"/> instance
        /// with the given <paramref name="assemblyLoader"/> instance.
        /// </summary>
        /// <param name="loader">The loader being configured.</param>
        /// <param name="assemblyLoader">The assembly loader that will load the types into the loader itself.</param>
        protected abstract void Initialize(ILoader<TTarget> loader,
                                           IAssemblyTargetLoader<TTarget, TAssembly, TType> assemblyLoader);
    }
}