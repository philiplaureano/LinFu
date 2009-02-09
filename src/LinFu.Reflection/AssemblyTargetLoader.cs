using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class AssemblyTargetLoader<TTarget> : IAssemblyTargetLoader<TTarget>
    {
        private readonly IList<IActionLoader<TTarget, Type>> typeLoaders = new List<IActionLoader<TTarget, Type>>();

        /// <summary>
        /// Initializes the class with the default property values.
        /// </summary>
        public AssemblyTargetLoader()
        {
            AssemblyLoader = new AssemblyLoader();
            AssemblyActionLoader = new AssemblyActionLoader<TTarget>(() => TypeLoaders);
        }

        /// <summary>
        /// The <see cref="IAssemblyLoader"/> instance that will load
        /// the target assemblies.
        /// </summary>
        public virtual IAssemblyLoader AssemblyLoader { get; set; }


        /// <summary>
        /// Gets or sets the value indicating the action loader 
        /// responsible for reading an assembly and converts it to 
        /// a list of actions to be performed against the target type.
        /// </summary>
        public virtual IActionLoader<IList<Action<TTarget>>, Assembly> AssemblyActionLoader
        {
            get;
            set;
        }

        /// <summary>
        /// The list of ActionLoaders that will be used to
        /// configure the target.
        /// </summary>
        public virtual IList<IActionLoader<TTarget, Type>> TypeLoaders
        {
            get { return typeLoaders; }
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
            return TypeLoaders.Count > 0 &&
                   Path.GetExtension(filename).ToLower() == ".dll" &&
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
            Assembly assembly = null;

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
    }
}