Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class StatementWalker
		Inherits BoundTreeWalker
		Public Sub New()
			MyBase.New()
		End Sub

		Protected Overrides Function VisitExpressionWithoutStackGuard(ByVal node As BoundExpression) As BoundExpression
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace