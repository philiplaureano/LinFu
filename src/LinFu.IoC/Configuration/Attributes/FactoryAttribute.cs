using System;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// An attribute that marks a type as a custom factory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FactoryAttribute : Attribute
    {
        private readonly Type _serviceType;

        /// <summary>
        /// The service name that will be associated
        /// with the service type.
        /// </summary>
        public string ServiceName;

        /// <summary>
        /// Marks a target type as a custom factory
        /// that can create object instances that
        /// can implement the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The service type to create.</param>
        public FactoryAttribute(Type serviceType)
        {
            _serviceType = serviceType;
        }

        /// <summary>
        /// Gets the service type that can be created
        /// using the factory instance.
        /// </summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }
    }
}