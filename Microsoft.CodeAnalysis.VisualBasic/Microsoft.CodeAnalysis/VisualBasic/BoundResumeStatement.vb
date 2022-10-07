Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundResumeStatement
		Inherits BoundStatement
		Private ReadOnly _ResumeKind As ResumeStatementKind

		Private ReadOnly _LabelOpt As LabelSymbol

		Private ReadOnly _LabelExpressionOpt As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.LabelExpressionOpt)
			End Get
		End Property

		Public ReadOnly Property LabelExpressionOpt As BoundExpression
			Get
				Return Me._LabelExpressionOpt
			End Get
		End Property

		Public ReadOnly Property LabelOpt As LabelSymbol
			Get
				Return Me._LabelOpt
			End Get
		End Property

		Public ReadOnly Property ResumeKind As ResumeStatementKind
			Get
				Return Me._ResumeKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, Optional ByVal isNext As Boolean = False)
			MyClass.New(syntax, If(isNext, ResumeStatementKind.[Next], ResumeStatementKind.Plain), Nothing, Nothing, False)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal labelExpressionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, ResumeStatementKind.Label, label, labelExpressionOpt, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal resumeKind As ResumeStatementKind, ByVal labelOpt As LabelSymbol, ByVal labelExpressionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ResumeStatement, syntax, If(hasErrors, True, labelExpressionOpt.NonNullAndHasErrors()))
			Me._ResumeKind = resumeKind
			Me._LabelOpt = labelOpt
			Me._LabelExpressionOpt = labelExpressionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitResumeStatement(Me)
		End Function

		Public Function Update(ByVal resumeKind As ResumeStatementKind, ByVal labelOpt As LabelSymbol, ByVal labelExpressionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundResumeStatement
			Dim boundResumeStatement As Microsoft.CodeAnalysis.VisualBasic.BoundResumeStatement
			If (resumeKind <> Me.ResumeKind OrElse CObj(labelOpt) <> CObj(Me.LabelOpt) OrElse labelExpressionOpt <> Me.LabelExpressionOpt) Then
				Dim boundResumeStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundResumeStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundResumeStatement(MyBase.Syntax, resumeKind, labelOpt, labelExpressionOpt, MyBase.HasErrors)
				boundResumeStatement1.CopyAttributes(Me)
				boundResumeStatement = boundResumeStatement1
			Else
				boundResumeStatement = Me
			End If
			Return boundResumeStatement
		End Function
	End Class
End Namespace