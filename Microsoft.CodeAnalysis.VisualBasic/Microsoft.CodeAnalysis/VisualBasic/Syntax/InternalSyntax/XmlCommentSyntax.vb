Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlCommentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _lessThanExclamationMinusMinusToken As PunctuationSyntax

		Friend ReadOnly _textTokens As GreenNode

		Friend ReadOnly _minusMinusGreaterThanToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property LessThanExclamationMinusMinusToken As PunctuationSyntax
			Get
				Return Me._lessThanExclamationMinusMinusToken
			End Get
		End Property

		Friend ReadOnly Property MinusMinusGreaterThanToken As PunctuationSyntax
			Get
				Return Me._minusMinusGreaterThanToken
			End Get
		End Property

		Friend ReadOnly Property TextTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._textTokens)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal minusMinusGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken)
			Me._lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(minusMinusGreaterThanToken)
			Me._minusMinusGreaterThanToken = minusMinusGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal minusMinusGreaterThanToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken)
			Me._lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(minusMinusGreaterThanToken)
			Me._minusMinusGreaterThanToken = minusMinusGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanExclamationMinusMinusToken As PunctuationSyntax, ByVal textTokens As GreenNode, ByVal minusMinusGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(lessThanExclamationMinusMinusToken)
			Me._lessThanExclamationMinusMinusToken = lessThanExclamationMinusMinusToken
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(minusMinusGreaterThanToken)
			Me._minusMinusGreaterThanToken = minusMinusGreaterThanToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._lessThanExclamationMinusMinusToken = punctuationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._textTokens = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._minusMinusGreaterThanToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlComment(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlCommentSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._lessThanExclamationMinusMinusToken
					Exit Select
				Case 1
					greenNode = Me._textTokens
					Exit Select
				Case 2
					greenNode = Me._minusMinusGreaterThanToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._lessThanExclamationMinusMinusToken, Me._textTokens, Me._minusMinusGreaterThanToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._lessThanExclamationMinusMinusToken, Me._textTokens, Me._minusMinusGreaterThanToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._lessThanExclamationMinusMinusToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._textTokens, IObjectWritable))
			writer.WriteValue(DirectCast(Me._minusMinusGreaterThanToken, IObjectWritable))
		End Sub
	End Class
End Namespace