using System;
using System.Collections.Generic;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// A class that describes a single service
    /// provided by a container.
    /// </summary>
    public interface IServiceInfo
    {
        /// <summary>
        /// The name of the service being created. By default, this property is blank.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The type of service being requested.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Gets a value indicating the list of arguments required by this particular service.
        /// </summary>
        IEnumerable<Type> ArgumentTypes { get; }
    }
}