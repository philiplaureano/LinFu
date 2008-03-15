Imports LibraryInterfaces
Imports LinFu.DesignByContract2.Core

<AttributeUsage(AttributeTargets.Method Or AttributeTargets.ReturnValue)> _
Public Class EnsureBalanceReflectsWithdrawalAmountAttribute
    Inherits Attribute
    Implements IPostcondition
    Private oldBalance As Integer
    Private expectedBalance As Integer


    Public Function AppliesTo(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) As Boolean Implements LinFu.DesignByContract2.Core.IContractCheck.AppliesTo
        If target Is Nothing Then Return False
        If Not (TypeOf target Is IBankAccount) Then Return False
        Return True
    End Function

    Public Sub [Catch](ByVal ex As System.Exception) Implements LinFu.DesignByContract2.Core.IContractCheck.Catch

    End Sub

    Public Sub BeforeMethodCall(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) Implements LinFu.DesignByContract2.Core.IPostcondition.BeforeMethodCall
        If target Is Nothing Then Return
        If Not (TypeOf target Is IBankAccount) Then Return

        Dim account As IBankAccount = CType(target, IBankAccount)
        oldBalance = account.Balance
    End Sub

    Public Function Check(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal returnValue As Object) As Boolean Implements LinFu.DesignByContract2.Core.IPostcondition.Check
        If target Is Nothing Then Return True
        If Not (TypeOf target Is IBankAccount) Then Return True

        Dim account As IBankAccount = CType(target, IBankAccount)
        Dim amount As Integer = CType(info.Arguments(0), Integer)
        expectedBalance = oldBalance - amount
        Return account.Balance = expectedBalance
    End Function

    Public Sub ShowError(ByVal output As System.IO.TextWriter, ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal returnValue As Object) Implements LinFu.DesignByContract2.Core.IPostcondition.ShowError
        output.WriteLine("Withdrawal Failed! The expected balance should have been '{0}'", expectedBalance)
    End Sub
End Class
