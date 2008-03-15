using System;
using System.Collections.Generic;
using System.Text;

namespace LinFu.DesignByContract2.Core
{
    public abstract class InvariantBase : ContractCheck, IInvariant
    {
        #region IInvariant Members

        public abstract bool Check(object target, LinFu.DynamicProxy.InvocationInfo info, InvariantState callState);
        public abstract void ShowError(System.IO.TextWriter output, object target, LinFu.DynamicProxy.InvocationInfo info,
                              InvariantState callState);        
        #endregion
    }
}
