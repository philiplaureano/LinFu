using System;
using System.Reflection;

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
        private readonly IAssemblyTargetLoader<TTarget, TAssembly, TType> _assemblyLoader;
        private readonly ITypeExtractor<TAssembly, TType> _typeExtractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTargetLoaderPlugin{TTarget,TAssembly,TType}"/> class.
        /// </summary>
        /// <param name="typeExtractor">The type extractor that will pull the types out of the current assembly.</param>
        /// <param name="assemblyLoader">The assembly loader that will load the current assembly into memory.</param>
        protected BaseTargetLoaderPlugin(ITypeExtractor<TAssembly, TType> typeExtractor,
                                         IAssemblyTargetLoader<TTarget, TAssembly, TType> assemblyLoader)
        {
            _typeExtractor = typeExtractor;
            _assemblyLoader = assemblyLoader;
        }

        #region IInitialize<ILoader<TTarget>> Members

        /// <summary>
        /// Searches the loader for an <see cref="IAssemblyTargetLoader{T}"/>
        /// instance and uses its derived classes to initialize
        /// the assembly target loader.
        /// </summary>
        /// <param name="source">The <see cref="ILoader{TTarget}"/> instance that will hold the plugin.</param>
        public void Initialize(ILoader<TTarget> source)
        {
            Initialize(source, _assemblyLoader);
        }

        #endregion

        /// <summary>
        /// Initializes the <paramref name="loader"/> instance
        /// with the given <paramref name="assemblyLoader"/> instance.
        /// </summary>
        /// <param name="loader">The loader being configured.</param>
        /// <param name="assemblyLoader">The assembly loader that will load the types into the loader itself.</param>
        protected abstract void Initialize(ILoader<TTarget> loader,
                                           IAssemblyTargetLoader<TTarget, TAssembly, TType> assemblyLoader);
    }

    /// <summary>
    /// A plugin class that provides the basic implementation
    /// for plugins that work with <see cref="IAssemblyTargetLoader{TTarget}"/> instances.
    /// </summary>
    public abstract class BaseTargetLoaderPlugin<TTarget> : BaseTargetLoaderPlugin<TTarget, Assembly, Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTargetLoaderPlugin{TTarget,TAssembly,TType}"/> class.
        /// </summary>
        protected BaseTargetLoaderPlugin() : this(new TypeExtractor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTargetLoaderPlugin{TTarget,TAssembly,TType}"/> class.
        /// </summary>
        /// <param name="typeExtractor">The type extractor that will pull the types out of the current assembly.</param>
        protected BaseTargetLoaderPlugin(ITypeExtractor<Assembly, Type> typeExtractor) :
            base(typeExtractor, new AssemblyTargetLoader<TTarget, Assembly, Type>(typeExtractor, new AssemblyLoader()))
        {
        }
    }
}