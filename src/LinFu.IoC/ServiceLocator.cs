using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;
using FactoryStorageExtensions = LinFu.IoC.FactoryStorageExtensions;

namespace LinFu.IoC
{
    public abstract class ServiceLocator : IServiceLocator
    {
        private readonly IGetService _getServiceBehavior;
        private readonly IFactoryStorage _factoryStorage;

        protected ServiceLocator()
        {
            _getServiceBehavior = GetDefaultGetServiceBehavior();
            _factoryStorage = GetDefaultFactoryStorage();
        }

        protected ServiceLocator(IGetService getServiceBehavior, IFactoryStorage factoryStorage)
        {
            if (getServiceBehavior == null)
                throw new ArgumentNullException("getServiceBehavior");

            if (factoryStorage == null)
                throw new ArgumentNullException("factoryStorage");

            _getServiceBehavior = getServiceBehavior;
            _factoryStorage = factoryStorage;
        }

        /// <summary>
        /// Determines whether or not the given <paramref name="serviceType"/>
        /// can be instantiated by the container.
        /// </summary>
        /// <param name="serviceType">The type of service to instantiate.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(Type serviceType, IEnumerable<Type> additionalParameterTypes)
        {
            return Contains(null, serviceType, additionalParameterTypes);
        }

        /// <summary>
        /// Overridden. Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <remarks>
        /// This overload of the <c>GetService</c> method has been overridden
        /// so that its results can be handled by the postprocessors.
        /// </remarks>
        /// <seealso cref="IPostProcessor"/>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        public object GetService(Type serviceType, params object[] additionalArguments)
        {
            return GetService(null, serviceType, additionalArguments);
        }

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>        
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public virtual object GetService(string serviceName, Type serviceType, params object[] additionalArguments)
        {
            IFactory factory = null;

            if (FactoryStorage != null)
                factory = FactoryStorage.GetFactory(serviceName, serviceType, additionalArguments);

            var serviceRequest = CreateRequest(serviceName, serviceType, additionalArguments, factory);
            object instance = _getServiceBehavior.GetService(serviceRequest);

            if (SuppressErrors == false && instance == null && serviceName == null)
                throw new ServiceNotFoundException(serviceType);

            if (SuppressErrors == false && instance == null && serviceName != null)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            return instance;
        }       

        /// <summary>
        /// Determines whether or not a service can be created using
        /// the given <paramref name="serviceName">service name</paramref>
        /// and <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="additionalParameterTypes">The list of additional parameters that this factory type will support.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(string serviceName, Type serviceType, IEnumerable<Type> additionalParameterTypes)
        {
            if (_factoryStorage==null)
                return false;

            return FactoryStorage.ContainsFactory(serviceName, serviceType, additionalParameterTypes);
        }

        /// <summary>
        /// Gets or sets a <see cref="bool">System.Boolean</see> value
        /// that determines whether or not the container should throw
        /// a <see cref="ServiceNotFoundException"/> if a requested service
        /// cannot be found or created.
        /// </summary>
        public virtual bool SuppressErrors { get; set; }

        /// <summary>
        /// Gets the value indicating the <see cref="IFactoryStorage"/> instance
        /// that will be used to store each <see cref="IFactory"/> instance.
        /// </summary>
        internal IFactoryStorage FactoryStorage
        {
            get { return _factoryStorage; }
        }

        protected abstract IGetService GetDefaultGetServiceBehavior();
        protected abstract IFactoryStorage GetDefaultFactoryStorage();
        protected abstract IServiceRequest CreateRequest(string serviceName, Type serviceType,
                                                         object[] additionalArguments, IFactory factory);
    }
}