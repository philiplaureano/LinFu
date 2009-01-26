using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a class that can instantiate object instances.
    /// </summary>
    public interface IActivator
    {
        /// <summary>
        /// Creates an object instance.
        /// </summary>
        /// <param name="concreteType">The <see cref="System.Type"/> to be instantiated.</param>
        /// <param name="container">The <see cref="IServiceContainer"/> instance that will be used to instantiate the concrete type.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the target type.</param>
        /// <returns>A valid object instance.</returns>
        object CreateInstance(Type concreteType, IServiceContainer container, object[] additionalArguments);
    }
}
