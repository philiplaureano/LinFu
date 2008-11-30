using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// A class responsible for setting property values.
    /// </summary>
    public interface IPropertySetter
    {
        /// <summary>
        /// Sets the value of the <paramref name="targetProperty"/>.
        /// </summary>
        /// /// <param name="target">The target instance that contains the property to be modified.</param>
        /// <param name="targetProperty">The property that will store the given value.</param>
        /// <param name="value">The value that will be assigned to the property.</param>
        void Set(object target, PropertyInfo targetProperty, object value);
    }
}
