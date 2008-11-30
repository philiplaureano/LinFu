using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// A factory that uses an existing object reference
    /// instead of creating a new service.
    /// </summary>
    public class InstanceFactory : IFactory
    {
        private readonly object _instance;

        /// <summary>
        /// Creates a factory using the existing <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The existing object reference that the factory will return.</param>
        public InstanceFactory(object instance)
        {
            _instance = instance;
        }

        #region IFactory Members

        /// <summary>
        /// A method that returns the existing object reference associated with
        /// this factory. 
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>A non-null object reference.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            return _instance;
        }

        #endregion
    }
}
