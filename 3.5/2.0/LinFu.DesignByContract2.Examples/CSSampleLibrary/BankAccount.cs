using System;
using System.Collections.Generic;
using System.Text;

using CSContractAssertions;
using LibraryInterfaces;

namespace CSSampleLibrary
{
    [type: ShouldAlwaysHaveAnOwner]
    [type: BalanceIsAlwaysNonNegative]
    public class BankAccount : IBankAccount
    {
        private int _balance;
        private IPerson _owner;
        public BankAccount(int balance)
        {
            _balance = balance;
        }
        public virtual IPerson Owner
        {
            get { return _owner;  }
            set { _owner = value; }
        }
        public virtual int Balance
        {
            [return:NonNegative] get { return _balance; }
        }
        
        [EnsureBalanceReflectsDepositAmount]
        public virtual void Deposit([NonNegative] int amount)
        {
            _balance += amount;
        }

        [MinorsCannotWithdrawAnyMoney] 
        [EnsureBalanceReflectsWithdrawalAmount]
        public virtual void Withdraw([NonNegative] int amount)
        {
            // Notice that I'm not checking for a negative
            // balance on the postcondition--this is an intentional error
            _balance -= amount;

            // The non-negative balance invariant should catch
            // the error
        }
    }
}
