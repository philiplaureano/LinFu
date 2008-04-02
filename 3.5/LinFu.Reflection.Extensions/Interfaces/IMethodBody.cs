using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection.Extensions
{
    public interface IMethodBody
    {
        object Invoke(object target, string methodName,
            IMethodSignature methodSignature, object[] arguments);
    }
}
