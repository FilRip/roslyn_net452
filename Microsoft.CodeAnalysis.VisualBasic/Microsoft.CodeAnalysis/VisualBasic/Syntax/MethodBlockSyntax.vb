Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class MethodBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
		Friend _subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax

		Friend _endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax
			Get
				Return Me.SubOrFunctionStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Get
				Return Me.SubOrFunctionStatement
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubOrFunctionStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubOrFunctionStatement
			End Get
		End Property

		Public ReadOnly Property EndSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endSubOrFunctionStatement, 2)
			End Get
		End Property

		Public Shadows ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property SubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax)(Me._subOrFunctionStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax, ByVal statements As SyntaxNode, ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(kind, errors, annotations, DirectCast(subOrFunctionStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endSubOrFunctionStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitMethodBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitMethodBlock(Me)
		End Sub

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._subOrFunctionStatement
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
			Dim subOrFunctionStatement As SyntaxNode
			Select Case i
				Case 0
					subOrFunctionStatement = Me.SubOrFunctionStatement
					Exit Select
				Case 1
					subOrFunctionStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					subOrFunctionStatement = Me.EndSubOrFunctionStatement
					Exit Select
				Case Else
					subOrFunctionStatement = Nothing
					Exit Select
			End Select
			Return subOrFunctionStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Dim methodBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			If (kind <> MyBase.Kind() OrElse subOrFunctionStatement <> Me.SubOrFunctionStatement OrElse statements <> Me.Statements OrElse endSubOrFunctionStatement <> Me.EndSubOrFunctionStatement) Then
				Dim methodBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.MethodBlock(kind, subOrFunctionStatement, statements, endSubOrFunctionStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				methodBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, methodBlockSyntax1, methodBlockSyntax1.WithAnnotations(annotations))
			Else
				methodBlockSyntax = Me
			End If
			Return methodBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.WithSubOrFunctionStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithSubOrFunctionStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.WithEndSubOrFunctionStatement([end])
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithEndSubOrFunctionStatement(endBlockStatement)
		End Function

		Public Function WithEndSubOrFunctionStatement(ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.SubOrFunctionStatement, Me.Statements, endSubOrFunctionStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.SubOrFunctionStatement, statements, Me.EndSubOrFunctionStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithStatements(statements)
		End Function

		Public Function WithSubOrFunctionStatement(ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax
			Return Me.Update(MyBase.Kind(), subOrFunctionStatement, Me.Statements, Me.EndSubOrFunctionStatement)
		End Function
	End Class
End Namespace