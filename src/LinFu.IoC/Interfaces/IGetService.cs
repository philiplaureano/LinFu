using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that determines the behavior a <see cref="ServiceContainer"/> instance.
    /// </summary>
    public interface IGetService
    {
        /// <summary>
        /// Causes the container to instantiate the service using the given
        /// <paramref name="serviceRequest">service request</paramref>. If the service type cannot be created, it will simply return null.
        /// </summary>
        /// <returns>A valid object reference if the service can be created; otherwise, it will return <c>null</c></returns>
        object GetService(IServiceRequest serviceRequest);
    }
}
