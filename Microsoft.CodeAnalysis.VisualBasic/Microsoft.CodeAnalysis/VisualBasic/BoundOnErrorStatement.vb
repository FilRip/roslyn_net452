Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundOnErrorStatement
		Inherits BoundStatement
		Private ReadOnly _OnErrorKind As OnErrorStatementKind

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

		Public ReadOnly Property OnErrorKind As OnErrorStatementKind
			Get
				Return Me._OnErrorKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal label As LabelSymbol, ByVal labelExpressionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, OnErrorStatementKind.GoToLabel, label, labelExpressionOpt, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal onErrorKind As OnErrorStatementKind, ByVal labelOpt As LabelSymbol, ByVal labelExpressionOpt As BoundExpression, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.OnErrorStatement, syntax, If(hasErrors, True, labelExpressionOpt.NonNullAndHasErrors()))
			Me._OnErrorKind = onErrorKind
			Me._LabelOpt = labelOpt
			Me._LabelExpressionOpt = labelExpressionOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitOnErrorStatement(Me)
		End Function

		Public Function Update(ByVal onErrorKind As OnErrorStatementKind, ByVal labelOpt As LabelSymbol, ByVal labelExpressionOpt As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundOnErrorStatement
			Dim boundOnErrorStatement As Microsoft.CodeAnalysis.VisualBasic.BoundOnErrorStatement
			If (onErrorKind <> Me.OnErrorKind OrElse CObj(labelOpt) <> CObj(Me.LabelOpt) OrElse labelExpressionOpt <> Me.LabelExpressionOpt) Then
				Dim boundOnErrorStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundOnErrorStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundOnErrorStatement(MyBase.Syntax, onErrorKind, labelOpt, labelExpressionOpt, MyBase.HasErrors)
				boundOnErrorStatement1.CopyAttributes(Me)
				boundOnErrorStatement = boundOnErrorStatement1
			Else
				boundOnErrorStatement = Me
			End If
			Return boundOnErrorStatement
		End Function
	End Class
End Namespace