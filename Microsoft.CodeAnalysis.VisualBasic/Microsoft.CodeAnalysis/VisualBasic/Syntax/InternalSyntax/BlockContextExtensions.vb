Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend Module BlockContextExtensions
		<Extension>
		Friend Function EndLambda(ByVal context As BlockContext) As BlockContext
			Dim isLambda As Boolean = False
			Do
				isLambda = context.IsLambda
				context = context.EndBlock(Nothing)
			Loop While Not isLambda
			Return context
		End Function

		<Extension>
		Friend Function FindNearest(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal conditionIsTrue As Func(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, Boolean)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				If (context Is Nothing) Then
					blockContext = Nothing
					Exit While
				ElseIf (Not conditionIsTrue(context)) Then
					context = context.PrevBlock
				Else
					blockContext = context
					Exit While
				End If
			End While
			Return blockContext
		End Function

		<Extension>
		Friend Function FindNearest(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal conditionIsTrue As Func(Of SyntaxKind, Boolean)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				If (context Is Nothing) Then
					blockContext = Nothing
					Exit While
				ElseIf (Not conditionIsTrue(context.BlockKind)) Then
					context = context.PrevBlock
				Else
					blockContext = context
					Exit While
				End If
			End While
			Return blockContext
		End Function

		<Extension>
		Friend Function FindNearest(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal ParamArray kinds As SyntaxKind()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				If (context Is Nothing) Then
					blockContext = Nothing
					Exit While
				ElseIf (Not kinds.Contains(context.BlockKind)) Then
					context = context.PrevBlock
				Else
					blockContext = context
					Exit While
				End If
			End While
			Return blockContext
		End Function

		<Extension>
		Friend Function FindNearestInSameMethodScope(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal ParamArray kinds As SyntaxKind()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				If (context Is Nothing) Then
					blockContext = Nothing
					Exit While
				ElseIf (kinds.Contains(context.BlockKind)) Then
					blockContext = context
					Exit While
				ElseIf (Not context.IsLambda) Then
					context = context.PrevBlock
				Else
					blockContext = Nothing
					Exit While
				End If
			End While
			Return blockContext
		End Function

		<Extension>
		Friend Function FindNearestLambdaOrSingleLineIf(ByVal context As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext, ByVal lastContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			Dim blockContext As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BlockContext
			While True
				If (context = lastContext) Then
					blockContext = Nothing
					Exit While
				ElseIf (context.IsLambda OrElse context.IsLineIf) Then
					blockContext = context
					Exit While
				Else
					context = context.PrevBlock
				End If
			End While
			Return blockContext
		End Function

		<Extension>
		Friend Function IsWithin(ByVal context As BlockContext, ByVal ParamArray kinds As SyntaxKind()) As Boolean
			Return context.FindNearest(kinds) IsNot Nothing
		End Function

		<Extension>
		Friend Sub RecoverFromMissingEnd(ByVal context As BlockContext, ByVal lastContext As BlockContext)
			While context.Level > lastContext.Level
				context = context.EndBlock(Nothing)
			End While
		End Sub
	End Module
End Namespace