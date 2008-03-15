using System;
using System.Diagnostics;
using System.Reflection;

namespace LinFu.DynamicProxy
{
    public delegate object InterceptorHandler(object proxy, MethodInfo targetMethod,
                                              StackTrace trace, Type[] genericTypeArgs, object[] args);
}