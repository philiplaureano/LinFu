Imports LibraryInterfaces
Imports LinFu.DesignByContract2.Core

<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Interface)> _
Public Class BalanceIsAlwaysNonNegativeAttribute
    Inherits Attribute
    Implements IInvariant

    Public Function AppliesTo(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) As Boolean Implements LinFu.DesignByContract2.Core.IContractCheck.AppliesTo
        If target Is Nothing Then Return False
        If Not (TypeOf target Is IBankAccount) Then Return False
        Return True
    End Function

    Public Sub [Catch](ByVal ex As System.Exception) Implements LinFu.DesignByContract2.Core.IContractCheck.Catch

    End Sub

    Public Function Check(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal callState As LinFu.DesignByContract2.Core.InvariantState) As Boolean Implements LinFu.DesignByContract2.Core.IInvariant.Check
        Dim account As IBankAccount = CType(target, IBankAccount)
        Return account.Balance >= 0
    End Function

    Public Sub ShowError(ByVal output As System.IO.TextWriter, ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal callState As LinFu.DesignByContract2.Core.InvariantState) Implements LinFu.DesignByContract2.Core.IInvariant.ShowError
        output.WriteLine("You cannot have a negative balance!")
    End Sub
End Class
