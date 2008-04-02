using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.MxClone.Interfaces
{
    public interface ICustomTypeConverter
    {
        bool CanConvertTo(Type targetType);
        object ConvertFromString(string value);
    }
}
