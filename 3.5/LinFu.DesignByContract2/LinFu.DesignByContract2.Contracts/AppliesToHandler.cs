using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts
{
    public delegate bool AppliesToHandler(object target, InvocationInfo info);
}
