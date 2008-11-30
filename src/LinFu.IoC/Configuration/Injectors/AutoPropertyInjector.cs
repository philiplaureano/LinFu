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
    /// A class that automatically injects property dependencies into
    /// service instances.
    /// </summary>
    public class AutoPropertyInjector : AutoMemberInjector<PropertyInfo>
    {
        /// <summary>
        /// Injects services from the <paramref name="container"/> into the target <see cref="PropertyInfo"/> instance.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="property">The <see cref="PropertyInfo"/> instance that will store the service instance.</param>
        /// <param name="resolver">The <see cref="IArgumentResolver"/> that will determine which arguments will be assigned to the target member.</param>
        /// <param name="additionalArguments">The additional arguments that were passed to the <see cref="IServiceRequestResult"/> during the instantiation process.</param>
        /// <param name="container">The container that will provide the service instances.</param>
        protected override void Inject(object target, PropertyInfo property,
            IArgumentResolver resolver, IServiceContainer container, object[] additionalArguments)
        {
            var setter = container.GetService<IPropertySetter>();
            if (setter == null)
                return;

            // Determine the property value
            var results = resolver.ResolveFrom(new Type[] { property.PropertyType }, container);
            var propertyValue = results.FirstOrDefault();
            
            if (propertyValue == null)
                return;
            
            // Call the setter against the target property
            setter.Set(target, property, propertyValue);
        }
    }
}
