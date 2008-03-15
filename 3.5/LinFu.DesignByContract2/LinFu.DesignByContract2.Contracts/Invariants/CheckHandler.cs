using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Invariants
{
    public delegate bool CheckHandler<T>(T target, InvocationInfo info, InvariantState callState)
        where T : class;
}
