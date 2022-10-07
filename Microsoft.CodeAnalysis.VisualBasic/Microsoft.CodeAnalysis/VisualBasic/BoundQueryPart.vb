Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundQueryPart
		Inherits BoundExpression
		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(kind, syntax, type, hasErrors)
		End Sub

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(kind, syntax, type)
		End Sub
	End Class
End Namespace