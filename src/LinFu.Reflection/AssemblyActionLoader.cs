using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// A class that reads an assembly and converts it into a set of actions
    /// that can be used to build the list of actions against the <typeparamref name="TTarget"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <typeparam name="TAssembly">The assembly type.</typeparam>
    /// <typeparam name="TType">The target input type.</typeparam>
    public class AssemblyActionLoader<TTarget, TAssembly, TType> : IActionLoader<IList<Action<TTarget>>, TAssembly>
    {
        private readonly Func<IList<IActionLoader<TTarget, TType>>> _getTypeLoaders;

        /// <summary>
        /// Initializes the class with a set of <see cref="IActionLoader{TTarget,Type}"/>
        /// instances that will be used to load the target assembly.
        /// </summary>
        /// <param name="getTypeLoaders">The delegate that will return the actual list of typeloaders.</param>
        /// <param name="typeExtractor">The type extractor that will be responsible for pulling the types out of the current assembly.</param>
        public AssemblyActionLoader(Func<IList<IActionLoader<TTarget, TType>>> getTypeLoaders,
                                    ITypeExtractor<TAssembly, TType> typeExtractor)
        {
            _getTypeLoaders = getTypeLoaders;
            TypeExtractor = typeExtractor;
        }

        /// <summary>
        /// The <see cref="ITypeExtractor"/> instance that will
        /// determine which types will be extracted from an assembly.
        /// </summary>
        public ITypeExtractor<TAssembly, TType> TypeExtractor { get; set; }

        #region IActionLoader<IList<Action<TTarget>>,TAssembly> Members

        /// <summary>
        /// Loads the target assembly and creates an action that can
        /// create the list of actions which will modify the <typeparamref name="TTarget"/> instance.
        /// </summary>
        /// <param name="input">The target assembly.</param>
        /// <returns>The list of actions which will modify the list of actions that will be executed against the <typeparamref name="TTarget"/> instance.</returns>
        public IEnumerable<Action<IList<Action<TTarget>>>> Load(TAssembly input)
        {
            yield return list => CreateActionsFrom(input, list);
        }

        /// <summary>
        /// Determines if an assembly can be loaded.
        /// </summary>
        /// <param name="assembly">The target assembly.</param>
        /// <returns>Returns <c>true</c> if the assembly is not <c>null</c>.</returns>
        public virtual bool CanLoad(TAssembly assembly)
        {
            return assembly != null;
        }

        #endregion

        /// <summary>
        /// Generates a list of actions from a target assemby.
        /// </summary>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="resultList">The list that will store the resulting actions.</param>
        private void CreateActionsFrom(TAssembly assembly, ICollection<Action<TTarget>> resultList)
        {
            // Grab the types embedded in the assembly
            IEnumerable<TType> types = new TType[0];
            if (assembly != null && TypeExtractor != null)
                types = TypeExtractor.GetTypes(assembly);

            // Pass the loaded types to
            // the type loaders for processing
            foreach (TType type in types)
            {
                // Skip any invalid types
                if (type == null)
                    continue;

                LoadResultsFromType(type, resultList);
            }
        }

        /// <summary>
        /// Generates the list of <see cref="Action{TTarget}"/>
        /// instances which will be used to configure a target instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance that holds the configuration information.</param>
        /// <param name="results">The list that will hold the actions which will configure the container.</param>
        private void LoadResultsFromType(TType type, ICollection<Action<TTarget>> results)
        {
            IList<IActionLoader<TTarget, TType>> typeLoaders = _getTypeLoaders();
            foreach (var typeLoader in typeLoaders)
            {
                if (typeLoader == null || !typeLoader.CanLoad(type))
                    continue;

                IEnumerable<Action<TTarget>> actions = typeLoader.Load(type);
                if (actions.Count() == 0)
                    continue;

                foreach (var action in actions)
                {
                    results.Add(action);
                }
            }
        }
    }

    /// <summary>
    /// A class that reads an assembly and converts it into a set of actions
    /// that can be used to build the list of actions against the <typeparamref name="TTarget"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public class AssemblyActionLoader<TTarget> : AssemblyActionLoader<TTarget, Assembly, Type>
    {
        /// <summary>
        /// Initializes the class with a set of <see cref="IActionLoader{TTarget,Type}"/>
        /// instances that will be used to load the target assembly.
        /// </summary>
        /// <param name="getTypeLoaders">The delegate that will return the actual list of typeloaders.</param>
        public AssemblyActionLoader(Func<IList<IActionLoader<TTarget, Type>>> getTypeLoaders)
            : base(getTypeLoaders, new TypeExtractor())
        {
        }
    }
}