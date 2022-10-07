Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class SingleLineIfOrElseBlockContext
		Inherits ExecutableStatementContext
		Friend Overrides ReadOnly Property IsSingleLine As Boolean
			Get
				Return True
			End Get
		End Property

		Protected ReadOnly Property TreatOtherAsStatementTerminator As Boolean
			Get
				Dim blockKind As SyntaxKind
				Dim prevBlock As BlockContext = MyBase.PrevBlock
				While True
					blockKind = prevBlock.BlockKind
					If (blockKind <> SyntaxKind.SingleLineIfStatement AndAlso blockKind <> SyntaxKind.SingleLineElseClause) Then
						Exit While
					End If
					prevBlock = prevBlock.PrevBlock
				End While
				Return If(blockKind = SyntaxKind.SingleLineSubLambdaExpression, True, False)
			End Get
		End Property

		Protected Sub New(ByVal kind As SyntaxKind, ByVal statement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax, ByVal prevContext As BlockContext)
			MyBase.New(kind, statement, prevContext)
		End Sub

		Protected Function ProcessOtherAsStatementTerminator() As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockKind As SyntaxKind
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext = Me.EndBlock(Nothing)
			While True
				blockKind = blockContext.BlockKind
				If (blockKind <> SyntaxKind.SingleLineIfStatement AndAlso blockKind <> SyntaxKind.SingleLineElseClause) Then
					Exit While
				End If
				blockContext = blockContext.EndBlock(Nothing)
			End While
			If (blockKind <> SyntaxKind.SingleLineSubLambdaExpression) Then
				Throw ExceptionUtilities.UnexpectedValue(blockContext.BlockKind)
			End If
			Return blockContext.PrevBlock
		End Function
	End Class
End Namespace