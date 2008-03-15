using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Contracts.Postconditions
{
    public delegate bool CheckHandler<T>(T target, InvocationInfo info, object returnValue)
        where T : class;
}
