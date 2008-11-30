using System;

namespace LinFu.IoC
{
    /// <summary>
    /// The exception thrown when a service type is
    /// requested from a container and that named container
    /// is unable to find or create that particular service instance.
    /// </summary>
    public class ServiceNotFoundException : Exception
    {
        private readonly Type _serviceType;

        /// <summary>
        /// Initializes the service exception using the
        /// given <paramref name="serviceType"/> as
        /// the service that was not found.
        /// </summary>
        /// <param name="serviceType">The service type being requested.</param>
        public ServiceNotFoundException(Type serviceType)
        {
            _serviceType = serviceType;
        }

        /// <summary>
        /// The error message that this particular exception
        /// will display.
        /// </summary>
        public override string Message
        {
            get { return string.Format("Service type '{0}' not found", _serviceType.AssemblyQualifiedName); }
        }
    }
}