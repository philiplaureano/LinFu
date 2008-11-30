using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// A type that represents a service instance returned by a container.
    /// </summary>
    public interface IServiceInstance
    {
        /// <summary>
        /// Gets the value indicating the <see cref="IServiceInfo"/> instance 
        /// that describes the service instance itself.
        /// </summary>
        IServiceInfo ServiceInfo { get; }

        /// <summary>
        /// Gets the value indicating the service instance itself.
        /// </summary>
        object Object { get; }
    }
}
