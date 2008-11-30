using System;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents the results returned when a service request
    /// is made against an <see cref="IServiceContainer"/> instance.
    /// </summary>
    public interface IServiceRequestResult : IServiceInfo
    {
        /// <summary>
        /// The raw object reference created by the container itself.
        /// </summary>
        object OriginalResult { get; }

        /// <summary>
        /// The result that will be returned from the container
        /// instead of the <see cref="OriginalResult"/>. 
        /// 
        /// If this property is null, then the original result will be used.
        /// </summary>
        object ActualResult { get; set; }

        /// <summary>
        /// The container that will handle the service request.
        /// </summary>
        IServiceContainer Container { get; }


        /// <summary>
        /// Gets the value indicating the additional arguments that
        /// were used during the service request.
        /// </summary>
        object[] AdditionalArguments { get; }
    }
}