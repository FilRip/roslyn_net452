Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundObjectInitializerExpressionBase
		Inherits BoundExpression
		Private ReadOnly _PlaceholderOpt As BoundWithLValueExpressionPlaceholder

		Private ReadOnly _Initializers As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property Initializers As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Initializers
			End Get
		End Property

		Public ReadOnly Property PlaceholderOpt As BoundWithLValueExpressionPlaceholder
			Get
				Return Me._PlaceholderOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal placeholderOpt As BoundWithLValueExpressionPlaceholder, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, type, hasErrors)
			Me._PlaceholderOpt = placeholderOpt
			Me._Initializers = initializers
		End Sub
	End Class
End Namespace