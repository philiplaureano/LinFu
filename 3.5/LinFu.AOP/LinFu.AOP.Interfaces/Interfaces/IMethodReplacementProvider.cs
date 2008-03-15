using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IMethodReplacementProvider
    {
        bool CanReplace(IInvocationContext context);
        IMethodReplacement GetMethodReplacement(IInvocationContext context);
    }
}
