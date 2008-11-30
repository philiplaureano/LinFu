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
    }
}
