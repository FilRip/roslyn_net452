Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundObjectCreationExpressionBase
		Inherits BoundExpression
		Private ReadOnly _InitializerOpt As BoundObjectInitializerExpressionBase

		Public ReadOnly Property InitializerOpt As BoundObjectInitializerExpressionBase
			Get
				Return Me._InitializerOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, type, hasErrors)
			Me._InitializerOpt = initializerOpt
		End Sub
	End Class
End Namespace