using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Preconditions
{
    public delegate bool CheckHandler<T>(T target, InvocationInfo info)
        where T : class;
}
