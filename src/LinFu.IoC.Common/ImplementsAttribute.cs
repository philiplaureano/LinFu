using System;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    ///     The attribute used to specify how a service should be implemented
    ///     in addition to its instancing behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ImplementsAttribute : Attribute
    {
        /// <summary>
        ///     The name to associate with the given service.
        /// </summary>
        public string ServiceName;

        /// <summary>
        ///     Allows users to add services to a container using a
        ///     given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <remarks>By default, each service will be created once per request.</remarks>
        /// <param name="serviceType">The <see cref="System.Type" /> of service to implement.</param>
        public ImplementsAttribute(Type serviceType) : this(serviceType, LifecycleType.OncePerRequest)
        {
        }

        /// <summary>
        ///     Allows users to add services to a container using a
        ///     given <paramref name="serviceType">service type</paramref> and
        ///     <paramref name="lifeCycleType">lifecycle type</paramref>.
        /// </summary>
        /// <param name="serviceType">The <see cref="System.Type" /> of service to implement.</param>
        /// <param name="lifeCycleType">The instancing behavior to use with this implementation.</param>
        public ImplementsAttribute(Type serviceType, LifecycleType lifeCycleType)
        {
            ServiceType = serviceType;
            LifecycleType = lifeCycleType;
        }

        /// <summary>
        ///     The type of service that will be implemented.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///     The instancing behavior of the service instance.
        /// </summary>
        /// <seealso cref="LifecycleType" />
        public LifecycleType LifecycleType { get; }
    }
}