Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNewT
		Inherits BoundObjectCreationExpressionBase
		Public Sub New(ByVal syntax As SyntaxNode, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.NewT, syntax, initializerOpt, type, If(hasErrors, True, initializerOpt.NonNullAndHasErrors()))
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNewT(Me)
		End Function

		Public Function Update(ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundNewT
			Dim boundNewT As Microsoft.CodeAnalysis.VisualBasic.BoundNewT
			If (initializerOpt <> MyBase.InitializerOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundNewT1 As Microsoft.CodeAnalysis.VisualBasic.BoundNewT = New Microsoft.CodeAnalysis.VisualBasic.BoundNewT(MyBase.Syntax, initializerOpt, type, MyBase.HasErrors)
				boundNewT1.CopyAttributes(Me)
				boundNewT = boundNewT1
			Else
				boundNewT = Me
			End If
			Return boundNewT
		End Function
	End Class
End Namespace