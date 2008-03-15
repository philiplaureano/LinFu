Imports LibraryInterfaces
Imports LinFu.DesignByContract2.Core

<AttributeUsage(AttributeTargets.Method)> _
Public Class MinorsCannotWithdrawAnyMoneyAttribute
    Inherits Attribute
    Implements IPrecondition


    Public Function AppliesTo(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) As Boolean Implements LinFu.DesignByContract2.Core.IContractCheck.AppliesTo
        Dim methodName As String = info.TargetMethod.Name
        If methodName <> "Withdraw" Then Return False
        If (TypeOf target Is IBankAccount) Then Return True
        Return False
    End Function

    Public Sub [Catch](ByVal ex As System.Exception) Implements LinFu.DesignByContract2.Core.IContractCheck.Catch

    End Sub

    Public Function Check(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) As Boolean Implements LinFu.DesignByContract2.Core.IPrecondition.Check
        Dim account As IBankAccount = CType(target, IBankAccount)
        Return Not ((account Is Nothing) Or (account.Owner Is Nothing)) And account.Owner.Age >= 18
    End Function

    Public Sub ShowError(ByVal output As System.IO.TextWriter, ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) Implements LinFu.DesignByContract2.Core.IPrecondition.ShowError
        output.WriteLine("Only persons over the age of 18 can withdraw money from their accounts!")
    End Sub
End Class
