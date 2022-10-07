Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class FieldInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _keyKeyword As KeywordSyntax

		Friend ReadOnly Property KeyKeyword As KeywordSyntax
			Get
				Return Me._keyKeyword
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyKeyword As KeywordSyntax)
			MyBase.New(kind)
			If (keyKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keyKeyword)
				Me._keyKeyword = keyKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			If (keyKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keyKeyword)
				Me._keyKeyword = keyKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations)
			If (keyKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keyKeyword)
				Me._keyKeyword = keyKeyword
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._keyKeyword = keywordSyntax
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._keyKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace