Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class LocalBinderBuilder
		Inherits VisualBasicSyntaxVisitor
		Private _nodeMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder)

		Private _listMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)

		Private ReadOnly _enclosingMethod As MethodSymbol

		Private _containingBinder As Binder

		Public ReadOnly Property NodeToBinderMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder)
			Get
				Return Me._nodeMap
			End Get
		End Property

		Public ReadOnly Property StmtListToBinderMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)
			Get
				Return Me._listMap
			End Get
		End Property

		Public Sub New(ByVal enclosingMethod As MethodSymbol)
			MyBase.New()
			Me._enclosingMethod = enclosingMethod
			Me._nodeMap = ImmutableDictionary.Create(Of SyntaxNode, BlockBaseBinder)()
			Me._listMap = ImmutableDictionary.Create(Of SyntaxList(Of StatementSyntax), BlockBaseBinder)()
		End Sub

		Public Sub New(ByVal enclosingMethod As MethodSymbol, ByVal nodeMap As ImmutableDictionary(Of SyntaxNode, BlockBaseBinder), ByVal listMap As ImmutableDictionary(Of SyntaxList(Of StatementSyntax), BlockBaseBinder))
			MyBase.New()
			Me._enclosingMethod = enclosingMethod
			Me._nodeMap = nodeMap
			Me._listMap = listMap
		End Sub

		Private Sub CreateBinderFromStatementList(ByVal list As SyntaxList(Of StatementSyntax), ByVal outerBinder As Binder)
			Dim statementListBinder As Microsoft.CodeAnalysis.VisualBasic.StatementListBinder = New Microsoft.CodeAnalysis.VisualBasic.StatementListBinder(outerBinder, list)
			Me._listMap = Me._listMap.SetItem(list, statementListBinder)
			Me.VisitStatementsInList(DirectCast(list, IEnumerable(Of StatementSyntax)), statementListBinder)
		End Sub

		Public Sub MakeBinder(ByVal node As SyntaxNode, ByVal containingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me._containingBinder
			Me._containingBinder = containingBinder
			MyBase.Visit(node)
			Me._containingBinder = binder
		End Sub

		Private Sub RememberBinder(ByVal node As VisualBasicSyntaxNode, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			Me._nodeMap = Me._nodeMap.SetItem(node, DirectCast(binder, BlockBaseBinder))
		End Sub

		Public Overrides Sub VisitAccessorBlock(ByVal node As AccessorBlockSyntax)
			Me.VisitMethodBlockBase(node)
		End Sub

		Public Overrides Sub VisitCaseBlock(ByVal node As CaseBlockSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitCatchBlock(ByVal node As CatchBlockSyntax)
			Me._containingBinder = New CatchBlockBinder(Me._containingBinder, node)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitCompilationUnit(ByVal node As CompilationUnitSyntax)
			Dim enumerator As SyntaxList(Of StatementSyntax).Enumerator = node.Members.GetEnumerator()
			While enumerator.MoveNext()
				Me.MakeBinder(enumerator.Current, Me._containingBinder)
			End While
		End Sub

		Public Overrides Sub VisitConstructorBlock(ByVal node As ConstructorBlockSyntax)
			Me.VisitMethodBlockBase(node)
		End Sub

		Public Overrides Sub VisitDoLoopBlock(ByVal node As DoLoopBlockSyntax)
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, SyntaxKind.ContinueDoStatement, SyntaxKind.ExitDoStatement)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitElseBlock(ByVal node As ElseBlockSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitElseIfBlock(ByVal node As ElseIfBlockSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitFinallyBlock(ByVal node As FinallyBlockSyntax)
			Me._containingBinder = New FinallyBlockBinder(Me._containingBinder)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitForBlock(ByVal node As ForBlockSyntax)
			Me._containingBinder = New ForOrForEachBlockBinder(Me._containingBinder, node)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitForEachBlock(ByVal node As ForEachBlockSyntax)
			Me._containingBinder = New ForOrForEachBlockBinder(Me._containingBinder, node)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitMethodBlock(ByVal node As MethodBlockSyntax)
			Me.VisitMethodBlockBase(node)
		End Sub

		Private Sub VisitMethodBlockBase(ByVal methodBlock As MethodBlockBaseSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = methodBlock.BlockStatement.Kind()
			Select Case syntaxKind1
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
					Exit Select
				Case Else
					Select Case syntaxKind1
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement

						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
							Throw ExceptionUtilities.UnexpectedValue(methodBlock.BlockStatement.Kind())
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitPropertyStatement

						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement
						Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement
							syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement

						Case Else
							Throw ExceptionUtilities.UnexpectedValue(methodBlock.BlockStatement.Kind())
					End Select

			End Select
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None, syntaxKind)
			Me.RememberBinder(methodBlock, Me._containingBinder)
			Me.CreateBinderFromStatementList(methodBlock.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitMultiLineIfBlock(ByVal node As MultiLineIfBlockSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
			Dim enumerator As SyntaxList(Of ElseIfBlockSyntax).Enumerator = node.ElseIfBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Me.MakeBinder(enumerator.Current, Me._containingBinder)
			End While
			Me.MakeBinder(node.ElseBlock, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitMultiLineLambdaExpression(ByVal node As MultiLineLambdaExpressionSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			If (node <> Me._enclosingMethod.Syntax) Then
				MyBase.VisitMultiLineLambdaExpression(node)
				Return
			End If
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineFunctionLambdaExpression) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineSubLambdaExpression) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
			End If
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None, syntaxKind)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitOperatorBlock(ByVal node As OperatorBlockSyntax)
			Me.VisitMethodBlockBase(node)
		End Sub

		Public Overrides Sub VisitSelectBlock(ByVal node As SelectBlockSyntax)
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, SyntaxKind.None, SyntaxKind.ExitSelectStatement)
			Me.RememberBinder(node, Me._containingBinder)
			Dim enumerator As SyntaxList(Of CaseBlockSyntax).Enumerator = node.CaseBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Me.MakeBinder(enumerator.Current, Me._containingBinder)
			End While
		End Sub

		Public Overrides Sub VisitSingleLineElseClause(ByVal node As SingleLineElseClauseSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitSingleLineIfStatement(ByVal node As SingleLineIfStatementSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
			Me.MakeBinder(node.ElseClause, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitSingleLineLambdaExpression(ByVal node As SingleLineLambdaExpressionSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			If (node <> Me._enclosingMethod.Syntax) Then
				MyBase.VisitSingleLineLambdaExpression(node)
				Return
			End If
			Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression) Then
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitFunctionStatement
			Else
				If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) Then
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
				End If
				syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExitSubStatement
			End If
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None, syntaxKind)
			Me.RememberBinder(node, Me._containingBinder)
			If (node.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) Then
				Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
			End If
			Me.MakeBinder(node.Body, Me._containingBinder)
		End Sub

		Private Sub VisitStatementsInList(ByVal list As IEnumerable(Of StatementSyntax), ByVal currentBinder As BlockBaseBinder)
			Dim enumerator As IEnumerator(Of StatementSyntax) = Nothing
			Try
				enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					Me.MakeBinder(enumerator.Current, currentBinder)
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub

		Public Overrides Sub VisitSyncLockBlock(ByVal node As SyncLockBlockSyntax)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitTryBlock(ByVal node As TryBlockSyntax)
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, SyntaxKind.None, SyntaxKind.ExitTryStatement)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
			Dim enumerator As SyntaxList(Of CatchBlockSyntax).Enumerator = node.CatchBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Me.MakeBinder(enumerator.Current, Me._containingBinder)
			End While
			Me.MakeBinder(node.FinallyBlock, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitUsingBlock(ByVal node As UsingBlockSyntax)
			Me._containingBinder = New UsingBlockBinder(Me._containingBinder, node)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitWhileBlock(ByVal node As WhileBlockSyntax)
			Me._containingBinder = New ExitableStatementBinder(Me._containingBinder, SyntaxKind.ContinueWhileStatement, SyntaxKind.ExitWhileStatement)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub

		Public Overrides Sub VisitWithBlock(ByVal node As WithBlockSyntax)
			Me._containingBinder = New WithBlockBinder(Me._containingBinder, node)
			Me.RememberBinder(node, Me._containingBinder)
			Me.CreateBinderFromStatementList(node.Statements, Me._containingBinder)
		End Sub
	End Class
End Namespace