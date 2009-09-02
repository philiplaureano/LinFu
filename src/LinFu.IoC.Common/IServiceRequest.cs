using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents a class that describes the context of a service request made to a service container.
    /// </summary>
    public interface IServiceRequest : IServiceInfo
    {
        /// <summary>
        /// The container that will handle the service request.
        /// </summary>
        IServiceContainer Container { get; }

        /// <summary>
        /// Gets or sets the value indicating the actual arguments that
        /// will be used for the service request.
        /// </summary>
        object[] ActualArguments { get; set; }

        /// <summary>
        /// Gets the value indicating the original arguments that
        /// were given during the service request.
        /// </summary>
        object[] ProposedArguments { get; }

        /// <summary>
        /// Gets the value indicating the original <see cref="IFactory"/> instance
        /// that will be used to handle the service request.
        /// </summary>
        IFactory ProposedFactory { get; }

        /// <summary>
        /// Gets or sets the value indicating the actual <see cref="IFactory"/> instance
        /// that will be used to handle the service request.
        /// </summary>
        IFactory ActualFactory { get; set; }
    }
}
