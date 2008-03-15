using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IAroundInvoke
    {
        void AfterInvoke(IInvocationContext context, object returnValue);
        void BeforeInvoke(IInvocationContext context);
    }
}
