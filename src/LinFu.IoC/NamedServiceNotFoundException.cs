using System;

namespace LinFu.IoC
{
    /// <summary>
    /// The exception thrown when a service name and a service type is
    /// requested from a named container and that named container
    /// is unable to find or create that particular service instance.
    /// </summary>
    public class NamedServiceNotFoundException : ServiceNotFoundException
    {
        private readonly string _serviceName;
        private readonly Type _serviceType;

        /// <summary>
        /// Initializes the service exception using the
        /// given <paramref name="serviceType"/> as
        /// the service that was not found.
        /// </summary>
        /// <param name="serviceType">The service type being requested.</param>
        /// <param name="serviceName">The name of the service being requested.</param>
        public NamedServiceNotFoundException(string serviceName, Type serviceType) : base(serviceType)
        {
            _serviceName = serviceName;
            _serviceType = serviceType;
        }

        /// <summary>
        /// The error message that this particular exception
        /// will display.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Unable to find a service named '{0}' with type '{1}'", _serviceName,
                                     _serviceType.AssemblyQualifiedName);
            }
        }
    }
}