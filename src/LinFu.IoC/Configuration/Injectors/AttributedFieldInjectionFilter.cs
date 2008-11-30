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
    /// class that returns fields which have the <see cref="InjectAttribute"/>
    /// defined.
    /// </summary>
    public class AttributedFieldInjectionFilter : BaseMemberInjectionFilter<FieldInfo>
    {
        private readonly Type _attributeType;

        /// <summary>
        /// Initializes the class and uses the <see cref="InjectAttribute"/>
        /// to specify which field should be automatically injected with
        /// services from the container.
        /// </summary>
        public AttributedFieldInjectionFilter()
        {
            _attributeType = typeof (InjectAttribute);
        }

        /// <summary>
        /// Initializes the class and uses the <paramref name="attributeType"/>
        /// to specify which fields should be automatically injected with
        /// services from the container.
        /// </summary>
        /// <param name="attributeType">The custom property attribute that will be used to mark properties for automatic injection.</param>
        public AttributedFieldInjectionFilter(Type attributeType)
        {
            _attributeType = attributeType;
        }

        /// <summary>
        /// Determines which members should be selected from the <paramref name="targetType"/>
        /// using the <paramref name="container"/>
        /// </summary>
        /// <param name="targetType">The target type that will supply the list of members that will be filtered.</param>
        /// <param name="container">The target container.</param>
        /// <returns>A list of <see cref="FieldInfo"/> objects that pass the filter description.</returns>
        protected override IEnumerable<FieldInfo> GetMembers(Type targetType, IServiceContainer container)
        {
            // The field type must exist in the container and must be marked as public
            var results = from field in targetType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                          let fieldType = field.FieldType
                          let attributes = field.GetCustomAttributes(_attributeType, false)
                          where attributes != null && attributes.Length > 0 &&
                                container.Contains(fieldType) 
                          select field;

            return results;
        }
    }
}
