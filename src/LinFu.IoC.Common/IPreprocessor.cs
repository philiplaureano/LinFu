using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that can inspect or modify service requests
    /// from a given container before a service is created.
    /// </summary>
    public interface IPreProcessor
    {
        /// <summary>
        /// Allows a <see cref="IPostProcessor"/> instance
        /// to inspect or modify the result of a service request
        /// just before the service is instantiated.
        /// </summary>
        /// <seealso cref="IServiceRequestResult"/>
        /// <param name="request">The <see cref="IServiceRequest"/> instance that describes the nature of the service that needs to be created. </param>
        void Preprocess(IServiceRequest request);
    }
}
