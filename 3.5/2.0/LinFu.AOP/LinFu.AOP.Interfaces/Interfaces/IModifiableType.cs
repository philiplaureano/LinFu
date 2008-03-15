using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IModifiableType
    {
        bool IsInterceptionEnabled { get; set; }
        IAroundInvokeProvider AroundInvokeProvider { get; set; }
        IMethodReplacementProvider MethodReplacementProvider { get; set; }
    }
}
