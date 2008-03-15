Imports CSSampleLibrary
Imports LinFu.DesignByContract2.Injectors
Imports LibraryInterfaces
Imports Simple.IoC
Imports Simple.IoC.Loaders
Module Module1

    Sub Main()
        Dim container As New SimpleContainer()
        Dim loader As New Loader(container)

        ' Automatically load LinFu.DesignByContract2.Injectors.dll
        loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "*.dll")

        Dim account As IBankAccount = New BankAccount(0)
        account.Owner = New Person("Me", 17)

        ' HACK: Add the account instance for the container (normally one would
        ' use a factory class for this one, but this should do for the demo)
        container.AddService(account)

        Dim contractLoader As IContractLoader = container.GetService(Of IContractLoader)()

        ' Load the inferred contract from the sample library instead of the
        ' one embedded on the BankAccount class

        contractLoader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "CSSampleLibrary.dll")

        ' Wrap the account instance using the container
        Dim wrappedAccount As IBankAccount = container.GetService(Of IBankAccount)()

        ' This call will fail since minors cannot withdraw any money
        wrappedAccount.Withdraw(100)

    End Sub

End Module
