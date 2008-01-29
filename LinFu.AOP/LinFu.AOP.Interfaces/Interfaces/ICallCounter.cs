using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.AOP.Interfaces
{
    internal interface ICallCounter
    {
        void Increment(IInvocationContext context);
        void Decrement(IInvocationContext context);
        int GetPendingCalls(IInvocationContext context);        
    }
}
