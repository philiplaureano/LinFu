using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a service container with additional
    /// extension points for customizing service instances
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        private readonly IFactoryStorage _factoryStorage = new FactoryStorage();
        private readonly List<IPostProcessor> _postProcessors = new List<IPostProcessor>();
        private readonly List<IPreProcessor> _preprocessors = new List<IPreProcessor>();

        /// <summary>
        /// Initializes the container with the default services.
        /// </summary>
        public ServiceContainer()
        {
            this.AddDefaultServices();
        }

        /// <summary>
        /// Gets or sets a <see cref="bool">System.Boolean</see> value
        /// that determines whether or not the container should throw
        /// a <see cref="ServiceNotFoundException"/> if a requested service
        /// cannot be found or created.
        /// </summary>
        public virtual bool SuppressErrors { get; set; }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public virtual void AddFactory(string serviceName, Type serviceType, IFactory factory)
        {
            FactoryStorage.AddFactory(serviceName, serviceType, factory);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The service type to associate with the factory</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be responsible for creating the service instance</param>
        public virtual void AddFactory(Type serviceType, IFactory factory)
        {
            FactoryStorage.AddFactory(null, serviceType, factory);
        }

        /// <summary>
        /// Determines whether or not the given <paramref name="serviceType"/>
        /// can be instantiated by the container.
        /// </summary>
        /// <param name="serviceType">The type of service to instantiate.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(Type serviceType)
        {
            return Contains(null, serviceType);
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
            object instance = null;
            var suppressErrors = SuppressErrors;

            // Attempt to create the service type using
            // the generic factories, if possible
            var factory = FactoryStorage.GetFactory(serviceName, serviceType);

            // Attempt to create the service type using
            // the generic factories, if possible
            if (factory == null && serviceType.IsGenericType)
            {
                var definitionType = serviceType.GetGenericTypeDefinition();
                factory = FactoryStorage.GetFactory(serviceName, definitionType);
            }

            // Allow users to intercept the instantiation process
            IServiceRequest serviceRequest = Preprocess(serviceName, serviceType, additionalArguments, factory);

            factory = serviceRequest.ActualFactory;
            var actualArguments = serviceRequest.ActualArguments;


            var factoryRequest = new FactoryRequest
            {
                ServiceType = serviceType,
                ServiceName = serviceName,
                Arguments = actualArguments,
                Container = this
            };

            // Generate the service instance
            if (factory != null)
                instance = factory.CreateInstance(factoryRequest);

            IServiceRequestResult result = PostProcess(serviceName, serviceType, instance, actualArguments);

            // Use the modified result, if possible; otherwise,
            // use the original result.
            instance = result.ActualResult ?? result.OriginalResult;

            if (suppressErrors == false && instance == null && serviceName == null)
                throw new ServiceNotFoundException(serviceType);

            if (suppressErrors == false && instance == null && serviceName != null)
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
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(string serviceName, Type serviceType)
        {
            // Use the default implementation for
            // non-generic types
            if (!serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
                return FactoryStorage.ContainsFactory(serviceName, serviceType);

            // If the service type is a generic type, determine
            // if the service type can be created by a 
            // standard factory that can create an instance
            // of that generic type (e.g., IFactory<IGeneric<T>>            
            var result = FactoryStorage.ContainsFactory(serviceName, serviceType);

            // Immediately return a positive match, if possible
            if (result)
                return true;

            if (serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
            {
                // Determine the base type definition
                var baseDefinition = serviceType.GetGenericTypeDefinition();

                // Check if there are any generic factories that can create
                // the entire family of services whose type definitions
                // match the base type
                result = FactoryStorage.ContainsFactory(serviceName, baseDefinition);
            }

            return result;
        }

        /// <summary>
        /// A method that searches the container for <see cref="IPreProcessor"/> instances
        /// and passes the service request to each one of those preprocessors.
        /// </summary>
        /// <param name="serviceName">The name of the service being requested. By default, this is usually blank.</param>
        /// <param name="serviceType">The type of service being requested.</param>        
        /// <param name="additionalArguments">The list of additional arguments that will be used for the service request.</param>
        /// <param name="proposedFactory">The <see cref="IFactory"/> instance that will be used to create the service instance.</param>
        /// <returns>A <see cref="IServiceRequest"/> object that describes which factory should be used to handle the service request.</returns>
        private IServiceRequest Preprocess(string serviceName, Type serviceType, object[] additionalArguments, IFactory proposedFactory)
        {
            var serviceRequest = new ServiceRequest(serviceName, serviceType, additionalArguments, proposedFactory, this);
            foreach (var preprocessor in PreProcessors)
            {
                preprocessor.Preprocess(serviceRequest);
            }
            return serviceRequest;
        }

        /// <summary>
        /// A method that searches the current container for
        /// postprocessors and passes every request result made
        /// to the list of <see cref="IServiceContainer.PostProcessors"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service being requested. By default, this is usually blank.</param>
        /// <param name="serviceType">The type of service being requested.</param>
        /// <param name="instance">The original instance returned by container's service instantiation attempt.</param>
        /// <param name="additionalArguments">The list of additional arguments that were used during the service request.</param>
        /// <returns>A <see cref="IServiceRequestResult"/> representing the results returned as a result of the postprocessors.</returns>
        private IServiceRequestResult PostProcess(string serviceName, Type serviceType, object instance, object[] additionalArguments)
        {
            // Initialize the results
            var result = new ServiceRequestResult
            {
                ServiceName = serviceName,
                ActualResult = instance,
                Container = this,
                OriginalResult = instance,
                ServiceType = serviceType,
                AdditionalArguments = additionalArguments
            };

            // Let each postprocessor inspect 
            // the results and/or modify the 
            // returned object instance
            var postprocessors = PostProcessors.ToArray();
            foreach (IPostProcessor postProcessor in postprocessors)
            {
                if (postProcessor == null)
                    continue;

                postProcessor.PostProcess(result);
            }

            return result;
        }

        /// <summary>
        /// Gets the value indicating the <see cref="IFactoryStorage"/> instance
        /// that will be used to store each <see cref="IFactory"/> instance.
        /// </summary>
        protected IFactoryStorage FactoryStorage
        {
            get
            {
                return _factoryStorage;
            }
        }

        /// <summary>
        /// The list of postprocessors that will handle every
        /// service request result.
        /// </summary>
        public IList<IPostProcessor> PostProcessors
        {
            get { return _postProcessors; }
        }

        /// <summary>
        /// The list of preprocessors that will handle
        /// every service request before each actual service is created.
        /// </summary>
        public IList<IPreProcessor> PreProcessors
        {
            get { return _preprocessors; }
        }

        /// <summary>
        /// The list of services currently available inside the container.
        /// </summary>
        public virtual IEnumerable<IServiceInfo> AvailableServices
        {
            get
            {
                return FactoryStorage.AvailableFactories;
            }
        }
    }
}
