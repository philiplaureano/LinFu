using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a type that can generate method arguments
    /// from an existing <see cref="IServiceContainer"/> instance.
    /// </summary>
    public interface IArgumentResolver
    {
        /// <summary>
        /// Generates constructor arguments from the given <paramref name="parameterTypes"/>
        /// and <paramref name="container"/>.
        /// </summary>
        /// <param name="parameterTypes">The parameter types for the target method.</param>
        /// <param name="container">The container that will provide the method arguments.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the target method.</param>
        /// <returns>An array of objects that represent the arguments to be passed to the target method.</returns>
        object[] ResolveFrom(IEnumerable<Type> parameterTypes, IServiceContainer container, params object[] additionalArguments);
    }
}