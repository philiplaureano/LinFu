using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.Reflection
{
    public interface IObjectProperties
    {
        object this[string propertyName] { get; set; }
    }
}
