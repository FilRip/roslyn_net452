Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ExternalChecksumDirectiveTriviaSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax
		Friend ReadOnly _externalChecksumKeyword As KeywordSyntax

		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _externalSource As StringLiteralTokenSyntax

		Friend ReadOnly _firstCommaToken As PunctuationSyntax

		Friend ReadOnly _guid As StringLiteralTokenSyntax

		Friend ReadOnly _secondCommaToken As PunctuationSyntax

		Friend ReadOnly _checksum As StringLiteralTokenSyntax

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Checksum As StringLiteralTokenSyntax
			Get
				Return Me._checksum
			End Get
		End Property

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property ExternalChecksumKeyword As KeywordSyntax
			Get
				Return Me._externalChecksumKeyword
			End Get
		End Property

		Friend ReadOnly Property ExternalSource As StringLiteralTokenSyntax
			Get
				Return Me._externalSource
			End Get
		End Property

		Friend ReadOnly Property FirstCommaToken As PunctuationSyntax
			Get
				Return Me._firstCommaToken
			End Get
		End Property

		Friend ReadOnly Property Guid As StringLiteralTokenSyntax
			Get
				Return Me._guid
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Friend ReadOnly Property SecondCommaToken As PunctuationSyntax
			Get
				Return Me._secondCommaToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 9
			MyBase.AdjustFlagsAndWidth(externalChecksumKeyword)
			Me._externalChecksumKeyword = externalChecksumKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(guid)
			Me._guid = guid
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(checksum)
			Me._checksum = checksum
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, hashToken)
			MyBase._slotCount = 9
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(externalChecksumKeyword)
			Me._externalChecksumKeyword = externalChecksumKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(guid)
			Me._guid = guid
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(checksum)
			Me._checksum = checksum
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal hashToken As PunctuationSyntax, ByVal externalChecksumKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal externalSource As StringLiteralTokenSyntax, ByVal firstCommaToken As PunctuationSyntax, ByVal guid As StringLiteralTokenSyntax, ByVal secondCommaToken As PunctuationSyntax, ByVal checksum As StringLiteralTokenSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations, hashToken)
			MyBase._slotCount = 9
			MyBase.AdjustFlagsAndWidth(externalChecksumKeyword)
			Me._externalChecksumKeyword = externalChecksumKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			MyBase.AdjustFlagsAndWidth(externalSource)
			Me._externalSource = externalSource
			MyBase.AdjustFlagsAndWidth(firstCommaToken)
			Me._firstCommaToken = firstCommaToken
			MyBase.AdjustFlagsAndWidth(guid)
			Me._guid = guid
			MyBase.AdjustFlagsAndWidth(secondCommaToken)
			Me._secondCommaToken = secondCommaToken
			MyBase.AdjustFlagsAndWidth(checksum)
			Me._checksum = checksum
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 9
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._externalChecksumKeyword = keywordSyntax
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
				Me._firstCommaToken = punctuationSyntax1
			End If
			Dim stringLiteralTokenSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (stringLiteralTokenSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stringLiteralTokenSyntax1)
				Me._guid = stringLiteralTokenSyntax1
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._secondCommaToken = punctuationSyntax2
			End If
			Dim stringLiteralTokenSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StringLiteralTokenSyntax)
			If (stringLiteralTokenSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(stringLiteralTokenSyntax2)
				Me._checksum = stringLiteralTokenSyntax2
			End If
			Dim punctuationSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax3)
				Me._closeParenToken = punctuationSyntax3
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitExternalChecksumDirectiveTrivia(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalChecksumDirectiveTriviaSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._hashToken
					Exit Select
				Case 1
					greenNode = Me._externalChecksumKeyword
					Exit Select
				Case 2
					greenNode = Me._openParenToken
					Exit Select
				Case 3
					greenNode = Me._externalSource
					Exit Select
				Case 4
					greenNode = Me._firstCommaToken
					Exit Select
				Case 5
					greenNode = Me._guid
					Exit Select
				Case 6
					greenNode = Me._secondCommaToken
					Exit Select
				Case 7
					greenNode = Me._checksum
					Exit Select
				Case 8
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._hashToken, Me._externalChecksumKeyword, Me._openParenToken, Me._externalSource, Me._firstCommaToken, Me._guid, Me._secondCommaToken, Me._checksum, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._hashToken, Me._externalChecksumKeyword, Me._openParenToken, Me._externalSource, Me._firstCommaToken, Me._guid, Me._secondCommaToken, Me._checksum, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._externalChecksumKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._externalSource, IObjectWritable))
			writer.WriteValue(DirectCast(Me._firstCommaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._guid, IObjectWritable))
			writer.WriteValue(DirectCast(Me._secondCommaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._checksum, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace