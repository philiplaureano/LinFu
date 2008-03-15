using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Core
{
    public abstract class PreconditionBase : ContractCheck, IPrecondition
    {
        #region IPrecondition Members

        public abstract bool Check(object target, LinFu.DynamicProxy.InvocationInfo info);
        

        public abstract void ShowError(System.IO.TextWriter output, object target,
                                       LinFu.DynamicProxy.InvocationInfo info);        

        #endregion

        #region IMethodContractCheck Members
        
        #endregion        
    }
}
