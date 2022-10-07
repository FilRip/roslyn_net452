Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class ForOrForEachStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
		Friend ReadOnly _forKeyword As KeywordSyntax

		Friend ReadOnly _controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode

		Friend ReadOnly Property ControlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Get
				Return Me._controlVariable
			End Get
		End Property

		Friend ReadOnly Property ForKeyword As KeywordSyntax
			Get
				Return Me._forKeyword
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(forKeyword)
			Me._forKeyword = forKeyword
			MyBase.AdjustFlagsAndWidth(controlVariable)
			Me._controlVariable = controlVariable
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(forKeyword)
			Me._forKeyword = forKeyword
			MyBase.AdjustFlagsAndWidth(controlVariable)
			Me._controlVariable = controlVariable
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(forKeyword)
			Me._forKeyword = forKeyword
			MyBase.AdjustFlagsAndWidth(controlVariable)
			Me._controlVariable = controlVariable
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._forKeyword = keywordSyntax
			End If
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)
			If (visualBasicSyntaxNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(visualBasicSyntaxNode)
				Me._controlVariable = visualBasicSyntaxNode
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._forKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._controlVariable, IObjectWritable))
		End Sub
	End Class
End Namespace