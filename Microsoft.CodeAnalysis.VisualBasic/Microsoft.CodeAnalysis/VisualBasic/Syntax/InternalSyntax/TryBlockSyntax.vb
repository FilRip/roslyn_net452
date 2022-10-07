Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TryBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax

		Friend ReadOnly _statements As GreenNode

		Friend ReadOnly _catchBlocks As GreenNode

		Friend ReadOnly _finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax

		Friend ReadOnly _endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CatchBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax)(Me._catchBlocks)
			End Get
		End Property

		Friend ReadOnly Property EndTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endTryStatement
			End Get
		End Property

		Friend ReadOnly Property FinallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax
			Get
				Return Me._finallyBlock
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Friend ReadOnly Property TryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax
			Get
				Return Me._tryStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax, ByVal statements As GreenNode, ByVal catchBlocks As GreenNode, ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(tryStatement)
			Me._tryStatement = tryStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (catchBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(catchBlocks)
				Me._catchBlocks = catchBlocks
			End If
			If (finallyBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(finallyBlock)
				Me._finallyBlock = finallyBlock
			End If
			MyBase.AdjustFlagsAndWidth(endTryStatement)
			Me._endTryStatement = endTryStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax, ByVal statements As GreenNode, ByVal catchBlocks As GreenNode, ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(tryStatement)
			Me._tryStatement = tryStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (catchBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(catchBlocks)
				Me._catchBlocks = catchBlocks
			End If
			If (finallyBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(finallyBlock)
				Me._finallyBlock = finallyBlock
			End If
			MyBase.AdjustFlagsAndWidth(endTryStatement)
			Me._endTryStatement = endTryStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal tryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax, ByVal statements As GreenNode, ByVal catchBlocks As GreenNode, ByVal finallyBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax, ByVal endTryStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(tryStatement)
			Me._tryStatement = tryStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (catchBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(catchBlocks)
				Me._catchBlocks = catchBlocks
			End If
			If (finallyBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(finallyBlock)
				Me._finallyBlock = finallyBlock
			End If
			MyBase.AdjustFlagsAndWidth(endTryStatement)
			Me._endTryStatement = endTryStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)
			If (tryStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(tryStatementSyntax)
				Me._tryStatement = tryStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._catchBlocks = greenNode1
			End If
			Dim finallyBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)
			If (finallyBlockSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(finallyBlockSyntax)
				Me._finallyBlock = finallyBlockSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endTryStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTryBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TryBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._tryStatement
					Exit Select
				Case 1
					greenNode = Me._statements
					Exit Select
				Case 2
					greenNode = Me._catchBlocks
					Exit Select
				Case 3
					greenNode = Me._finallyBlock
					Exit Select
				Case 4
					greenNode = Me._endTryStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._tryStatement, Me._statements, Me._catchBlocks, Me._finallyBlock, Me._endTryStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._tryStatement, Me._statements, Me._catchBlocks, Me._finallyBlock, Me._endTryStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._tryStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
			writer.WriteValue(DirectCast(Me._catchBlocks, IObjectWritable))
			writer.WriteValue(DirectCast(Me._finallyBlock, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endTryStatement, IObjectWritable))
		End Sub
	End Class
End Namespace