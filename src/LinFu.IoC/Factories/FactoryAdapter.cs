using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// An adapter that converts strongly-typed IFactory&lt;T&gt; 
    /// instances into an equivalent IFactory instance.
    /// </summary>
    /// <typeparam name="T">The service type to create.</typeparam>
    public class FactoryAdapter<T> : IFactory
    {
        private readonly object _factory;

        /// <summary>
        /// Creates the factory adapter using the given
        /// IFactory&lt;T&gt; instance.
        /// </summary>
        /// <param name="factory">The factory instance that
        /// will be called every time the <see cref="IFactory.CreateInstance"/> method
        /// is called. </param>
        public FactoryAdapter(object factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// The factory that will create the service instance.
        /// </summary>
        public IFactory<T> Factory
        {
            get { return _factory as IFactory<T>; }
        }

        #region IFactory Members

        /// <summary>
        /// Overridden. Uses the strongly-typed factory
        /// to create the service instance every time
        /// the <see cref="IFactory.CreateInstance"/> method 
        /// is called.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the requested service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            if (_factory == null)
                return default(T);

            var factory = _factory as IFactory<T>;
            if (factory == null)
                return default(T);

            return factory.CreateInstance(request);
        }

        #endregion
    }
}