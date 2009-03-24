using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a type that can intercept activation requests.
    /// </summary>
    public interface IActivatorHost
    {
        /// <summary>
        /// Gets or sets the value indicating the <see cref="IMethodActivator"/> that
        /// will be used to instantiate object types.
        /// </summary>
        IMethodActivator Activator { get; set; }
    }
}
