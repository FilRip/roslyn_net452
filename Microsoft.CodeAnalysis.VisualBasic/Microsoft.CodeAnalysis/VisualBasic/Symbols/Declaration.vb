Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class Declaration
		Public ReadOnly Property Children As ImmutableArray(Of Declaration)
			Get
				Return Me.GetDeclarationChildren()
			End Get
		End Property

		Public MustOverride ReadOnly Property Kind As DeclarationKind

		Public Property Name As String

		Protected Sub New(ByVal name As String)
			MyBase.New()
			Me.Name = name
		End Sub

		Protected MustOverride Function GetDeclarationChildren() As ImmutableArray(Of Declaration)
	End Class
End Namespace