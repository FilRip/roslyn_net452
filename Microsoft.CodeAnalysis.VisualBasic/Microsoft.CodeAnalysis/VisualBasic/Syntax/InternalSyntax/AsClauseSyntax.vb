Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class AsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _asKeyword As KeywordSyntax

		Friend ReadOnly Property AsKeyword As KeywordSyntax
			Get
				Return Me._asKeyword
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._asKeyword = keywordSyntax
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._asKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace