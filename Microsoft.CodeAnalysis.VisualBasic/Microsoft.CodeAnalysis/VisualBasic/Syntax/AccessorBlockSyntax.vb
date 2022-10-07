Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AccessorBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
		Friend _accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax

		Friend _endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property AccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax)(Me._accessorStatement)
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use AccessorStatement instead.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax
			Get
				Return Me.AccessorStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Get
				Return Me.AccessorStatement
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use EndAccessorStatement instead.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndAccessorStatement
			End Get
		End Property

		Public ReadOnly Property EndAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endAccessorStatement, 2)
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndAccessorStatement
			End Get
		End Property

		Public Shadows ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax, ByVal statements As SyntaxNode, ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax(kind, errors, annotations, DirectCast(accessorStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endAccessorStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAccessorBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAccessorBlock(Me)
		End Sub

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._accessorStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endAccessorStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim accessorStatement As SyntaxNode
			Select Case i
				Case 0
					accessorStatement = Me.AccessorStatement
					Exit Select
				Case 1
					accessorStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					accessorStatement = Me.EndAccessorStatement
					Exit Select
				Case Else
					accessorStatement = Nothing
					Exit Select
			End Select
			Return accessorStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Dim accessorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			If (kind <> MyBase.Kind() OrElse accessorStatement <> Me.AccessorStatement OrElse statements <> Me.Statements OrElse endAccessorStatement <> Me.EndAccessorStatement) Then
				Dim accessorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AccessorBlock(kind, accessorStatement, statements, endAccessorStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				accessorBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, accessorBlockSyntax1, accessorBlockSyntax1.WithAnnotations(annotations))
			Else
				accessorBlockSyntax = Me
			End If
			Return accessorBlockSyntax
		End Function

		Public Function WithAccessorStatement(ByVal accessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.Update(MyBase.Kind(), accessorStatement, Me.Statements, Me.EndAccessorStatement)
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithAccessorStatement instead.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.WithAccessorStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithAccessorStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorStatementSyntax))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithEndAccessorStatement instead.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.WithEndAccessorStatement([end])
		End Function

		Public Function WithEndAccessorStatement(ByVal endAccessorStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.AccessorStatement, Me.Statements, endAccessorStatement)
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithEndAccessorStatement(endBlockStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax
			Return Me.Update(MyBase.Kind(), Me.AccessorStatement, statements, Me.EndAccessorStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithStatements(statements)
		End Function
	End Class
End Namespace