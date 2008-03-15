Imports LibraryInterfaces
Imports VBContractAssertions

<ShouldAlwaysHaveAnOwner(), BalanceIsAlwaysNonNegative()> _
Public Class BankAccount
    Implements IBankAccount
    Private accountBalance As Integer
    Private _owner As IPerson

    Public Sub New(ByVal amount As Integer)
        accountBalance = amount
    End Sub
    Public Overridable Property Owner() As IPerson Implements IBankAccount.Owner
        Get
            Return _owner
        End Get
        Set(ByVal value As IPerson)
            _owner = value
        End Set
    End Property

    ' BUG: VB.NET doesn't allow you to define attributes on getters 
    ' so the NonNegativeAttribute postcondition is not supported
    Public Overridable ReadOnly Property Balance() As Integer Implements IBankAccount.Balance
        Get
            Return accountBalance
        End Get
        
    End Property

    <EnsureBalanceReflectsDepositAmount()> _
    Public Overridable Sub Deposit(<NonNegative()> ByVal amount As Integer) Implements IBankAccount.Deposit
        accountBalance += amount
    End Sub
    <EnsureBalanceReflectsWithdrawalAmount(), MinorsCannotWithdrawAnyMoney()> _
    Public Overridable Sub Withdraw(<NonNegative()> ByVal amount As Integer) Implements IBankAccount.Withdraw
        ' Notice that I'm not checking for a negative
        ' balance on the postcondition--this is an intentional error
        accountBalance -= amount

        ' The non-negative balance invariant should catch
        ' the error
    End Sub
End Class
