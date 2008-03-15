using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Injectors
{
    public interface ITypeWrapper
    {
        bool CanWrap(Type targetType, object instance);
        object Wrap(Type type, object value);
    }
}
