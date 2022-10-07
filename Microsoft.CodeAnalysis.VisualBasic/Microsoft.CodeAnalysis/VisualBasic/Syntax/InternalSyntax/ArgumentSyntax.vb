Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class ArgumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend Sub New(ByVal kind As SyntaxKind)
			MyBase.New(kind)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation())
			MyBase.New(kind, errors, annotations)
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
		End Sub

		Public Function GetExpression() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Dim kind As SyntaxKind = MyBase.Kind
			If (kind = SyntaxKind.OmittedArgument) Then
				expressionSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingExpression()
			Else
				expressionSyntax = If(kind = SyntaxKind.RangeArgument, DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax).UpperBound, DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax).Expression)
			End If
			Return expressionSyntax
		End Function
	End Class
End Namespace