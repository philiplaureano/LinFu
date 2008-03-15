using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryInterfaces;
using VBSampleLibrary;

using LinFu.DesignByContract2.Contracts;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;


namespace AdHocContractSample
{
    class Program
    {
        static void Main(string[] args)
        {
            AdHocContract contract = new AdHocContract();

            // Only adults can withdraw any money
            Require.On(contract)
                .ForMethodWith(m => m.Name == "Withdraw")
                .That<IBankAccount>(b => b.Owner != null && b.Owner.Age >= 18)
                .OtherwisePrint("Only persons over the age of 18 can withdraw money from their accounts!");
            
            BankAccount account = new BankAccount(0);
            account.Owner = new Person("Me", 17);

            // Wrap the bank account instance in a contract
            ProxyFactory factory = new ProxyFactory();
            ContractChecker checker = new ContractChecker(contract);
            checker.Target = account;

            IBankAccount wrappedAccount = factory.CreateProxy<IBankAccount>(checker);

            // This call will fail since minors cannot withdraw any money
            wrappedAccount.Withdraw(100);
        }
    }
}
