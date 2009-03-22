using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a special type of <see cref="IActivationContext"/> that can be used to instantiate a given type
    /// and be used to describe the method that invoked the instantiation operation as well as specify the object
    /// instance that invoked the instantiation itself.
    /// </summary>
    public interface IMethodActivationContext : IActivationContext
    {
        /// <summary>
        /// Gets the value indicating the object instance that initiated the object instantiation operation.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Gets the value indiating the <see cref="MethodBase"/> instance that initiated the object instantiation operation.
        /// </summary>
        MethodBase TargetMethod { get; }
    }
}
