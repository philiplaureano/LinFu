Imports LinFu.DesignByContract2.Attributes
Imports LinFu.DesignByContract2.Core

<AttributeUsage(AttributeTargets.Parameter Or AttributeTargets.ReturnValue)> _
Public Class NonNegativeAttribute
    Inherits Attribute
    Implements IParameterPrecondition
    Implements IPostcondition

    Public Function Check(ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal parameter As System.Reflection.ParameterInfo, ByVal argValue As Object) As Boolean Implements LinFu.DesignByContract2.Attributes.IParameterPrecondition.Check
        Dim value As Integer = Convert.ToInt32(argValue)
        Return value >= 0
    End Function

    Public Sub ShowErrorMessage(ByVal stdOut As System.IO.TextWriter, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal parameter As System.Reflection.ParameterInfo, ByVal argValue As Object) Implements LinFu.DesignByContract2.Attributes.IParameterPrecondition.ShowErrorMessage
        stdOut.WriteLine("The parameter '{0}' must be non-negative", parameter.Name)
    End Sub

    Public Function AppliesTo(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) As Boolean Implements LinFu.DesignByContract2.Core.IContractCheck.AppliesTo
        If target Is Nothing Then Return False
        Return True
    End Function

    Public Sub [Catch](ByVal ex As System.Exception) Implements LinFu.DesignByContract2.Core.IContractCheck.Catch
        ' Do nothing
    End Sub

    Public Sub BeforeMethodCall(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo) Implements LinFu.DesignByContract2.Core.IPostcondition.BeforeMethodCall

    End Sub

    Public Function Check1(ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal returnValue As Object) As Boolean Implements LinFu.DesignByContract2.Core.IPostcondition.Check
        Dim value As Integer = Convert.ToInt32(returnValue)
        Return value >= 0
    End Function

    Public Sub ShowError(ByVal output As System.IO.TextWriter, ByVal target As Object, ByVal info As LinFu.DynamicProxy.InvocationInfo, ByVal returnValue As Object) Implements LinFu.DesignByContract2.Core.IPostcondition.ShowError
        output.WriteLine("The return value must be non-negative")
    End Sub
End Class
