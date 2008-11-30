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
    /// A class that automatically injects fields using values
    /// provided by an <see cref="IServiceContainer"/> instance.
    /// </summary>
    public class AutoFieldInjector : AutoMemberInjector<FieldInfo>
    {
        /// <summary>
        /// Injects a field with values from a given container.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="member">The <see cref="FieldInfo"/> instance that will store the service instance.</param>
        /// <param name="argumentResolver">The <see cref="IArgumentResolver"/> that will determine which values will be assigned to the target member.</param>
        /// <param name="additionalArguments">The additional arguments that were passed to the <see cref="IServiceRequestResult"/> during the instantiation process. Note: This parameter will be ignored by this override.</param>
        /// <param name="container">The container that will provide the service instances.</param>
        protected override void Inject(object target, FieldInfo member, IArgumentResolver argumentResolver,
            IServiceContainer container, object[] additionalArguments)
        {
            // Get the field value from the container
            var fieldType = member.FieldType;
            var fieldValues = argumentResolver.ResolveFrom(new Type[] { fieldType }, container);

            if (fieldValues == null || fieldValues.Length == 0)
                return;

            // Cast the field value to the target type
            var value = fieldValues[0];
            member.SetValue(target, value);
        }       
    }
}
