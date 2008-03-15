using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace CSContractAssertions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MinorsCannotWithdrawAnyMoneyAttribute : Attribute, IPrecondition
    {
        #region IPrecondition Members

        public bool Check(object target, InvocationInfo info)
        {
            IBankAccount account = (IBankAccount) target;
            return account.Owner != null && account.Owner.Age >= 18;
        }

        public void ShowError(TextWriter output, object target, InvocationInfo info)
        {
            output.WriteLine("Only persons over the age of 18 can withdraw money from their accounts!");
        }

        #endregion

        #region IContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            string methodName = info.TargetMethod.Name;
            if (methodName != "Withdraw")
                return false;

            if (target is IBankAccount)
                return true;

            return false;
        }

        public void Catch(Exception ex)
        {
            // Ignore the error
        }

        #endregion
    }
}
