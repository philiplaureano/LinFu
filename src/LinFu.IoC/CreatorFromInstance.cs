using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents an <see cref="ICreateInstance"/> type that generates an object instance from an existing instance.
    /// </summary>
    internal class CreatorFromInstance : ICreateInstance
    {
        private readonly object _instance;

        /// <summary>
        /// Initializes the class with the target <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance that will be returned every time the <see cref="CreateFrom"/> method is called.</param>
        internal CreatorFromInstance(object instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// Returns the object instance that given when the <see cref="CreatorFromInstance"/> class instance was initialized.
        /// </summary>
        /// <param name="factoryRequest">The <see cref="IFactoryRequest"/> instance that describes the context of the service request.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used to instantiate the service type.</param>
        /// <returns>A valid service instance.</returns>
        public object CreateFrom(IFactoryRequest factoryRequest, IFactory factory)
        {
            return _instance;
        }
    }
}
