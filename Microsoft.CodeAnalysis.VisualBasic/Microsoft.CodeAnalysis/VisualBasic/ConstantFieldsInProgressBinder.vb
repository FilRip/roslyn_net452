Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ConstantFieldsInProgressBinder
		Inherits Binder
		Private ReadOnly _inProgress As Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress

		Private ReadOnly _field As FieldSymbol

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ConstantFieldsInProgress As Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress
			Get
				Return Me._inProgress
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return Me._field
			End Get
		End Property

		Friend Sub New(ByVal inProgress As Microsoft.CodeAnalysis.VisualBasic.ConstantFieldsInProgress, ByVal [next] As Binder, ByVal field As FieldSymbol)
			MyBase.New([next])
			Me._inProgress = inProgress
			Me._field = field
		End Sub
	End Class
End Namespace