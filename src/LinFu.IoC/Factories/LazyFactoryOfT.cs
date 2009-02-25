using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// Represents a factory that returns strongly-typed IFactory instances.
    /// </summary>
    /// <typeparam name="T">The service type to be created.</typeparam>
    public class LazyFactory<T> : IFactory<T>, IFactory
    {
        private Func<IFactoryRequest, IFactory> _getFactory;

        /// <summary>
        /// Initializes the factory with the given <paramref name="getFactory"/> functor.
        /// </summary>
        /// <param name="getFactory">The functor that will instantiate the actual factory instance.</param>
        public LazyFactory(Func<IFactoryRequest, IFactory> getFactory)
        {
            _getFactory = getFactory;
        }

        /// <summary>
        /// Instantiates the service type using the actual factory.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> instance that describes the service to be created.</param>
        /// <returns></returns>
        public T CreateInstance(IFactoryRequest request)
        {
            if (_getFactory == null)
                throw new NotImplementedException();

            var factory = _getFactory(request) as IFactory<T>;

            if (factory == null)
                return default(T);

            return factory.CreateInstance(request);
        }

        object IFactory.CreateInstance(IFactoryRequest request)
        {
            IFactory<T> thisFactory = this;
            return thisFactory.CreateInstance(request);
        }
    }
}
