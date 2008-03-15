using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IAroundInvokeProvider
    {        
        IAroundInvoke GetSurroundingImplementation(IInvocationContext context);        
    }
}
