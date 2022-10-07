Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class FinallyBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax

		Friend ReadOnly _statements As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property FinallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax
			Get
				Return Me._finallyStatement
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax, ByVal statements As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(finallyStatement)
			Me._finallyStatement = finallyStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax, ByVal statements As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(finallyStatement)
			Me._finallyStatement = finallyStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal finallyStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax, ByVal statements As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(finallyStatement)
			Me._finallyStatement = finallyStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)
			If (finallyStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(finallyStatementSyntax)
				Me._finallyStatement = finallyStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitFinallyBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._finallyStatement
			ElseIf (num = 1) Then
				greenNode = Me._statements
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._finallyStatement, Me._statements)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._finallyStatement, Me._statements)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._finallyStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
		End Sub
	End Class
End Namespace