Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class ForOrForEachBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _statements As GreenNode

		Friend ReadOnly _nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax

		Friend ReadOnly Property NextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax
			Get
				Return Me._nextStatement
			End Get
		End Property

		Friend ReadOnly Property Statements As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._statements)
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			MyBase.New(kind)
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (nextStatement IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nextStatement)
				Me._nextStatement = nextStatement
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (nextStatement IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nextStatement)
				Me._nextStatement = nextStatement
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			MyBase.New(kind, errors, annotations)
			If (statements IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(statements)
				Me._statements = statements
			End If
			If (nextStatement IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nextStatement)
				Me._nextStatement = nextStatement
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._statements = greenNode
			End If
			Dim nextStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			If (nextStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(nextStatementSyntax)
				Me._nextStatement = nextStatementSyntax
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._statements, IObjectWritable))
			writer.WriteValue(DirectCast(Me._nextStatement, IObjectWritable))
		End Sub
	End Class
End Namespace