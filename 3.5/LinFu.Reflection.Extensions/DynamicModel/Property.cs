using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public class Property<T> : PropertySpec
    {
        public Property(string propertyName)
        {
            PropertyName = propertyName;
            PropertyType = typeof(T);
            Behavior = new PropertyBag();
        }
    }
}
