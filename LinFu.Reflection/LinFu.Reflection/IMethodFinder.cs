using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Reflection
{
    public interface IMethodFinder
    {
        MethodInfo Find(string methodName, Type targetType, object[] arguments);
    }
}
