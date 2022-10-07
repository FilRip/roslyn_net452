Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class MultiLineIfBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax

		Friend ReadOnly _statements As GreenNode

		Friend ReadOnly _elseIfBlocks As GreenNode

		Friend ReadOnly _elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax

		Friend ReadOnly _endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ElseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax
			Get
				Return Me._elseBlock
			End Get
		End Property

		Friend ReadOnly Property ElseIfBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax)(Me._elseIfBlocks)
			End Get
		End Property

		Friend ReadOnly Property EndIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endIfStatement
			End Get
		End Property

		Friend ReadOnly Property IfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax
			Get
				Return Me._ifStatement
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax, ByVal statements As GreenNode, ByVal elseIfBlocks As GreenNode, ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(ifStatement)
			Me._ifStatement = ifStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (elseIfBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseIfBlocks)
				Me._elseIfBlocks = elseIfBlocks
			End If
			If (elseBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseBlock)
				Me._elseBlock = elseBlock
			End If
			MyBase.AdjustFlagsAndWidth(endIfStatement)
			Me._endIfStatement = endIfStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax, ByVal statements As GreenNode, ByVal elseIfBlocks As GreenNode, ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(ifStatement)
			Me._ifStatement = ifStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (elseIfBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseIfBlocks)
				Me._elseIfBlocks = elseIfBlocks
			End If
			If (elseBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseBlock)
				Me._elseBlock = elseBlock
			End If
			MyBase.AdjustFlagsAndWidth(endIfStatement)
			Me._endIfStatement = endIfStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax, ByVal statements As GreenNode, ByVal elseIfBlocks As GreenNode, ByVal elseBlock As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax, ByVal endIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(ifStatement)
			Me._ifStatement = ifStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (elseIfBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseIfBlocks)
				Me._elseIfBlocks = elseIfBlocks
			End If
			If (elseBlock IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseBlock)
				Me._elseBlock = elseBlock
			End If
			MyBase.AdjustFlagsAndWidth(endIfStatement)
			Me._endIfStatement = endIfStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)
			If (ifStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(ifStatementSyntax)
				Me._ifStatement = ifStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._elseIfBlocks = greenNode1
			End If
			Dim elseBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)
			If (elseBlockSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseBlockSyntax)
				Me._elseBlock = elseBlockSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endIfStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitMultiLineIfBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._ifStatement
					Exit Select
				Case 1
					greenNode = Me._statements
					Exit Select
				Case 2
					greenNode = Me._elseIfBlocks
					Exit Select
				Case 3
					greenNode = Me._elseBlock
					Exit Select
				Case 4
					greenNode = Me._endIfStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._ifStatement, Me._statements, Me._elseIfBlocks, Me._elseBlock, Me._endIfStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._ifStatement, Me._statements, Me._elseIfBlocks, Me._elseBlock, Me._endIfStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._ifStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
			writer.WriteValue(DirectCast(Me._elseIfBlocks, IObjectWritable))
			writer.WriteValue(DirectCast(Me._elseBlock, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endIfStatement, IObjectWritable))
		End Sub
	End Class
End Namespace