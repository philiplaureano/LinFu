using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace LinFu.AOP.Interfaces
{
    public interface IInvocationContext
    {
        object Target { get; }
        MethodInfo TargetMethod { get; }
        MethodInfo CallingMethod { get; }
        StackTrace StackTrace { get; }
        Type[] TypeArguments { get; }
        Type[] ParameterTypes { get; }
        Type ReturnType { get; }
        object[] Arguments { get; }
    }
}
