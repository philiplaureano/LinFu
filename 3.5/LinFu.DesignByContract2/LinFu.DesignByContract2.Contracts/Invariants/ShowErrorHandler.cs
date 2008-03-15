using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public delegate void ShowErrorHandler(TextWriter output, object target, InvocationInfo info, InvariantState callState);
}
