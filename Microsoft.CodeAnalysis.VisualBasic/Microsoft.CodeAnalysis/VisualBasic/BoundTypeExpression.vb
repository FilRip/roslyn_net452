Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTypeExpression
		Inherits BoundExpression
		Private ReadOnly _UnevaluatedReceiverOpt As BoundExpression

		Private ReadOnly _AliasOpt As AliasSymbol

		Public ReadOnly Property AliasOpt As AliasSymbol
			Get
				Return Me._AliasOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Dim aliasOpt As [Object] = Me.AliasOpt
				If (aliasOpt Is Nothing) Then
					aliasOpt = MyBase.Type
				End If
				Return aliasOpt
			End Get
		End Property

		Public ReadOnly Property UnevaluatedReceiverOpt As BoundExpression
			Get
				Return Me._UnevaluatedReceiverOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, Nothing, Nothing, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal unevaluatedReceiverOpt As BoundExpression, ByVal aliasOpt As AliasSymbol, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.TypeExpression, syntax, type, If(hasErrors, True, unevaluatedReceiverOpt.NonNullAndHasErrors()))
			Me._UnevaluatedReceiverOpt = unevaluatedReceiverOpt
			Me._AliasOpt = aliasOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTypeExpression(Me)
		End Function

		Public Function Update(ByVal unevaluatedReceiverOpt As BoundExpression, ByVal aliasOpt As AliasSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression
			Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression
			If (unevaluatedReceiverOpt <> Me.UnevaluatedReceiverOpt OrElse CObj(aliasOpt) <> CObj(Me.AliasOpt) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTypeExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression(MyBase.Syntax, unevaluatedReceiverOpt, aliasOpt, type, MyBase.HasErrors)
				boundTypeExpression1.CopyAttributes(Me)
				boundTypeExpression = boundTypeExpression1
			Else
				boundTypeExpression = Me
			End If
			Return boundTypeExpression
		End Function
	End Class
End Namespace