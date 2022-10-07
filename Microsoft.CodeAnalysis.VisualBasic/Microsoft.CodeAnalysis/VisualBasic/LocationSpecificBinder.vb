Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LocationSpecificBinder
		Inherits Binder
		Private ReadOnly _location As Microsoft.CodeAnalysis.VisualBasic.BindingLocation

		Private ReadOnly _owner As Symbol

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property BindingLocation As Microsoft.CodeAnalysis.VisualBasic.BindingLocation
			Get
				Return Me._location
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return If(Me._owner, MyBase.ContainingMember)
			End Get
		End Property

		Public Sub New(ByVal location As Microsoft.CodeAnalysis.VisualBasic.BindingLocation, ByVal containingBinder As Binder)
			MyClass.New(location, Nothing, containingBinder)
		End Sub

		Public Sub New(ByVal location As Microsoft.CodeAnalysis.VisualBasic.BindingLocation, ByVal owner As Symbol, ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
			Me._location = location
			Me._owner = owner
		End Sub
	End Class
End Namespace