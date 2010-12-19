using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a type that describes a service implementation.
    /// </summary>
    public interface IImplementationInfo
    {
        /// <summary>
        /// The name of the service that will be implemented.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The type of service that will be implemented.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Gets the value indicating the type that will be used to implement the <see cref="ServiceType"/>
        /// </summary>
        Type ImplementingType { get; }

        /// <summary>
        /// The instancing behavior of the service instance.        
        /// </summary>
        /// <seealso cref="LifecycleType"/>
        LifecycleType LifecycleType { get; }
    }
}
