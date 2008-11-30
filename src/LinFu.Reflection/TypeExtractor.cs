using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a type that can extract <see cref="System.Type"/>
    /// objects from an <see cref="Assembly"/> instance.
    /// </summary>
    public class TypeExtractor : ITypeExtractor
    {
        #region ITypeExtractor Members

        /// <summary>
        /// Returns a set of types from a given assembly.
        /// </summary>
        /// <param name="targetAssembly">The <see cref="Assembly"/> that contains the target types.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of types from the target assembly.</returns>
        public IEnumerable<Type> GetTypes(Assembly targetAssembly)
        {
            Type[] loadedTypes = null;
            try
            {
                loadedTypes = targetAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                loadedTypes = ex.Types;
            }
            return loadedTypes;
        }

        #endregion
    }
}