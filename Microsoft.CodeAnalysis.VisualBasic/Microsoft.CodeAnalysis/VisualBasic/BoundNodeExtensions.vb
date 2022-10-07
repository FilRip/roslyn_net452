Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module BoundNodeExtensions
		<Extension>
		Public Function GetBinderFromLambda(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim kind As BoundKind = boundNode.Kind
			If (kind = BoundKind.UnboundLambda) Then
				binder = DirectCast(boundNode, UnboundLambda).Binder
			ElseIf (kind = BoundKind.QueryLambda) Then
				binder = DirectCast(boundNode, BoundQueryLambda).LambdaSymbol.ContainingBinder
			ElseIf (kind = BoundKind.GroupTypeInferenceLambda) Then
				binder = DirectCast(boundNode, GroupTypeInferenceLambda).Binder
			Else
				binder = Nothing
			End If
			Return binder
		End Function

		<Extension>
		Public Function IsAnyLambda(ByVal boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Boolean
			Dim kind As BoundKind = boundNode.Kind
			If (kind = BoundKind.UnboundLambda OrElse kind = BoundKind.Lambda OrElse kind = BoundKind.QueryLambda) Then
				Return True
			End If
			Return kind = BoundKind.GroupTypeInferenceLambda
		End Function

		<Extension>
		Public Function MakeCompilerGenerated(Of T As BoundNode)(ByVal this As T) As T
			this.SetWasCompilerGenerated()
			Return this
		End Function

		<Extension>
		Public Function NonNullAndHasErrors(Of T As BoundNode)(ByVal nodeArray As ImmutableArray(Of T)) As Boolean
			Dim flag As Boolean
			If (Not nodeArray.IsDefault) Then
				Dim length As Integer = nodeArray.Length - 1
				Dim num As Integer = 0
				While num <= length
					If (Not nodeArray(num).HasErrors) Then
						num = num + 1
					Else
						flag = True
						Return flag
					End If
				End While
				flag = False
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Public Function NonNullAndHasErrors(ByVal node As BoundNode) As Boolean
			If (node Is Nothing) Then
				Return False
			End If
			Return node.HasErrors
		End Function
	End Module
End Namespace