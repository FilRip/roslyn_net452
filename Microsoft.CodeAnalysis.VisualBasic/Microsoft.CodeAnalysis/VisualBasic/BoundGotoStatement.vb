Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundGotoStatement
		Inherits BoundStatement
		Private ReadOnly _Label As LabelSymbol

		Private ReadOnly _LabelExpressionOpt As BoundLabel

		Public ReadOnly Property Label As LabelSymbol
			Get
				Return Me._Label
			End Get
		End Property

		Public ReadOnly Property LabelExpressionOpt As BoundLabel
			Get
				Return Me._LabelExpressionOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal labelExpressionOpt As BoundLabel, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.GotoStatement, syntax, If(hasErrors, True, labelExpressionOpt.NonNullAndHasErrors()))
			Me._Label = label
			Me._LabelExpressionOpt = labelExpressionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitGotoStatement(Me)
		End Function

		Public Function Update(ByVal label As LabelSymbol, ByVal labelExpressionOpt As BoundLabel) As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement
			Dim boundGotoStatement As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement
			If (CObj(label) <> CObj(Me.Label) OrElse labelExpressionOpt <> Me.LabelExpressionOpt) Then
				Dim boundGotoStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(MyBase.Syntax, label, labelExpressionOpt, MyBase.HasErrors)
				boundGotoStatement1.CopyAttributes(Me)
				boundGotoStatement = boundGotoStatement1
			Else
				boundGotoStatement = Me
			End If
			Return boundGotoStatement
		End Function
	End Class
End Namespace