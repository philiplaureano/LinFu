using System;
using System.Collections.Generic;
using System.Text;
using LibraryInterfaces;
using LinFu.DesignByContract2.Injectors;
using VBSampleLibrary;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace InferredContractsInCS
{
    class Program
    {
        static void Main()
        {
            SimpleContainer container = new SimpleContainer();
            Loader loader = new Loader(container);
            // Automatically load LinFu.DesignByContract2.Injectors.dll
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "*.dll");

            IBankAccount account = new BankAccount(0);
            account.Owner = new Person("Me", 17);

            // HACK: Add the account instance for the container (normally one would
            // use a factory class for this one, but this should do for the demo)
            container.AddService(account);

            IContractLoader contractLoader = container.GetService<IContractLoader>();
            
            // Load the inferred contract from the sample library instead of the
            // one embedded on the BankAccount class
            contractLoader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, 
                "VBSampleLibrary.dll");

            // Wrap the account instance using the container
            IBankAccount wrappedAccount = container.GetService<IBankAccount>();

            // This call will fail since minors cannot withdraw any money
            wrappedAccount.Withdraw(100);
        }
    }
}
