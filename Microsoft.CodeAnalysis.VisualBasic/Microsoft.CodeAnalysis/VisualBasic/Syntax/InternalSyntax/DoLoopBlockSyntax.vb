Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class DoLoopBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax

		Friend ReadOnly _statements As GreenNode

		Friend ReadOnly _loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property DoStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax
			Get
				Return Me._doStatement
			End Get
		End Property

		Friend ReadOnly Property LoopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax
			Get
				Return Me._loopStatement
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As GreenNode, ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(doStatement)
			Me._doStatement = doStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			MyBase.AdjustFlagsAndWidth(loopStatement)
			Me._loopStatement = loopStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As GreenNode, ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(doStatement)
			Me._doStatement = doStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			MyBase.AdjustFlagsAndWidth(loopStatement)
			Me._loopStatement = loopStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal doStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax, ByVal statements As GreenNode, ByVal loopStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(doStatement)
			Me._doStatement = doStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			MyBase.AdjustFlagsAndWidth(loopStatement)
			Me._loopStatement = loopStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim doStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)
			If (doStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(doStatementSyntax)
				Me._doStatement = doStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
			Dim loopStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)
			If (loopStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(loopStatementSyntax)
				Me._loopStatement = loopStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitDoLoopBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.DoLoopBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._doStatement
					Exit Select
				Case 1
					greenNode = Me._statements
					Exit Select
				Case 2
					greenNode = Me._loopStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._doStatement, Me._statements, Me._loopStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._doStatement, Me._statements, Me._loopStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._doStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
			writer.WriteValue(DirectCast(Me._loopStatement, IObjectWritable))
		End Sub
	End Class
End Namespace