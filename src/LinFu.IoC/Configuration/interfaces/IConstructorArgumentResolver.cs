using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a type that determines the method arguments that should be used for a given constructor.
    /// </summary>
    public interface IConstructorArgumentResolver
    {
        /// <summary>
        /// Determines the parameter values that should be used for a given constructor.
        /// </summary>
        /// <param name="constructor">The target constructor.</param>
        /// <param name="container">The host container instance.</param>
        /// <param name="additionalArguments">The list of additional arguments that should be combined with the arguments from the container.</param>
        /// <returns>A list of arguments that will be used for the given constructor.</returns>
        object[] GetConstructorArguments(ConstructorInfo constructor, IServiceContainer container,
                                         object[] additionalArguments);
    }
}
