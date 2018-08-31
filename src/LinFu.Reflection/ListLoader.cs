using System;
using System.Collections.Generic;
using System.IO;

namespace LinFu.Reflection
{
    /// <summary>
    ///     Represents an action loader that can load collections from types embedded within a given assembly.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    public class CollectionLoader<T> : IActionLoader<ICollection<T>, Type>
        where T : class
    {
        /// <summary>
        ///     Creates the list of actions that load the target collection into memory.
        /// </summary>
        /// <param name="input">The source type.</param>
        /// <returns>A list of actions that load the target collection into memory.</returns>
        public IEnumerable<Action<ICollection<T>>> Load(Type input)
        {
            var actionList = new List<Action<ICollection<T>>>();

            var component = (T) Activator.CreateInstance(input);
            actionList.Add(items => items.Add(component));

            return actionList;
        }

        /// <summary>
        ///     Determines whether or not the given type can be loaded into memory.
        /// </summary>
        /// <param name="inputType">The source type.</param>
        /// <returns>Returns <c>true</c> if the type can be loaded into memory; otherwise, it will return <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {
            try
            {
                if (!typeof(T).IsAssignableFrom(inputType))
                    return false;

                if (!inputType.IsClass)
                    return false;

                if (inputType.IsAbstract)
                    return false;
            }
            catch (TypeInitializationException)
            {
                // Ignore the error
                return false;
            }
            catch (FileNotFoundException)
            {
                // Ignore the error
                return false;
            }

            return true;
        }
    }
}