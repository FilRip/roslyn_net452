Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundFieldOrPropertyInitializer
		Inherits BoundInitializer
		Private ReadOnly _MemberAccessExpressionOpt As BoundExpression

		Private ReadOnly _InitialValue As BoundExpression

		Public ReadOnly Property InitialValue As BoundExpression
			Get
				Return Me._InitialValue
			End Get
		End Property

		Public ReadOnly Property MemberAccessExpressionOpt As BoundExpression
			Get
				Return Me._MemberAccessExpressionOpt
			End Get
		End Property

		Protected Sub New(ByVal kind As BoundKind, ByVal syntax As SyntaxNode, ByVal memberAccessExpressionOpt As BoundExpression, ByVal initialValue As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(kind, syntax, hasErrors)
			Me._MemberAccessExpressionOpt = memberAccessExpressionOpt
			Me._InitialValue = initialValue
		End Sub
	End Class
End Namespace