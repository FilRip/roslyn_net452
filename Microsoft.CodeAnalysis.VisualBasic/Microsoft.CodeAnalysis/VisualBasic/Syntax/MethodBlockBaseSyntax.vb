Imports Microsoft.CodeAnalysis
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class MethodBlockBaseSyntax
		Inherits DeclarationStatementSyntax
		Friend _statements As SyntaxNode

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use BlockStatement or a more specific property (e.g. SubOrFunctionStatement) instead.", True)>
		Public ReadOnly Property Begin As MethodBaseSyntax
			Get
				Return Me.BlockStatement
			End Get
		End Property

		Public MustOverride ReadOnly Property BlockStatement As MethodBaseSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndSubOrFunctionStatement) instead.", True)>
		Public ReadOnly Property [End] As EndBlockStatementSyntax
			Get
				Return Me.EndBlockStatement
			End Get
		End Property

		Public MustOverride ReadOnly Property EndBlockStatement As EndBlockStatementSyntax

		Public ReadOnly Property Statements As SyntaxList(Of StatementSyntax)
			Get
				Return Me.GetStatementsCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddStatements(ByVal ParamArray items As StatementSyntax()) As MethodBlockBaseSyntax
			Return Me.AddStatementsCore(items)
		End Function

		Friend MustOverride Function AddStatementsCore(ByVal ParamArray items As StatementSyntax()) As MethodBlockBaseSyntax

		Friend Overridable Function GetStatementsCore() As SyntaxList(Of StatementSyntax)
			Return New SyntaxList(Of StatementSyntax)(MyBase.GetRedAtZero(Me._statements))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithSubOrFunctionStatement) instead.", True)>
		Public Function WithBegin(ByVal begin As MethodBaseSyntax) As MethodBlockBaseSyntax
			Return Me.WithBlockStatement(begin)
		End Function

		Public MustOverride Function WithBlockStatement(ByVal blockStatement As MethodBaseSyntax) As MethodBlockBaseSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndSubOrFunctionStatement) instead.", True)>
		Public Function WithEnd(ByVal [end] As EndBlockStatementSyntax) As MethodBlockBaseSyntax
			Return Me.WithEndBlockStatement([end])
		End Function

		Public MustOverride Function WithEndBlockStatement(ByVal endBlockStatement As EndBlockStatementSyntax) As MethodBlockBaseSyntax

		Public Function WithStatements(ByVal statements As SyntaxList(Of StatementSyntax)) As MethodBlockBaseSyntax
			Return Me.WithStatementsCore(statements)
		End Function

		Friend MustOverride Function WithStatementsCore(ByVal statements As SyntaxList(Of StatementSyntax)) As MethodBlockBaseSyntax
	End Class
End Namespace