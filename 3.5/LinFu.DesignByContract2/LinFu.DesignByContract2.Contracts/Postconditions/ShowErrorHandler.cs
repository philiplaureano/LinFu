using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public delegate void ShowErrorHandler<T>(TextWriter output, T target, InvocationInfo info, object returnValue);
}
