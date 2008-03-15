using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Core
{
    public interface IMethodContract
    {
        IList<IPrecondition> Preconditions { get; }
        IList<IPostcondition> Postconditions { get; }
    }
}
