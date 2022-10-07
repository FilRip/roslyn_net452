Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundOmittedArgument
		Inherits BoundExpression
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.OmittedArgument, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.OmittedArgument, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitOmittedArgument(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundOmittedArgument
			Dim boundOmittedArgument As Microsoft.CodeAnalysis.VisualBasic.BoundOmittedArgument
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundOmittedArgument = Me
			Else
				Dim boundOmittedArgument1 As Microsoft.CodeAnalysis.VisualBasic.BoundOmittedArgument = New Microsoft.CodeAnalysis.VisualBasic.BoundOmittedArgument(MyBase.Syntax, type, MyBase.HasErrors)
				boundOmittedArgument1.CopyAttributes(Me)
				boundOmittedArgument = boundOmittedArgument1
			End If
			Return boundOmittedArgument
		End Function
	End Class
End Namespace