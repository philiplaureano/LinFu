using System;
using System.Collections.Generic;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.DesignByContract2.Core
{
    public abstract class ContractCheck : IContractCheck
    {
        #region IContractCheck Members

        public virtual void Catch(Exception ex)
        {
            // Note: Override this method to have the application catch any exceptions thrown by a contract check
        }

        #endregion

        public abstract bool AppliesTo(object target, InvocationInfo info);
    }
}
