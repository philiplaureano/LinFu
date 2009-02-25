using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a class that can wrap itself around any given method call.
    /// </summary>
    public interface IAroundInvoke : IBeforeInvoke, IAfterInvoke
    {        
    }
}
