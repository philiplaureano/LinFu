Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports CSSampleLibrary
Imports LibraryInterfaces
Imports LinFu.DesignByContract2.Attributes
Imports LinFu.DesignByContract2.Core
Imports LinFu.DynamicProxy

Module Module1
    Sub Main()
        ' Notice that the actual contracts were written in C#
        Dim factory As New ProxyFactory()

        Dim myAccount As BankAccount = New BankAccount(0)
        myAccount.Owner = New Person("Me", 17)


        ' The contract will be generated automatically at runtime
        Dim checker As New ContractChecker(New AttributeContractProvider())
        checker.Target = myAccount


        Dim account As IBankAccount = factory.CreateProxy(Of IBankAccount)(checker)

        ' This call will fail since minors cannot withdraw any money
        account.Withdraw(100)
    End Sub

End Module