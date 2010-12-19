using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents an action loader that can load collections from types embedded within a given assembly.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    public class CollectionLoader<T> : IActionLoader<ICollection<T>, Type>
        where T : class
    {
        #region IActionLoader<ICollection<T>,Type> Members

        /// <summary>
        /// Creates the list of actions that load the target collection into memory.
        /// </summary>
        /// <param name="input">The source type.</param>
        /// <returns>A list of actions that load the target collection into memory.</returns>
        public IEnumerable<Action<ICollection<T>>> Load(Type input)
        {
            var defaultConstructor = (from c in input.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic)
                                     let parameterCount = c.GetParameters().Count()
                                     where parameterCount == 0
                                     select c).FirstOrDefault();

            if (defaultConstructor == null)
                yield break;

            var component = (T) Activator.CreateInstance(input);
            yield return items => items.Add(component);
        }

        /// <summary>
        /// Determines whether or not the given type can be loaded into memory.
        /// </summary>
        /// <param name="inputType">The source type.</param>
        /// <returns>Returns <c>true</c> if the type can be loaded into memory; otherwise, it will return <c>false</c>.</returns>
        public bool CanLoad(Type inputType)
        {
            try
            {
                if (!typeof (T).IsAssignableFrom(inputType))
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

        #endregion
    }
}