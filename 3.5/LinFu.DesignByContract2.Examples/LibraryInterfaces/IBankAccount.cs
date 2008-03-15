using LibraryInterfaces;

namespace LibraryInterfaces
{
    public interface IBankAccount
    {
        IPerson Owner { get; set; }
        int Balance { get; }
        void Deposit(int amount);
        void Withdraw(int amount);
    }
}