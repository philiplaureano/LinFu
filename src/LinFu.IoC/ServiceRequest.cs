using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Reprsents the default implementation of the <see cref="IServiceRequest"/> interface.
    /// </summary>
    internal class ServiceRequest : ServiceInfo, IServiceRequest
    {
        private readonly object[] _proposedArguments;
        private readonly IFactory _proposedFactory;
        private readonly IServiceContainer _container;
        
        /// <summary>
        /// Initializes the <see cref="ServiceRequest"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <param name="serviceType">The requested service type.</param>
        /// <param name="proposedArguments">The proposed set of arguments that will be given to the factory.</param>
        /// <param name="proposedFactory">The <see cref="IFactory"/> instance that will be used to handle the service request.</param>
        /// <param name="container">The host container.</param>
        internal ServiceRequest(string serviceName, Type serviceType,
            object[] proposedArguments, IFactory proposedFactory, IServiceContainer container) : base(serviceName, serviceType)
        {
            _proposedArguments = proposedArguments;
            _proposedFactory = proposedFactory;
            _container = container;

            // The proposed arguments
            // will match the actual arguments by default
            ActualArguments = proposedArguments;

            // The same rule applies to the ActualFactory
            // property
            ActualFactory = proposedFactory;
        }

        /// <summary>
        /// Gets the value indicating the original arguments that
        /// were given during the service request.
        /// </summary>
        public object[] ProposedArguments
        {
            get { return _proposedArguments; }
        }

        /// <summary>
        /// Gets the value indicating the original <see cref="IFactory"/> instance
        /// that will be used to handle the service request.
        /// </summary>
        public IFactory ProposedFactory
        {
            get { return _proposedFactory; }
        }

        /// <summary>
        /// The container that will handle the service request.
        /// </summary>
        public IServiceContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Gets or sets the value indicating the actual arguments that
        /// will be used for the service request.
        /// </summary>
        public object[] ActualArguments
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the value indicating the actual <see cref="IFactory"/> instance
        /// that will be used to handle the service request.
        /// </summary>
        public IFactory ActualFactory
        {
            get;
            set;
        }           
    }
}
