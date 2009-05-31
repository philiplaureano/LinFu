using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents an <see cref="ActivationContext"/> that can be used to instantiate a given type
    /// and be used to describe the method that invoked the instantiation operation as well as specify the object
    /// instance that invoked the instantiation itself.
    /// </summary>
    public class TypeActivationContext : ActivationContext, ITypeActivationContext
    {
        /// <summary>
        /// Initializes a new instance of the MethodActivationContext class.
        /// </summary>
        /// <param name="target">The object instance that initiated the activation request.</param>
        /// <param name="targetMethod">The method where the activation was invoked.</param>
        /// <param name="concreteType">The type to be constructed.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the constructor.</param>
        public TypeActivationContext(object target, MethodBase targetMethod,
            Type concreteType, object[] additionalArguments)
            : base(concreteType, additionalArguments)
        {
            Target = target;
            TargetMethod = targetMethod;
        }

        /// <summary>
        /// Gets the value indicating the object instance that initiated the object instantiation operation.
        /// </summary>
        public object Target
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the value indiating the <see cref="MethodBase"/> instance that initiated the object instantiation operation.
        /// </summary>
        public MethodBase TargetMethod
        {
            get;
            protected set;
        }
    }
}
