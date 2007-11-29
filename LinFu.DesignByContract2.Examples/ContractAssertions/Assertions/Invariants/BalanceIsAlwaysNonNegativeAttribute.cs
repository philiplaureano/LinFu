using System;
using System.Collections.Generic;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace CSContractAssertions
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class BalanceIsAlwaysNonNegativeAttribute : Attribute, IInvariant
    {
        #region IInvariant Members

        public bool Check(object target, InvocationInfo info, InvariantState callState)
        {
            IBankAccount account = (IBankAccount) target;
            return account.Balance >= 0;
        }

        public void ShowError(System.IO.TextWriter output, object target, InvocationInfo info, InvariantState callState)
        {
            output.WriteLine("You cannot have a negative balance!");
        }

        #endregion

        #region IContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            return target != null && target is IBankAccount;
        }

        public void Catch(Exception ex)
        {
            // Do nothing
        }

        #endregion
    }
}
