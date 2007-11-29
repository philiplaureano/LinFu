using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public abstract class PostconditionBase : ContractCheck, IPostcondition 
    {
        #region IPostcondition Members

        public virtual void BeforeMethodCall(object target, InvocationInfo info)
        {            
        }

        public abstract bool Check(object target, InvocationInfo info, object returnValue);

        public abstract void ShowError(System.IO.TextWriter output, object target,
                                       InvocationInfo info, object returnValue);
        
        #endregion
    }
}
