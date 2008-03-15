using System;
using System.Collections.Generic;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Attributes;
using LinFu.DesignByContract2.Core;
using LinFu.DynamicProxy;

using VBSampleLibrary;

namespace EmbeddedContractsInCS
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxyFactory factory = new ProxyFactory();
            IBankAccount myAccount = new BankAccount(0);
            myAccount.Owner = new Person("Me", 17);

            // The contract will be generated automatically at runtime
            ContractChecker checker = new ContractChecker(new AttributeContractProvider());
            checker.Target = myAccount;

            IBankAccount account = factory.CreateProxy<IBankAccount>(checker);

            // This call will fail since minors cannot withdraw any money
            account.Withdraw(100);
        }
    }
}
