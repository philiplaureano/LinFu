using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that describes the state of a field just as it is being intercepted.
    /// </summary>
    public interface IFieldInterceptionContext
    {
        /// <summary>
        /// Gets a value indicating the target instance that is attached to the target field.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Gets a value indicating the host method that is currently accessing the target field.
        /// </summary>
        MethodBase TargetMethod { get; }

        /// <summary>
        /// Gets a value indicating the field that is currently being accessed by the target method.
        /// </summary>
        FieldInfo TargetField { get; }

        /// <summary>
        /// Gets a value indicating the <see cref="System.Type"/> that holds the target field.
        /// </summary>
        Type HostType { get; }
    }
}
