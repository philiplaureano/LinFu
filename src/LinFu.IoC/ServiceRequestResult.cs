using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents the results returned when a service request
    /// is made against an <see cref="IContainer"/> instance.
    /// </summary>
    internal class ServiceRequestResult : IServiceRequestResult
    {
        /// <summary>
        /// The name of the service being created. By default, this property is blank.
        /// </summary>
        public string ServiceName { get; internal set; }

        /// <summary>
        /// The raw object reference created by the container itself.
        /// </summary>
        public object OriginalResult { get; internal set; }

        /// <summary>
        /// The result that will be returned from the container
        /// instead of the <see cref="OriginalResult"/>. 
        /// 
        /// If this property is null, then the original result will be used.
        /// </summary>
        public object ActualResult { get; set; }

        /// <summary>
        /// The type of service being requested.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// The container that will handle the service request.
        /// </summary>
        public IServiceContainer Container { get; internal set; }

        /// <summary>
        /// Gets or sets the value indicating the additional arguments that
        /// were used during the service request.
        /// </summary>
        public object[] AdditionalArguments { get; internal set; }
    }
}