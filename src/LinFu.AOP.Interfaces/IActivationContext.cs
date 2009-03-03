using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that describes a request to instantiate a particular object type.
    /// </summary>
    public interface IActivationContext
    {
        /// <summary>
        /// Gets the value indicating the type to be instantiated.
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// Gets the value indicating the arguments that will be passed to the constructor during instantiation.
        /// </summary>
        object[] AdditionalArguments { get; }
    }
}