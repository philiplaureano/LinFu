using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Extensions;
using System.Reflection;
using LinFu.MxClone.Interfaces;

namespace LinFu.MxClone
{
    [Collectable]
    public class DefaultStringToTypeConverter : ICustomTypeConverter
    {
        private readonly Assembly _defaultAssembly = typeof(object).Assembly;
        #region ICustomTypeConverter Members

        public bool CanConvertTo(Type targetType)
        {
            return typeof(Type).IsAssignableFrom(targetType);
        }

        public object ConvertFromString(string value)
        {
            object result = _defaultAssembly.GetType(value);

            return result;
        }

        #endregion
    }
}
