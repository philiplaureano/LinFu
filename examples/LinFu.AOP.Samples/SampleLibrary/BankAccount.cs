using System;

namespace SampleLibrary
{
    public class BankAccount
    {
        private int _balance;

        public BankAccount(int balance)
        {
            _balance = balance;
        }

        public int Balance
        {
            get { return _balance; }
        }

        public void Deposit(int amount)
        {
            _balance += amount;

            Console.WriteLine("Deposited: {0}", amount);
            Console.WriteLine("New Balance: {0}", Balance);
        }

        public void Withdraw(int amount)
        {
            _balance -= amount;
        }
    }
}
