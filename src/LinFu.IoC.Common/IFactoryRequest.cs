using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents the parameters made to a <see cref="IFactory"/> instance during
    /// a <see cref="IFactory.CreateInstance"/> method call.
    /// </summary>
    public interface IFactoryRequest
    {
        /// <summary>
        /// Gets or sets the value indicating the service container that made the service request.
        /// </summary>
        IServiceContainer Container { get; set; }

        /// <summary>
        /// Gets the value indicating the service name.
        /// </summary>
        /// <remarks>A null service name indicates that no service name was given during the request.</remarks>
        string ServiceName { get; }

        /// <summary>
        /// Gets the value indicating the requested service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Gets the value indicating the additional arguments given in the factory request.
        /// </summary>
        object[] Arguments { get; }
    }
}
