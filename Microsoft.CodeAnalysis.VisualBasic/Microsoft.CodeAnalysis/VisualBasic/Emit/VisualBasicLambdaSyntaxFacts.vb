Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend Class VisualBasicLambdaSyntaxFacts
		Inherits LambdaSyntaxFacts
		Public ReadOnly Shared Instance As LambdaSyntaxFacts

		Shared Sub New()
			VisualBasicLambdaSyntaxFacts.Instance = New VisualBasicLambdaSyntaxFacts()
		End Sub

		Private Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function GetDeclaratorPosition(ByVal node As SyntaxNode) As Integer
			Return node.SpanStart
		End Function

		Public Overrides Function GetLambda(ByVal lambdaOrLambdaBodySyntax As SyntaxNode) As SyntaxNode
			Return LambdaUtilities.GetLambda(lambdaOrLambdaBodySyntax)
		End Function

		Public Overrides Function TryGetCorrespondingLambdaBody(ByVal previousLambdaSyntax As SyntaxNode, ByVal lambdaOrLambdaBodySyntax As SyntaxNode) As SyntaxNode
			Return LambdaUtilities.GetCorrespondingLambdaBody(lambdaOrLambdaBodySyntax, previousLambdaSyntax)
		End Function
	End Class
End Namespace