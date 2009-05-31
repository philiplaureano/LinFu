using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that describes the state of a field just as it is being intercepted by a <see cref="IFieldInterceptor"/>.
    /// </summary>
    public class FieldInterceptionContext : IFieldInterceptionContext
    {
        /// <summary>
        /// Initializes a new instance of the FieldInterceptionContext class.
        /// </summary>
        /// <param name="target">The target that hosts the given field.</param>
        /// <param name="targetMethod">The method that accessed the target field.</param>
        /// <param name="targetField">The field currently being accessed by the target method.</param>
        /// <param name="hostType">The type that hosts the target field.</param>
        public FieldInterceptionContext(object target, MethodBase targetMethod, FieldInfo targetField, Type hostType)
        {
            Target = target;
            TargetMethod = targetMethod;
            TargetField = targetField;
            HostType = hostType;
        }

        /// <summary>
        /// Gets a value indicating the target instance that is attached to the target field.
        /// </summary>
        public object Target
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets a value indicating the host method that is currently accessing the target field.
        /// </summary>
        public MethodBase TargetMethod
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating the field that is currently being accessed by the target method.
        /// </summary>
        public FieldInfo TargetField
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating the <see cref="System.Type"/> that holds the target field.
        /// </summary>
        public Type HostType
        {
            get;
            internal set;
        }
    }
}
