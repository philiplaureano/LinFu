using System;

namespace SampleLibrary
{
    public class Employee
    {
        private readonly BankAccount _checkingAccount = new BankAccount(42);
        public void Pay(int wage)
        {
            var currentAmount = wage;
            _checkingAccount.Deposit(currentAmount);

            Console.WriteLine("Paid {0} units", wage);
        }
    }
}
