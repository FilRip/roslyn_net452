Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class EvaluatedConstant
		Public ReadOnly Shared None As EvaluatedConstant

		Public ReadOnly Value As ConstantValue

		Public ReadOnly Type As TypeSymbol

		Shared Sub New()
			EvaluatedConstant.None = New EvaluatedConstant(Nothing, Nothing)
		End Sub

		Public Sub New(ByVal value As ConstantValue, ByVal type As TypeSymbol)
			MyBase.New()
			Me.Value = value
			Me.Type = type
		End Sub
	End Class
End Namespace