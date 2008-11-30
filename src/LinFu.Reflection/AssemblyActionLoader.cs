using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// A class that reads an assembly and converts it into a set of actions
    /// that can be used to build the list of actions against the <typeparamref name="TTarget"/>
    /// type.
    /// </summary>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public class AssemblyActionLoader<TTarget> : IActionLoader<IList<Action<TTarget>>, Assembly>
    {
        private readonly Func<IList<IActionLoader<TTarget, Type>>> _getTypeLoaders;

        /// <summary>
        /// Initializes the class with a set of <see cref="IActionLoader{TTarget,Type}"/>
        /// instances that will be used to load the target assembly.
        /// </summary>
        /// <param name="getTypeLoaders">The delegate that will return the actual list of typeloaders.</param>
        public AssemblyActionLoader(Func<IList<IActionLoader<TTarget, Type>>> getTypeLoaders)
        {
            _getTypeLoaders = getTypeLoaders;
            TypeExtractor = new TypeExtractor();
        }

        /// <summary>
        /// Loads the target assembly and creates an action that can
        /// create the list of actions which will modify the <typeparamref name="TTarget"/> instance.
        /// </summary>
        /// <param name="input">The target assembly.</param>
        /// <returns>The list of actions which will modify the list of actions that will be executed against the <typeparamref name="TTarget"/> instance.</returns>
        public IEnumerable<Action<IList<Action<TTarget>>>> Load(Assembly input)
        {
            yield return list => CreateActionsFrom(input, list);
        }

        /// <summary>
        /// Determines if an <see cref="Assembly"/> instance can be loaded.
        /// </summary>
        /// <param name="assembly">The target assembly.</param>
        /// <returns>Returns <c>true</c> if the assembly is not <c>null</c>.</returns>
        public virtual bool CanLoad(Assembly assembly)
        {
            return assembly != null;
        }

        /// <summary>
        /// The <see cref="ITypeExtractor"/> instance that will
        /// determine which types will be extracted from an assembly.
        /// </summary>
        public ITypeExtractor TypeExtractor { get; set; }

        /// <summary>
        /// Generates a list of actions from a target assemby.
        /// </summary>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="resultList">The list that will store the resulting actions.</param>
        private void CreateActionsFrom(Assembly assembly, IList<Action<TTarget>> resultList)
        {
            // Grab the types embedded in the assembly
            IEnumerable<Type> types = new Type[0];
            if (assembly != null && TypeExtractor != null)
                types = TypeExtractor.GetTypes(assembly);

            // Pass the loaded types to
            // the type loaders for processing
            foreach (var type in types)
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
        private void LoadResultsFromType(Type type, ICollection<Action<TTarget>> results)
        {
            var typeLoaders = _getTypeLoaders();
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
}
