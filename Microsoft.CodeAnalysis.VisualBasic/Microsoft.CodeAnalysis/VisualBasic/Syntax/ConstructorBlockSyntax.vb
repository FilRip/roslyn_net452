Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ConstructorBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
		Friend _subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax

		Friend _endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Get
				Return Me.SubNewStatement
			End Get
		End Property

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax
			Get
				Return Me.SubNewStatement
			End Get
		End Property

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubStatement
			End Get
		End Property

		Public ReadOnly Property EndSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endSubStatement, 2)
			End Get
		End Property

		Public Shadows ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 1))
			End Get
		End Property

		Public ReadOnly Property SubNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax)(Me._subNewStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax, ByVal statements As SyntaxNode, ByVal endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax(kind, errors, annotations, DirectCast(subNewStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax), If(statements IsNot Nothing, statements.Green, Nothing), DirectCast(endSubStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitConstructorBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitConstructorBlock(Me)
		End Sub

		Public Shadows Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function AddStatementsCore(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.AddStatements(items)
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._subNewStatement
					Exit Select
				Case 1
					syntaxNode = Me._statements
					Exit Select
				Case 2
					syntaxNode = Me._endSubStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim subNewStatement As SyntaxNode
			Select Case i
				Case 0
					subNewStatement = Me.SubNewStatement
					Exit Select
				Case 1
					subNewStatement = MyBase.GetRed(Me._statements, 1)
					Exit Select
				Case 2
					subNewStatement = Me.EndSubStatement
					Exit Select
				Case Else
					subNewStatement = Nothing
					Exit Select
			End Select
			Return subNewStatement
		End Function

		Friend Overrides Function GetStatementsCore() As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Return Me.Statements
		End Function

		Public Function Update(ByVal subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Dim constructorBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			If (subNewStatement <> Me.SubNewStatement OrElse statements <> Me.Statements OrElse endSubStatement <> Me.EndSubStatement) Then
				Dim constructorBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ConstructorBlock(subNewStatement, statements, endSubStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				constructorBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, constructorBlockSyntax1, constructorBlockSyntax1.WithAnnotations(annotations))
			Else
				constructorBlockSyntax = Me
			End If
			Return constructorBlockSyntax
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Function WithBegin(ByVal begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.WithSubNewStatement(begin)
		End Function

		Public Overrides Function WithBlockStatement(ByVal blockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithSubNewStatement(DirectCast(blockStatement, Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete.", True)>
		Public Shadows Function WithEnd(ByVal [end] As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.WithEndSubStatement([end])
		End Function

		Public Overrides Function WithEndBlockStatement(ByVal endBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithEndSubStatement(endBlockStatement)
		End Function

		Public Function WithEndSubStatement(ByVal endSubStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.Update(Me.SubNewStatement, Me.Statements, endSubStatement)
		End Function

		Public Shadows Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.Update(Me.SubNewStatement, statements, Me.EndSubStatement)
		End Function

		Friend Overrides Function WithStatementsCore(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax
			Return Me.WithStatements(statements)
		End Function

		Public Function WithSubNewStatement(ByVal subNewStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax
			Return Me.Update(subNewStatement, Me.Statements, Me.EndSubStatement)
		End Function
	End Class
End Namespace