Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Class TypeBlockContext
		Inherits DeclarationContext
		Protected _inheritsDecls As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)

		Private _implementsDecls As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)

		Protected _state As SyntaxKind

		Friend Sub New(ByVal contextKind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(contextKind, statement, prevContext)
			Me._state = SyntaxKind.None
		End Sub

		Friend Overrides Function CreateBlockSyntax(ByVal endStmt As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim typeStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax = Nothing
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(endStmt, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.GetBeginEndStatements(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)(typeStatementSyntax, endBlockStatementSyntax)
			If (Me._state <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._state
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
					Me._inheritsDecls = MyBase.BaseDeclarations(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)()
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement) Then
					Me._implementsDecls = MyBase.BaseDeclarations(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)()
				End If
				Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
			End If
			Dim typeBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TypeBlock(MyBase.BlockKind, typeStatementSyntax, Me._inheritsDecls, Me._implementsDecls, MyBase.Body(), endBlockStatementSyntax)
			MyBase.FreeStatements()
			Return typeBlockSyntax
		End Function

		Friend Overrides Function ProcessSyntax(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = Me._state
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None) Then
					Dim kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind
					If (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement
					ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement) Then
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement
					Else
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
					If (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement) Then
						Me._inheritsDecls = MyBase.BaseDeclarations(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)()
						Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement
					Else
						MyBase.Add(node)
						Exit While
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement) Then
					blockContext = MyBase.ProcessSyntax(node)
					Return blockContext
				ElseIf (node.Kind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement) Then
					Me._implementsDecls = MyBase.BaseDeclarations(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)()
					Me._state = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement
				Else
					MyBase.Add(node)
					Exit While
				End If
			End While
			blockContext = Me
			Return blockContext
		End Function
	End Class
End Namespace