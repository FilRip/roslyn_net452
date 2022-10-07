Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTupleExpression
		Inherits BoundExpression
		Private ReadOnly _Arguments As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property Arguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Arguments
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, type, hasErrors)
			Me._Arguments = arguments
		End Sub
	End Class
End Namespace