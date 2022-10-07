Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class InstanceExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _keyword As KeywordSyntax

		Friend ReadOnly Property Keyword As KeywordSyntax
			Get
				Return Me._keyword
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyword As KeywordSyntax)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._keyword = keywordSyntax
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._keyword, IObjectWritable))
		End Sub
	End Class
End Namespace