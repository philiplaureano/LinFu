Imports LibraryInterfaces

Public Class Person
    Implements IPerson
    Private _age As Integer
    Private _name As String
    Public Sub New(ByVal currentName As String, ByVal currentAge As Integer)
        _age = currentAge
        _name = currentName
    End Sub

    Public Overridable Property Age() As Integer Implements LibraryInterfaces.IPerson.Age
        Get
            Return _age
        End Get
        Set(ByVal value As Integer)
            _age = value
        End Set
    End Property

    Public Overridable Property Name() As String Implements LibraryInterfaces.IPerson.Name
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
End Class
