Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundNoPiaObjectCreationExpression
		Inherits BoundObjectCreationExpressionBase
		Private ReadOnly _GuidString As String

		Public ReadOnly Property GuidString As String
			Get
				Return Me._GuidString
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal guidString As String, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.NoPiaObjectCreationExpression, syntax, initializerOpt, type, If(hasErrors, True, initializerOpt.NonNullAndHasErrors()))
			Me._GuidString = guidString
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitNoPiaObjectCreationExpression(Me)
		End Function

		Public Function Update(ByVal guidString As String, ByVal initializerOpt As BoundObjectInitializerExpressionBase, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundNoPiaObjectCreationExpression
			Dim boundNoPiaObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundNoPiaObjectCreationExpression
			If (CObj(guidString) <> CObj(Me.GuidString) OrElse initializerOpt <> MyBase.InitializerOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundNoPiaObjectCreationExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundNoPiaObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundNoPiaObjectCreationExpression(MyBase.Syntax, guidString, initializerOpt, type, MyBase.HasErrors)
				boundNoPiaObjectCreationExpression1.CopyAttributes(Me)
				boundNoPiaObjectCreationExpression = boundNoPiaObjectCreationExpression1
			Else
				boundNoPiaObjectCreationExpression = Me
			End If
			Return boundNoPiaObjectCreationExpression
		End Function
	End Class
End Namespace