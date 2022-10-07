Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SelectBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax

		Friend ReadOnly _caseBlocks As GreenNode

		Friend ReadOnly _endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CaseBlocks As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax)(Me._caseBlocks)
			End Get
		End Property

		Friend ReadOnly Property EndSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endSelectStatement
			End Get
		End Property

		Friend ReadOnly Property SelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax
			Get
				Return Me._selectStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, ByVal caseBlocks As GreenNode, ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(selectStatement)
			Me._selectStatement = selectStatement
			If (caseBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(caseBlocks)
				Me._caseBlocks = caseBlocks
			End If
			MyBase.AdjustFlagsAndWidth(endSelectStatement)
			Me._endSelectStatement = endSelectStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, ByVal caseBlocks As GreenNode, ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(selectStatement)
			Me._selectStatement = selectStatement
			If (caseBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(caseBlocks)
				Me._caseBlocks = caseBlocks
			End If
			MyBase.AdjustFlagsAndWidth(endSelectStatement)
			Me._endSelectStatement = endSelectStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal selectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax, ByVal caseBlocks As GreenNode, ByVal endSelectStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(selectStatement)
			Me._selectStatement = selectStatement
			If (caseBlocks IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(caseBlocks)
				Me._caseBlocks = caseBlocks
			End If
			MyBase.AdjustFlagsAndWidth(endSelectStatement)
			Me._endSelectStatement = endSelectStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim selectStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)
			If (selectStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(selectStatementSyntax)
				Me._selectStatement = selectStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._caseBlocks = greenNode
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endSelectStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSelectBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._selectStatement
					Exit Select
				Case 1
					greenNode = Me._caseBlocks
					Exit Select
				Case 2
					greenNode = Me._endSelectStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._selectStatement, Me._caseBlocks, Me._endSelectStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._selectStatement, Me._caseBlocks, Me._endSelectStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._selectStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._caseBlocks, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endSelectStatement, IObjectWritable))
		End Sub
	End Class
End Namespace