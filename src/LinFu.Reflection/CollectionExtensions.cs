using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// Adds additional support methods to the standard System.Collection classes.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Determines whether or not an element exists that matches the given
        /// <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="items">The list of items being searched.</param>
        /// <param name="predicate">The predicate that will be used to describe the matching items.</param>
        /// <returns>Returns <c>true</c> if at least one match is found; otherwise, it will return <c>false</c>.</returns>
        public static bool HasElementWith<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var matches = from item in items
                          where predicate(item)
                          select item;

            return matches.Count() > 0;
        }

        /// <summary>
        /// Loads a list of types that match the given <typeparamref name="T">target type</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The target type to be loaded.</typeparam>
        /// <param name="list">The list that will hold the instances of the target type.</param>
        /// <param name="targetDirectory">The directory that will be used to scan for assemblies that contain the target type.</param>
        /// <param name="filespec">The wildcard pattern that describes the files to be loaded.</param>
        public static void LoadFrom<T>(this ICollection<T> list, string targetDirectory, string filespec)
            where T : class
        {
            if (list == null)
                throw new ArgumentNullException("list");

            var loader = new Loader<ICollection<T>>();
            var targetLoader = new AssemblyTargetLoader<ICollection<T>>();
            targetLoader.TypeLoaders.Add(new CollectionLoader<T>());
            loader.FileLoaders.Add(targetLoader);
            loader.LoadDirectory(targetDirectory, filespec);
            loader.LoadInto(list);
        }
    }
}
