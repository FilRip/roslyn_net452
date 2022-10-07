Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Structure AnonymousTypeField
		Public ReadOnly Name As String

		Public ReadOnly Location As Microsoft.CodeAnalysis.Location

		Private _type As TypeSymbol

		Public ReadOnly IsKey As Boolean

		Public ReadOnly Property IsByRef As Boolean
			Get
				Return Me.IsKey
			End Get
		End Property

		Public ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Public Sub New(ByVal name As String, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, Optional ByVal isKeyOrByRef As Boolean = False)
			Me = New AnonymousTypeField() With
			{
				.Name = If([String].IsNullOrWhiteSpace(name), "<Empty Name>", name),
				._type = type,
				.IsKey = isKeyOrByRef,
				.Location = location
			}
		End Sub

		Public Sub New(ByVal name As String, ByVal location As Microsoft.CodeAnalysis.Location, ByVal isKey As Boolean)
			MyClass.New(name, Nothing, location, isKey)
		End Sub

		<Conditional("DEBUG")>
		Friend Sub AssertGood()
		End Sub

		Friend Sub AssignFieldType(ByVal newType As TypeSymbol)
			Me._type = newType
		End Sub
	End Structure
End Namespace