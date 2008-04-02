using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public interface IPropertyBehavior
    {
        object GetValue(object target, string propertyName, Type propertyType);
        void SetValue(object target, string propertyName, Type propertyType, object value);
    }
}
