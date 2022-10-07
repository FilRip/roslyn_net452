Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ElseIfBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax

		Friend ReadOnly _statements As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ElseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax
			Get
				Return Me._elseIfStatement
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax, ByVal statements As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(elseIfStatement)
			Me._elseIfStatement = elseIfStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax, ByVal statements As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(elseIfStatement)
			Me._elseIfStatement = elseIfStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elseIfStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax, ByVal statements As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(elseIfStatement)
			Me._elseIfStatement = elseIfStatement
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim elseIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)
			If (elseIfStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(elseIfStatementSyntax)
				Me._elseIfStatement = elseIfStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitElseIfBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ElseIfBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._elseIfStatement
			ElseIf (num = 1) Then
				greenNode = Me._statements
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._elseIfStatement, Me._statements)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._elseIfStatement, Me._statements)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._elseIfStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
		End Sub
	End Class
End Namespace