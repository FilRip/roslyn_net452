Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class StatementSyntaxWalker
		Inherits VisualBasicSyntaxVisitor
		Public Sub New()
			MyBase.New()
		End Sub

		Public Overrides Sub VisitAccessorBlock(ByVal node As AccessorBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitCaseBlock(ByVal node As CaseBlockSyntax)
			Me.Visit(node.CaseStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitCatchBlock(ByVal node As CatchBlockSyntax)
			Me.Visit(node.CatchStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitClassBlock(ByVal node As ClassBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.[Inherits], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.[Implements], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitCompilationUnit(ByVal node As CompilationUnitSyntax)
			Me.VisitList(DirectCast(node.Options, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.[Imports], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.Attributes, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitConstructorBlock(ByVal node As ConstructorBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitDoLoopBlock(ByVal node As DoLoopBlockSyntax)
			Me.Visit(node.DoStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.LoopStatement)
		End Sub

		Public Overrides Sub VisitElseBlock(ByVal node As ElseBlockSyntax)
			Me.Visit(node.ElseStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitElseIfBlock(ByVal node As ElseIfBlockSyntax)
			Me.Visit(node.ElseIfStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitEnumBlock(ByVal node As EnumBlockSyntax)
			Me.Visit(node.EnumStatement)
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndEnumStatement)
		End Sub

		Public Overrides Sub VisitEventBlock(ByVal node As EventBlockSyntax)
			Me.Visit(node.EventStatement)
			Me.VisitList(DirectCast(node.Accessors, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndEventStatement)
		End Sub

		Public Overrides Sub VisitFinallyBlock(ByVal node As FinallyBlockSyntax)
			Me.Visit(node.FinallyStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitForBlock(ByVal node As ForBlockSyntax)
			Me.Visit(node.ForStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitForEachBlock(ByVal node As ForEachBlockSyntax)
			Me.Visit(node.ForEachStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitInterfaceBlock(ByVal node As InterfaceBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.[Inherits], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overridable Sub VisitList(ByVal list As IEnumerable(Of VisualBasicSyntaxNode))
			Dim enumerator As IEnumerator(Of VisualBasicSyntaxNode) = Nothing
			Try
				enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(enumerator.Current)
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Public Overrides Sub VisitMethodBlock(ByVal node As MethodBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitModuleBlock(ByVal node As ModuleBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitMultiLineIfBlock(ByVal node As MultiLineIfBlockSyntax)
			Me.Visit(node.IfStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.ElseIfBlocks, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.ElseBlock)
			Me.Visit(node.EndIfStatement)
		End Sub

		Public Overrides Sub VisitNamespaceBlock(ByVal node As NamespaceBlockSyntax)
			Me.Visit(node.NamespaceStatement)
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndNamespaceStatement)
		End Sub

		Public Overrides Sub VisitOperatorBlock(ByVal node As OperatorBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitPropertyBlock(ByVal node As PropertyBlockSyntax)
			Me.Visit(node.PropertyStatement)
			Me.VisitList(DirectCast(node.Accessors, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndPropertyStatement)
		End Sub

		Public Overrides Sub VisitSelectBlock(ByVal node As SelectBlockSyntax)
			Me.Visit(node.SelectStatement)
			Me.VisitList(DirectCast(node.CaseBlocks, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndSelectStatement)
		End Sub

		Public Overrides Sub VisitSingleLineElseClause(ByVal node As SingleLineElseClauseSyntax)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
		End Sub

		Public Overrides Sub VisitSingleLineIfStatement(ByVal node As SingleLineIfStatementSyntax)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.ElseClause)
		End Sub

		Public Overrides Sub VisitStructureBlock(ByVal node As StructureBlockSyntax)
			Me.Visit(node.BlockStatement)
			Me.VisitList(DirectCast(node.[Inherits], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.[Implements], IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.Members, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndBlockStatement)
		End Sub

		Public Overrides Sub VisitSyncLockBlock(ByVal node As SyncLockBlockSyntax)
			Me.Visit(node.SyncLockStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndSyncLockStatement)
		End Sub

		Public Overrides Sub VisitTryBlock(ByVal node As TryBlockSyntax)
			Me.Visit(node.TryStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.VisitList(DirectCast(node.CatchBlocks, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.FinallyBlock)
			Me.Visit(node.EndTryStatement)
		End Sub

		Public Overrides Sub VisitUsingBlock(ByVal node As UsingBlockSyntax)
			Me.Visit(node.UsingStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndUsingStatement)
		End Sub

		Public Overrides Sub VisitWhileBlock(ByVal node As WhileBlockSyntax)
			Me.Visit(node.WhileStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndWhileStatement)
		End Sub

		Public Overrides Sub VisitWithBlock(ByVal node As WithBlockSyntax)
			Me.Visit(node.WithStatement)
			Me.VisitList(DirectCast(node.Statements, IEnumerable(Of VisualBasicSyntaxNode)))
			Me.Visit(node.EndWithStatement)
		End Sub
	End Class
End Namespace