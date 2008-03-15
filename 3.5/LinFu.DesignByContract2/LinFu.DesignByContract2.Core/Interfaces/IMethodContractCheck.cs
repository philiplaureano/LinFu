using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public interface IMethodContractCheck : IContractCheck 
    {
        // This is the marker interface for preconditions and postconditions
    }
}
