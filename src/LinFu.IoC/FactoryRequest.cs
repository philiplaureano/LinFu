using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IFactoryRequest"/> interface.
    /// </summary>
    public class FactoryRequest : IFactoryRequest
    {
        /// <summary>
        /// Gets the value indicating the service container that made the service request.
        /// </summary>
        public IServiceContainer Container { get; set; }

        /// <summary>
        /// Gets the value indicating the service name.
        /// </summary>
        /// <remarks>A null service name indicates that no service name was given during the request.</remarks>
        public string ServiceName
        {
            get; set;
        }

        /// <summary>
        /// Gets the value indicating the requested service type.
        /// </summary>
        public Type ServiceType
        {
            get; set;
        }

        /// <summary>
        /// Gets the value indicating the additional arguments given in the factory request.
        /// </summary>
        public object[] Arguments
        {
            get; set;
        }
    }
}
