Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTryStatement
		Inherits BoundStatement
		Private ReadOnly _TryBlock As BoundBlock

		Private ReadOnly _CatchBlocks As ImmutableArray(Of BoundCatchBlock)

		Private ReadOnly _FinallyBlockOpt As BoundBlock

		Private ReadOnly _ExitLabelOpt As LabelSymbol

		Public ReadOnly Property CatchBlocks As ImmutableArray(Of BoundCatchBlock)
			Get
				Return Me._CatchBlocks
			End Get
		End Property

		Public ReadOnly Property ExitLabelOpt As LabelSymbol
			Get
				Return Me._ExitLabelOpt
			End Get
		End Property

		Public ReadOnly Property FinallyBlockOpt As BoundBlock
			Get
				Return Me._FinallyBlockOpt
			End Get
		End Property

		Public ReadOnly Property TryBlock As BoundBlock
			Get
				Return Me._TryBlock
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal tryBlock As BoundBlock, ByVal catchBlocks As ImmutableArray(Of BoundCatchBlock), ByVal finallyBlockOpt As BoundBlock, ByVal exitLabelOpt As LabelSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.TryStatement, syntax, If(hasErrors OrElse tryBlock.NonNullAndHasErrors() OrElse catchBlocks.NonNullAndHasErrors(), True, finallyBlockOpt.NonNullAndHasErrors()))
			Me._TryBlock = tryBlock
			Me._CatchBlocks = catchBlocks
			Me._FinallyBlockOpt = finallyBlockOpt
			Me._ExitLabelOpt = exitLabelOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTryStatement(Me)
		End Function

		Public Function Update(ByVal tryBlock As BoundBlock, ByVal catchBlocks As ImmutableArray(Of BoundCatchBlock), ByVal finallyBlockOpt As BoundBlock, ByVal exitLabelOpt As LabelSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement
			Dim boundTryStatement As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement
			If (tryBlock <> Me.TryBlock OrElse catchBlocks <> Me.CatchBlocks OrElse finallyBlockOpt <> Me.FinallyBlockOpt OrElse CObj(exitLabelOpt) <> CObj(Me.ExitLabelOpt)) Then
				Dim boundTryStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement(MyBase.Syntax, tryBlock, catchBlocks, finallyBlockOpt, exitLabelOpt, MyBase.HasErrors)
				boundTryStatement1.CopyAttributes(Me)
				boundTryStatement = boundTryStatement1
			Else
				boundTryStatement = Me
			End If
			Return boundTryStatement
		End Function
	End Class
End Namespace