using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A default implementation of the <see cref="IMemberInjectionFilter{TMember}"/>
    /// class that returns methods which have the <see cref="InjectAttribute"/>
    /// defined.
    /// </summary>
    public class AttributedMethodInjectionFilter : BaseMemberInjectionFilter<MethodInfo>
    {
        private readonly Type _attributeType;

        /// <summary>
        /// Initializes the class with the <see cref="InjectAttribute"/> as the
        /// default injection attribute.
        /// </summary>
        public AttributedMethodInjectionFilter()
        {
            _attributeType = typeof (InjectAttribute);
        }

        /// <summary>
        /// Initializes the class and uses the <paramref name="attributeType"/>
        /// as the custom injection attribute.
        /// </summary>
        /// <param name="attributeType"></param>
        public AttributedMethodInjectionFilter(Type attributeType)
        {
            _attributeType = attributeType;
        }

        /// <summary>
        /// Returns the methods that have the custom attribute type defined.
        /// </summary>
        /// <param name="targetType">The target type that contains the target methods.</param>
        /// <param name="container">The host container.</param>
        /// <returns>The list of methods that have the custom attribute type defined.</returns>
        protected override IEnumerable<MethodInfo> GetMembers(Type targetType, 
            IServiceContainer container)
        {
            var results = from method in targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          let attributes = _attributeType != null ? 
                          method.GetCustomAttributes(_attributeType, false) : null            
                          where attributes != null && attributes.Length > 0
                          select method;

            return results;
        }
    }
}
