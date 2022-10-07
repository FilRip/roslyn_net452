Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DeclarationInitializerBinder
		Inherits Binder
		Private ReadOnly _symbol As Symbol

		Private ReadOnly _additionalSymbols As ImmutableArray(Of Symbol)

		Private ReadOnly _root As VisualBasicSyntaxNode

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return Me._additionalSymbols
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return Me._symbol
			End Get
		End Property

		Friend ReadOnly Property Root As VisualBasicSyntaxNode
			Get
				Return Me._root
			End Get
		End Property

		Public Sub New(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal additionalSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal [next] As Binder, ByVal root As VisualBasicSyntaxNode)
			MyBase.New([next])
			Me._symbol = symbol
			Me._additionalSymbols = additionalSymbols
			Me._root = root
		End Sub

		Public Overrides Function GetBinder(ByVal node As SyntaxNode) As Binder
			Return Nothing
		End Function

		Public Overrides Function GetBinder(ByVal stmts As SyntaxList(Of StatementSyntax)) As Binder
			Return Nothing
		End Function
	End Class
End Namespace