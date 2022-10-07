Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConditionalGoto
		Inherits BoundStatement
		Private ReadOnly _Condition As BoundExpression

		Private ReadOnly _JumpIfTrue As Boolean

		Private ReadOnly _Label As LabelSymbol

		Public ReadOnly Property Condition As BoundExpression
			Get
				Return Me._Condition
			End Get
		End Property

		Public ReadOnly Property JumpIfTrue As Boolean
			Get
				Return Me._JumpIfTrue
			End Get
		End Property

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal condition As BoundExpression, ByVal jumpIfTrue As Boolean, ByVal label As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ConditionalGoto, syntax, If(hasErrors, True, condition.NonNullAndHasErrors()))
			Me._Condition = condition
			Me._JumpIfTrue = jumpIfTrue
			Me._Label = label
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConditionalGoto(Me)
		End Function

		Public Function Update(ByVal condition As BoundExpression, ByVal jumpIfTrue As Boolean, ByVal label As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto
			Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto
			If (condition <> Me.Condition OrElse jumpIfTrue <> Me.JumpIfTrue OrElse CObj(label) <> CObj(Me.Label)) Then
				Dim boundConditionalGoto1 As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(MyBase.Syntax, condition, jumpIfTrue, label, MyBase.HasErrors)
				boundConditionalGoto1.CopyAttributes(Me)
				boundConditionalGoto = boundConditionalGoto1
			Else
				boundConditionalGoto = Me
			End If
			Return boundConditionalGoto
		End Function
	End Class
End Namespace