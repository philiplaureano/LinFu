using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a type that adapts an <see cref="ImplementsAttribute"/> class to a <see cref="IImplementationInfo"/> interface.
    /// </summary>
    internal class ImplementsAttributeImplementationInfoAdapter : IImplementationInfo
    {
        private readonly ImplementsAttribute _implementsAttribute;
        private readonly Type _declaringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementsAttributeImplementationInfoAdapter"/> class.
        /// </summary>
        /// <param name="implementsAttribute">The <see cref="ImplementsAttribute"/> instance that describes a given service.</param>
        /// <param name="declaringType">The declaring type that is attached to the given attribute.</param>
        public ImplementsAttributeImplementationInfoAdapter(ImplementsAttribute implementsAttribute, Type declaringType)
        {
            _implementsAttribute = implementsAttribute;
            _declaringType = declaringType;
        }

        /// <summary>
        /// The name of the service that will be implemented.
        /// </summary>
        public string ServiceName
        {
            get { return _implementsAttribute.ServiceName; }
        }

        /// <summary>
        /// The type of service that will be implemented.
        /// </summary>
        public Type ServiceType
        {
            get { return _implementsAttribute.ServiceType; }
        }

        /// <summary>
        /// Gets the value indicating the type that will be used to implement the <see cref="ServiceType"/>
        /// </summary>
        public Type ImplementingType
        {
            get { return _declaringType; }
        }

        /// <summary>
        /// The instancing behavior of the service instance.        
        /// </summary>
        /// <seealso cref="LifecycleType"/>
        public LifecycleType LifecycleType
        {
            get
            {
                return _implementsAttribute.LifecycleType;
            }
        }
    }
}
