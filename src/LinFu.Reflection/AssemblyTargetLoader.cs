using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a loader class that takes <see cref="System.Type"/>
    /// instances as input and generates <see cref="Action{T}"/>
    /// instances that can be used to configure a <typeparamref name="TTarget"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    /// <typeparam name="TAssembly">The assembly type.</typeparam>
    /// <typeparam name="TType">The target type.</typeparam>
    public class AssemblyTargetLoader<TTarget, TAssembly, TType> : IAssemblyTargetLoader<TTarget, TAssembly, TType>,
                                                                   IActionLoader<TTarget, string>
    {
        private readonly IList<IActionLoader<TTarget, TType>> _typeLoaders = new List<IActionLoader<TTarget, TType>>();
        private IActionLoader<IList<Action<TTarget>>, TAssembly> _assemblyActionLoader;
        private IAssemblyLoader<TAssembly> _assemblyLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTargetLoader{TTarget,TAssembly,TType}"/> class.
        /// </summary>
        public AssemblyTargetLoader(ITypeExtractor<TAssembly, TType> typeExtractor,
                                    IAssemblyLoader<TAssembly> assemblyLoader)
        {
            _assemblyActionLoader = new AssemblyActionLoader<TTarget, TAssembly, TType>(() => TypeLoaders, typeExtractor);
            _assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Gets or sets the value indicating the action loader 
        /// responsible for reading an assembly and converts it to 
        /// a list of actions to be performed against the target type.
        /// </summary>
        public virtual IActionLoader<IList<Action<TTarget>>, TAssembly> AssemblyActionLoader
        {
            get { return _assemblyActionLoader; }
            set { _assemblyActionLoader = value; }
        }

        #region IAssemblyTargetLoader<TTarget,TAssembly,TType> Members

        /// <summary>
        /// The <see cref="IAssemblyLoader"/> instance that will load
        /// the target assemblies.
        /// </summary>
        public virtual IAssemblyLoader<TAssembly> AssemblyLoader
        {
            get { return _assemblyLoader; }
            set { _assemblyLoader = value; }
        }

        /// <summary>
        /// The list of ActionLoaders that will be used to
        /// configure the target.
        /// </summary>
        public virtual IList<IActionLoader<TTarget, TType>> TypeLoaders
        {
            get { return _typeLoaders; }
        }

        /// <summary>
        /// Determines whether or not the current type loader
        /// instance can load the current file type.
        /// </summary>
        /// <remarks>
        /// This class only loads assemblies with the ".dll" extension.
        /// </remarks>
        /// <param name="filename">The filename and full path of the target file.</param>
        /// <returns>Returns <c>true</c> if the file can be loaded; otherwise, the result is <c>false</c>.</returns>
        public virtual bool CanLoad(string filename)
        {
            var extension = Path.GetExtension(filename).ToLower();
            return TypeLoaders.Count > 0 &&
                   (extension == ".dll" || extension == ".exe") &&
                   File.Exists(filename);
        }

        /// <summary>
        /// Reads an input file using the given <paramref name="filename"/>
        /// and converts it into a set of <see cref="Action{TTarget}"/>
        /// instances that can be applied to a target class instance..
        /// </summary>
        /// <remarks>This class can only load valid .NET Assemblies.</remarks>
        /// <param name="filename">The target file to be loaded.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances to apply to a target type.</returns>
        public virtual IEnumerable<Action<TTarget>> Load(string filename)
        {
            var assembly = default(TAssembly);

            if (AssemblyLoader == null)
                throw new ArgumentException("The assembly loader cannot be null");

            // Load the assembly into memory
            assembly = AssemblyLoader.Load(filename);

            var results = new List<Action<TTarget>>();
            var listActions = AssemblyActionLoader.Load(assembly);
            foreach (var action in listActions)
            {
                action(results);
            }

            return results;
        }

        #endregion
    }

    /// <summary>
    /// Represents a loader class that takes <see cref="System.Type"/>
    /// instances as input and generates <see cref="Action{T}"/>
    /// instances that can be used to configure a <typeparamref name="TTarget"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    public class AssemblyTargetLoader<TTarget> : AssemblyTargetLoader<TTarget, Assembly, Type>
    {
        /// <summary>
        /// Initializes the class with the default property values.
        /// </summary>
        public AssemblyTargetLoader()
            : base(new TypeExtractor(), new AssemblyLoader())
        {
        }
    }
}