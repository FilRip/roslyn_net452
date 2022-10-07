Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class CastExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _keyword As KeywordSyntax

		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _commaToken As PunctuationSyntax

		Friend ReadOnly _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property CommaToken As PunctuationSyntax
			Get
				Return Me._commaToken
			End Get
		End Property

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property Keyword As KeywordSyntax
			Get
				Return Me._keyword
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Friend ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Get
				Return Me._type
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal keyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase.AdjustFlagsAndWidth(keyword)
			Me._keyword = keyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._keyword = keywordSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._commaToken = punctuationSyntax1
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (typeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeSyntax)
				Me._type = typeSyntax
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._closeParenToken = punctuationSyntax2
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._keyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._commaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._type, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace