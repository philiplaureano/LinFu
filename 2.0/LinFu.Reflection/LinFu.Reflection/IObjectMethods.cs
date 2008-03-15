using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Delegates;

namespace LinFu.Reflection
{
    public interface IObjectMethods
    {
        CustomDelegate this[string methodName] { get; }
    }
}
