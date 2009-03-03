using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a class that describes a request to instantiate a particular object type using a given
    /// <see cref="IServiceContainer"/> instance.
    /// </summary>
    public interface IContainerActivationContext : IActivationContext 
    {
        /// <summary>
        /// Gets the value indicating the <see cref="IServiceContainer"/> instance
        /// that will instantiate the <see cref="IActivationContext.TargetType"/>.
        /// </summary>
        IServiceContainer Container { get; }
    }
}
