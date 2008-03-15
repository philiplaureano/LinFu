using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LinFu.Delegates
{
    public interface IInvoker
    {
        object Invoke(object target, MethodBase targetMethod, IEnumerable<object> curriedArguments, IEnumerable<object> invokeArguments);
    }
}
