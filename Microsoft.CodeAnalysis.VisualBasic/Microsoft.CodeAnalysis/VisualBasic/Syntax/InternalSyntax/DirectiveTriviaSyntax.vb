Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class DirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructuredTriviaSyntax
		Friend ReadOnly _hashToken As PunctuationSyntax

		Friend ReadOnly Property HashToken As PunctuationSyntax
			Get
				Return Me._hashToken
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(hashToken)
			Me._hashToken = hashToken
			MyBase.SetFlags(GreenNode.NodeFlags.ContainsDirectives)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(hashToken)
			Me._hashToken = hashToken
			MyBase.SetFlags(GreenNode.NodeFlags.ContainsDirectives)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(hashToken)
			Me._hashToken = hashToken
			MyBase.SetFlags(GreenNode.NodeFlags.ContainsDirectives)
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._hashToken = punctuationSyntax
			End If
			MyBase.SetFlags(GreenNode.NodeFlags.ContainsDirectives)
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._hashToken, IObjectWritable))
		End Sub
	End Class
End Namespace