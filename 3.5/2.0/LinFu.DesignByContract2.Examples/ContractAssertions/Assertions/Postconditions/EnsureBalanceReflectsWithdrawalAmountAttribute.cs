using System;
using System.Collections.Generic;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

namespace CSContractAssertions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnsureBalanceReflectsWithdrawalAmountAttribute : Attribute, IPostcondition
    {
        private int _oldBalance;
        private int _expectedBalance;
        #region IPostcondition Members

        public void BeforeMethodCall(object target, InvocationInfo info)
        {
            if (target == null || !(target is IBankAccount))
                return;
            IBankAccount account = (IBankAccount)target;
            _oldBalance = account.Balance;
        }

        public bool Check(object target, InvocationInfo info, object returnValue)
        {
            if (target == null || !(target is IBankAccount))
                return true;

            IBankAccount account = target as IBankAccount;
            int amount = (int)info.Arguments[0];
            _expectedBalance = _oldBalance - amount;

            return account.Balance == _expectedBalance;
        }

        public void ShowError(System.IO.TextWriter output, object target, InvocationInfo info, object returnValue)
        {
            output.WriteLine("Withdrawal Failed! The expected balance should have been '{0}'", _expectedBalance);
        }

        #endregion

        #region IContractCheck Members

        public bool AppliesTo(object target, InvocationInfo info)
        {
            return target is IBankAccount;
        }

        public void Catch(Exception ex)
        {
            // Do nothing
        }

        #endregion
    }
}
