using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default implementation for the <see cref="IGetService"/> interface.
    /// </summary>
    public class DefaultGetServiceBehavior : IGetService
    {
        private readonly IServiceContainer _container;
        private readonly ICreateInstance _creator;
        private readonly IPreProcessor _preProcessor;
        private readonly IPostProcessor _postProcessor;

        /// <summary>
        /// Initializes the class with the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="container">The target service container.</param>
        public DefaultGetServiceBehavior(IServiceContainer container)
        {
            _container = container;
            _creator = new DefaultCreator();
            _preProcessor = new CompositePreProcessor(container.PreProcessors);
            _postProcessor = new CompositePostProcessor(container.PostProcessors);
        }

        /// <summary>
        /// Initializes the class with the given <paramref name="container"/> instance.
        /// </summary>
        /// <param name="container">The target service container.</param>
        /// <param name="creator">The <see cref="ICreateInstance"/> instance responsible for instantiating service types.</param>
        /// <param name="preProcessor">The <see cref="IPreProcessor"/> that will allow users to intercept a given service request.</param>
        /// <param name="postProcessor">The <see cref="IPostProcessor"/> instance that will handle the results of a given service request.</param>
        public DefaultGetServiceBehavior(IServiceContainer container, ICreateInstance creator, IPreProcessor preProcessor, IPostProcessor postProcessor)
        {
            _container = container;
            _creator = creator;
            _preProcessor = preProcessor;
            _postProcessor = postProcessor;
        }

        /// <summary>
        /// Instantiates the service described by the <paramref name="serviceRequest"/>.
        /// </summary>
        /// <param name="serviceRequest">The <see cref="IServiceRequest"/> that describes the service that needs to be instantiated.</param>
        /// <returns>A valid object reference if the service can be found; otherwise, it will return <c>null</c>.</returns>
        public virtual object GetService(IServiceRequest serviceRequest)
        {
            // Allow users to intercept the instantiation process
            if (_preProcessor != null)
                _preProcessor.Preprocess(serviceRequest);

            var factoryRequest = new FactoryRequest
            {
                ServiceType = serviceRequest.ServiceType,
                ServiceName = serviceRequest.ServiceName,
                Arguments = serviceRequest.ActualArguments,
                Container = _container
            };

            var instance = _creator.CreateFrom(factoryRequest, serviceRequest.ActualFactory);

            // Postprocess the results
            var result = new ServiceRequestResult
            {
                ServiceName = serviceRequest.ServiceName,
                ActualResult = instance,
                Container = _container,
                OriginalResult = instance,
                ServiceType = serviceRequest.ServiceType,
                AdditionalArguments = serviceRequest.ActualArguments
            };

            if (_postProcessor != null)
                _postProcessor.PostProcess(result);

            return result.ActualResult ?? result.OriginalResult;
        }       
    }
}
