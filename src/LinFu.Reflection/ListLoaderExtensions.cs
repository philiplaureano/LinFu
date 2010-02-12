using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents an extension class that adds methods that make it easier to load types into memory.
    /// </summary>
    public static class ListLoaderExtensions
    {
        /// <summary>
        /// Loads a list of types that match the given <typeparamref name="T">target type</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The target type to be loaded.</typeparam>
        /// <param name="list">The list that will hold the instances of the target type.</param>
        /// <param name="targetDirectory">The directory that will be used to scan for assemblies that contain the target type.</param>
        public static void LoadFrom<T>(this IList<T> list, string targetDirectory)
            where T : class
        {            
            if (list == null)
                throw new ArgumentNullException("list");

            var loader = new Loader<IList<T>>();
            var targetLoader = new AssemblyTargetLoader<IList<T>>();
            targetLoader.TypeLoaders.Add(new ListLoader<T>());
            loader.FileLoaders.Add(targetLoader);
            loader.LoadDirectory(targetDirectory, "*.dll");
            loader.LoadInto(list);
        }
    }
}
