Imports LinFu.DesignByContract2.Injectors
Imports LibraryInterfaces
Imports VBContractAssertions

<ContractFor(GetType(IBankAccount)), _
ShouldAlwaysHaveAnOwner(), _
BalanceIsAlwaysNonNegative()> _
Public Interface IContractForBankAccount
    Property Balance() As Integer

    <EnsureBalanceReflectsDepositAmount()> _
    Sub Deposit(<NonNegative()> ByVal amount As Integer)

    <EnsureBalanceReflectsWithdrawalAmount(), MinorsCannotWithdrawAnyMoney()> _
    Sub Withdraw(<NonNegative()> ByVal amount As Integer)

End Interface
