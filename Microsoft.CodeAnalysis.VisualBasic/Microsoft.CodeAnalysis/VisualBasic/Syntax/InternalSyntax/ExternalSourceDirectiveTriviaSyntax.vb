Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ExternalSourceDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _externalSourceKeyword As KeywordSyntax

		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _externalSource As StringLiteralTokenSyntax

		Friend ReadOnly _commaToken As PunctuationSyntax

		Friend ReadOnly _lineStart As IntegerLiteralTokenSyntax

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

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

		Friend ReadOnly Property ExternalSource As StringLiteralTokenSyntax
			Get
				Return Me._externalSource
			End Get
		End Property

		Friend ReadOnly Property ExternalSourceKeyword As KeywordSyntax
			Get
				Return Me._externalSourceKeyword
			End Get
		End Property

		Friend ReadOnly Property LineStart As IntegerLiteralTokenSyntax
			Get
				Return Me._lineStart
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 7
			MyBase.AdjustFlagsAndWidth(externalSourceKeyword)
			Me._externalSourceKeyword = externalSourceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(lineStart)
			Me._lineStart = lineStart
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 7
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(externalSourceKeyword)
			Me._externalSourceKeyword = externalSourceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(lineStart)
			Me._lineStart = lineStart
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal externalSourceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal commaToken As PunctuationSyntax, ByVal lineStart As IntegerLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 7
			MyBase.AdjustFlagsAndWidth(externalSourceKeyword)
			Me._externalSourceKeyword = externalSourceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(lineStart)
			Me._lineStart = lineStart
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 7
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._externalSourceKeyword = keywordSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim stringLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (stringLiteralTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stringLiteralTokenSyntax)
				Me._externalSource = stringLiteralTokenSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._commaToken = punctuationSyntax1
			End If
			Dim integerLiteralTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IntegerLiteralTokenSyntax)
			If (integerLiteralTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(integerLiteralTokenSyntax)
				Me._lineStart = integerLiteralTokenSyntax
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._closeParenToken = punctuationSyntax2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitExternalSourceDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._hashToken
					Exit Select
				Case 1
					greenNode = Me._externalSourceKeyword
					Exit Select
				Case 2
					greenNode = Me._openParenToken
					Exit Select
				Case 3
					greenNode = Me._externalSource
					Exit Select
				Case 4
					greenNode = Me._commaToken
					Exit Select
				Case 5
					greenNode = Me._lineStart
					Exit Select
				Case 6
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._externalSourceKeyword, Me._openParenToken, Me._externalSource, Me._commaToken, Me._lineStart, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._externalSourceKeyword, Me._openParenToken, Me._externalSource, Me._commaToken, Me._lineStart, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._externalSourceKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._externalSource, IObjectWritable))
			writer.WriteValue(DirectCast(Me._commaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._lineStart, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace