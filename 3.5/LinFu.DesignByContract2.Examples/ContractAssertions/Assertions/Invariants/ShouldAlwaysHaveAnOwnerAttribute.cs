using System;
using System.Collections.Generic;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Core;

namespace CSContractAssertions
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class ShouldAlwaysHaveAnOwnerAttribute : Attribute, IInvariant
    {
        #region IInvariant Members

        public bool Check(object target, LinFu.DynamicProxy.InvocationInfo info, InvariantState callState)
        {
            IBankAccount account = (IBankAccount) target;
            return account.Owner != null;
        }

        public void ShowError(System.IO.TextWriter output, object target, LinFu.DynamicProxy.InvocationInfo info, InvariantState callState)
        {
            output.WriteLine("This bank account does not have an owner");
        }

        #endregion

        #region IContractCheck Members

        public bool AppliesTo(object target, LinFu.DynamicProxy.InvocationInfo info)
        {
            return target != null && target is IBankAccount;
        }

        public void Catch(Exception ex)
        {
            // Ignore the error
        }

        #endregion
    }
}
