using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public delegate void ShowErrorHandler(TextWriter output, object target, InvocationInfo info);
}
