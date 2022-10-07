Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNodeFinder
		Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		Private ReadOnly _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException As Boolean

		Private _nodeToFind As BoundNode

		Private Sub New(ByVal _nodeToFind As BoundNode, ByVal recursionDepth As Integer, ByVal convertInsufficientExecutionStackExceptionToCancelledByStackGuardException As Boolean)
			MyBase.New(recursionDepth)
			Me._nodeToFind = _nodeToFind
			Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = convertInsufficientExecutionStackExceptionToCancelledByStackGuardException
		End Sub

		Public Shared Function ContainsNode(ByVal findWhere As BoundNode, ByVal findWhat As BoundNode, ByVal recursionDepth As Integer, ByVal convertInsufficientExecutionStackExceptionToCancelledByStackGuardException As Boolean) As Boolean
			Dim flag As Boolean
			If (findWhere <> findWhat) Then
				Dim boundNodeFinder As Microsoft.CodeAnalysis.VisualBasic.BoundNodeFinder = New Microsoft.CodeAnalysis.VisualBasic.BoundNodeFinder(findWhat, recursionDepth, convertInsufficientExecutionStackExceptionToCancelledByStackGuardException)
				boundNodeFinder.Visit(findWhere)
				flag = boundNodeFinder._nodeToFind Is Nothing
			Else
				flag = True
			End If
			Return flag
		End Function

		Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
			Return Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException
		End Function

		Public Overrides Function Visit(ByVal node As BoundNode) As BoundNode
			If (Me._nodeToFind IsNot Nothing) Then
				If (Me._nodeToFind <> node) Then
					MyBase.Visit(node)
				Else
					Me._nodeToFind = Nothing
				End If
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
			Me.Visit(node.BindForErrorRecovery())
			Return Nothing
		End Function
	End Class
End Namespace