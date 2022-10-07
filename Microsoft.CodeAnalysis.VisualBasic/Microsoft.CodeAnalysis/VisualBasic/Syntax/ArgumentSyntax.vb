Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class ArgumentSyntax
		Inherits VisualBasicSyntaxNode
		Public MustOverride ReadOnly Property IsNamed As Boolean

		Public ReadOnly Property IsOmitted As Boolean
			Get
				Return MyBase.Kind() = SyntaxKind.OmittedArgument
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public MustOverride Function GetExpression() As ExpressionSyntax
	End Class
End Namespace