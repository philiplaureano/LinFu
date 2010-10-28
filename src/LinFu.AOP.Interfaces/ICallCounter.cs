using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    internal interface ICallCounter
    {
        void Increment(IInvocationInfo context);
        void Decrement(IInvocationInfo context);
        int GetPendingCalls(IInvocationInfo context);
    }
}
