Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlProcessingInstructionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _lessThanQuestionToken As PunctuationSyntax

		Friend ReadOnly _name As XmlNameTokenSyntax

		Friend ReadOnly _textTokens As GreenNode

		Friend ReadOnly _questionGreaterThanToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property LessThanQuestionToken As PunctuationSyntax
			Get
				Return Me._lessThanQuestionToken
			End Get
		End Property

		Friend ReadOnly Property Name As XmlNameTokenSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property QuestionGreaterThanToken As PunctuationSyntax
			Get
				Return Me._questionGreaterThanToken
			End Get
		End Property

		Friend ReadOnly Property TextTokens As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of XmlTextTokenSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._textTokens)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As GreenNode, ByVal questionGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As GreenNode, ByVal questionGreaterThanToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanQuestionToken As PunctuationSyntax, ByVal name As XmlNameTokenSyntax, ByVal textTokens As GreenNode, ByVal questionGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (textTokens IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(textTokens)
				Me._textTokens = textTokens
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._lessThanQuestionToken = punctuationSyntax
			End If
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (xmlNameTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNameTokenSyntax)
				Me._name = xmlNameTokenSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._textTokens = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._questionGreaterThanToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlProcessingInstruction(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlProcessingInstructionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._lessThanQuestionToken
					Exit Select
				Case 1
					greenNode = Me._name
					Exit Select
				Case 2
					greenNode = Me._textTokens
					Exit Select
				Case 3
					greenNode = Me._questionGreaterThanToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._lessThanQuestionToken, Me._name, Me._textTokens, Me._questionGreaterThanToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._lessThanQuestionToken, Me._name, Me._textTokens, Me._questionGreaterThanToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._lessThanQuestionToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._textTokens, IObjectWritable))
			writer.WriteValue(DirectCast(Me._questionGreaterThanToken, IObjectWritable))
		End Sub
	End Class
End Namespace