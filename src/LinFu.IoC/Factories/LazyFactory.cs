using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// Represents an <see cref="IFactory"/> class that instantiates a factory only on request.
    /// </summary>
    public class LazyFactory : IFactory
    {
        private readonly Func<IFactoryRequest, IFactory> _getFactory;
        private IFactory _realFactory;

        /// <summary>
        /// Instantiates the class with the factory functor method.
        /// </summary>
        /// <param name="getFactory">The functor that will be responsible for instantiating the actual factory.</param>
        public LazyFactory(Func<IFactoryRequest, IFactory> getFactory)
        {
            _getFactory = getFactory;
        }

        /// <summary>
        /// Instantiates the actual factory instance and uses it to instantiate the target service type.
        /// </summary>
        /// <param name="request">The <see cref="IFactoryRequest"/> that describes the service to be created.</param>
        /// <returns>A valid service instance.</returns>
        public object CreateInstance(IFactoryRequest request)
        {
            // Create the factory if necessary
            if (_realFactory == null)
                _realFactory = _getFactory(request);

            return _realFactory == null ? null : _realFactory.CreateInstance(request);
        }
    }
}
