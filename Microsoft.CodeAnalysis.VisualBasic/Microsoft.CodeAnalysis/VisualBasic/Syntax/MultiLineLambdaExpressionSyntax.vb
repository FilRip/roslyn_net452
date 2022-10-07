Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MultiLineLambdaExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax
		Friend _statements As SyntaxNode

		Friend _endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Get
				Return Me.SubOrFunctionHeader
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndClassStatement) instead.", True)>
		Public ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubOrFunctionStatement
			End Get
		End Property

		Public ReadOnly Property EndSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endSubOrFunctionStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public Shadows ReadOnly Property SubOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax)(Me._subOrFunctionHeader)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax, ByVal statements As SyntaxNode, ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax(kind, errors, annotations, DirectCast(subOrFunctionHeader.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endSubOrFunctionStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMultiLineLambdaExpression(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMultiLineLambdaExpression(Me)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._subOrFunctionHeader
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endSubOrFunctionStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim subOrFunctionHeader As SyntaxNode
			Select Case i
				Case 0
					subOrFunctionHeader = Me.SubOrFunctionHeader
					Exit Select
				Case 1
					subOrFunctionHeader = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					subOrFunctionHeader = Me.EndSubOrFunctionStatement
					Exit Select
				Case Else
					subOrFunctionHeader = Nothing
					Exit Select
			End Select
			Return subOrFunctionHeader
		End Function

		Friend Overrides Function GetSubOrFunctionHeaderCore() As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax
			Return Me.SubOrFunctionHeader
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Dim multiLineLambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			If (kind <> MyBase.Kind() OrElse subOrFunctionHeader <> Me.SubOrFunctionHeader OrElse statements <> Me.Statements OrElse endSubOrFunctionStatement <> Me.EndSubOrFunctionStatement) Then
				Dim multiLineLambdaExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MultiLineLambdaExpression(kind, subOrFunctionHeader, statements, endSubOrFunctionStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				multiLineLambdaExpressionSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, multiLineLambdaExpressionSyntax1, multiLineLambdaExpressionSyntax1.WithAnnotations(annotations))
			Else
				multiLineLambdaExpressionSyntax = Me
			End If
			Return multiLineLambdaExpressionSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithClassStatement) instead.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.WithSubOrFunctionHeader(begin)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndClassStatement) instead.", True)>
		Public Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.WithEndSubOrFunctionStatement([end])
		End Function

		Public Function WithEndSubOrFunctionStatement(ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.SubOrFunctionHeader, Me.Statements, endSubOrFunctionStatement)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.Update(MyBase.Kind(), Me.SubOrFunctionHeader, statements, Me.EndSubOrFunctionStatement)
		End Function

		Public Shadows Function WithSubOrFunctionHeader(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineLambdaExpressionSyntax
			Return Me.Update(MyBase.Kind(), subOrFunctionHeader, Me.Statements, Me.EndSubOrFunctionStatement)
		End Function

		Friend Overrides Function WithSubOrFunctionHeaderCore(ByVal subOrFunctionHeader As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax
			Return Me.WithSubOrFunctionHeader(subOrFunctionHeader)
		End Function
	End Class
End Namespace