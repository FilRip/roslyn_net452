Imports Microsoft.CodeAnalysis
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class TypeBlockSyntax
		Inherits DeclarationStatementSyntax
		Friend _inherits As SyntaxNode

		Friend _implements As SyntaxNode

		Friend _members As SyntaxNode

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use BlockStatement or a more specific property (e.g. ClassStatement) instead.", True)>
		Public ReadOnly Property Begin As TypeStatementSyntax
			Get
				Return Me.BlockStatement
			End Get
		End Property

		Public MustOverride ReadOnly Property BlockStatement As TypeStatementSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use EndBlockStatement or a more specific property (e.g. EndClassStatement) instead.", True)>
		Public ReadOnly Property [End] As EndBlockStatementSyntax
			Get
				Return Me.EndBlockStatement
			End Get
		End Property

		Public MustOverride ReadOnly Property EndBlockStatement As EndBlockStatementSyntax

		Public ReadOnly Property [Implements] As SyntaxList(Of ImplementsStatementSyntax)
			Get
				Return Me.GetImplementsCore()
			End Get
		End Property

		Public ReadOnly Property [Inherits] As SyntaxList(Of InheritsStatementSyntax)
			Get
				Return Me.GetInheritsCore()
			End Get
		End Property

		Public ReadOnly Property Members As SyntaxList(Of StatementSyntax)
			Get
				Return Me.GetMembersCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Public Function AddImplements(ByVal ParamArray items As ImplementsStatementSyntax()) As TypeBlockSyntax
			Return Me.AddImplementsCore(items)
		End Function

		Friend MustOverride Function AddImplementsCore(ByVal ParamArray items As ImplementsStatementSyntax()) As TypeBlockSyntax

		Public Function AddInherits(ByVal ParamArray items As InheritsStatementSyntax()) As TypeBlockSyntax
			Return Me.AddInheritsCore(items)
		End Function

		Friend MustOverride Function AddInheritsCore(ByVal ParamArray items As InheritsStatementSyntax()) As TypeBlockSyntax

		Public Function AddMembers(ByVal ParamArray items As StatementSyntax()) As TypeBlockSyntax
			Return Me.AddMembersCore(items)
		End Function

		Friend MustOverride Function AddMembersCore(ByVal ParamArray items As StatementSyntax()) As TypeBlockSyntax

		Friend Overridable Function GetImplementsCore() As SyntaxList(Of ImplementsStatementSyntax)
			Return New SyntaxList(Of ImplementsStatementSyntax)(MyBase.GetRed(Me._implements, 1))
		End Function

		Friend Overridable Function GetInheritsCore() As SyntaxList(Of InheritsStatementSyntax)
			Return New SyntaxList(Of InheritsStatementSyntax)(MyBase.GetRedAtZero(Me._inherits))
		End Function

		Friend Overridable Function GetMembersCore() As SyntaxList(Of StatementSyntax)
			Return New SyntaxList(Of StatementSyntax)(MyBase.GetRed(Me._members, 2))
		End Function

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithBlockStatement or a more specific property (e.g. WithClassStatement) instead.", True)>
		Public Function WithBegin(ByVal begin As TypeStatementSyntax) As TypeBlockSyntax
			Return Me.WithBlockStatement(begin)
		End Function

		Public MustOverride Function WithBlockStatement(ByVal blockStatement As TypeStatementSyntax) As TypeBlockSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use WithEndBlockStatement or a more specific property (e.g. WithEndClassStatement) instead.", True)>
		Public Function WithEnd(ByVal [end] As EndBlockStatementSyntax) As TypeBlockSyntax
			Return Me.WithEndBlockStatement([end])
		End Function

		Public MustOverride Function WithEndBlockStatement(ByVal endBlockStatement As EndBlockStatementSyntax) As TypeBlockSyntax

		Public Function WithImplements(ByVal [implements] As SyntaxList(Of ImplementsStatementSyntax)) As TypeBlockSyntax
			Return Me.WithImplementsCore([implements])
		End Function

		Friend MustOverride Function WithImplementsCore(ByVal [implements] As SyntaxList(Of ImplementsStatementSyntax)) As TypeBlockSyntax

		Public Function WithInherits(ByVal [inherits] As SyntaxList(Of InheritsStatementSyntax)) As TypeBlockSyntax
			Return Me.WithInheritsCore([inherits])
		End Function

		Friend MustOverride Function WithInheritsCore(ByVal [inherits] As SyntaxList(Of InheritsStatementSyntax)) As TypeBlockSyntax

		Public Function WithMembers(ByVal members As SyntaxList(Of StatementSyntax)) As TypeBlockSyntax
			Return Me.WithMembersCore(members)
		End Function

		Friend MustOverride Function WithMembersCore(ByVal members As SyntaxList(Of StatementSyntax)) As TypeBlockSyntax
	End Class
End Namespace