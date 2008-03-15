using System;
using System.Collections.Generic;
using System.Text;
using CSContractAssertions;
using LibraryInterfaces;
using LinFu.DesignByContract2.Injectors;

namespace CSSampleLibrary
{
    [ContractFor(typeof(IBankAccount))]
    [type: ShouldAlwaysHaveAnOwner]
    [type: BalanceIsAlwaysNonNegative]
    public interface IContractForBankAccount
    {
        int Balance { [return: NonNegative] get; }        
        [EnsureBalanceReflectsDepositAmount] void Deposit([NonNegative] int amount);
        
        [EnsureBalanceReflectsWithdrawalAmount, 
        MinorsCannotWithdrawAnyMoney] void Withdraw([NonNegative] int amount);
    }
}
