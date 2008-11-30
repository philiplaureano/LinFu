using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a class that is responsible for generating methods
    /// from other existing methods.
    /// </summary>
    /// <typeparam name="TMethod">The method type to generate.</typeparam>
    public interface IMethodBuilder<TMethod>
        where TMethod : MethodBase
    {
        /// <summary>
        /// Creates a method from the <paramref name="existingMethod"/>.
        /// </summary>
        /// <param name="existingMethod">The method that will be used to define the new method.</param>
        /// <returns>A method based on the old method.</returns>
        MethodBase CreateMethod(TMethod existingMethod);
    }
}
