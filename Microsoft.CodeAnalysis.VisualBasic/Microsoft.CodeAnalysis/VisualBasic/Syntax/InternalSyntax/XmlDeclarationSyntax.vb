Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlDeclarationSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _lessThanQuestionToken As PunctuationSyntax

		Friend ReadOnly _xmlKeyword As KeywordSyntax

		Friend ReadOnly _version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax

		Friend ReadOnly _encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax

		Friend ReadOnly _standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax

		Friend ReadOnly _questionGreaterThanToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Get
				Return Me._encoding
			End Get
		End Property

		Friend ReadOnly Property LessThanQuestionToken As PunctuationSyntax
			Get
				Return Me._lessThanQuestionToken
			End Get
		End Property

		Friend ReadOnly Property QuestionGreaterThanToken As PunctuationSyntax
			Get
				Return Me._questionGreaterThanToken
			End Get
		End Property

		Friend ReadOnly Property Standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Get
				Return Me._standalone
			End Get
		End Property

		Friend ReadOnly Property Version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax
			Get
				Return Me._version
			End Get
		End Property

		Friend ReadOnly Property XmlKeyword As KeywordSyntax
			Get
				Return Me._xmlKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanQuestionToken As PunctuationSyntax, ByVal xmlKeyword As KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(xmlKeyword)
			Me._xmlKeyword = xmlKeyword
			MyBase.AdjustFlagsAndWidth(version)
			Me._version = version
			If (encoding IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(encoding)
				Me._encoding = encoding
			End If
			If (standalone IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(standalone)
				Me._standalone = standalone
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanQuestionToken As PunctuationSyntax, ByVal xmlKeyword As KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 6
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(xmlKeyword)
			Me._xmlKeyword = xmlKeyword
			MyBase.AdjustFlagsAndWidth(version)
			Me._version = version
			If (encoding IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(encoding)
				Me._encoding = encoding
			End If
			If (standalone IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(standalone)
				Me._standalone = standalone
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanQuestionToken As PunctuationSyntax, ByVal xmlKeyword As KeywordSyntax, ByVal version As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal encoding As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal standalone As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax, ByVal questionGreaterThanToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 6
			MyBase.AdjustFlagsAndWidth(lessThanQuestionToken)
			Me._lessThanQuestionToken = lessThanQuestionToken
			MyBase.AdjustFlagsAndWidth(xmlKeyword)
			Me._xmlKeyword = xmlKeyword
			MyBase.AdjustFlagsAndWidth(version)
			Me._version = version
			If (encoding IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(encoding)
				Me._encoding = encoding
			End If
			If (standalone IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(standalone)
				Me._standalone = standalone
			End If
			MyBase.AdjustFlagsAndWidth(questionGreaterThanToken)
			Me._questionGreaterThanToken = questionGreaterThanToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 6
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._lessThanQuestionToken = punctuationSyntax
			End If
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._xmlKeyword = keywordSyntax
			End If
			Dim xmlDeclarationOptionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (xmlDeclarationOptionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlDeclarationOptionSyntax)
				Me._version = xmlDeclarationOptionSyntax
			End If
			Dim xmlDeclarationOptionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (xmlDeclarationOptionSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlDeclarationOptionSyntax1)
				Me._encoding = xmlDeclarationOptionSyntax1
			End If
			Dim xmlDeclarationOptionSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)
			If (xmlDeclarationOptionSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlDeclarationOptionSyntax2)
				Me._standalone = xmlDeclarationOptionSyntax2
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._questionGreaterThanToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlDeclaration(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._lessThanQuestionToken
					Exit Select
				Case 1
					greenNode = Me._xmlKeyword
					Exit Select
				Case 2
					greenNode = Me._version
					Exit Select
				Case 3
					greenNode = Me._encoding
					Exit Select
				Case 4
					greenNode = Me._standalone
					Exit Select
				Case 5
					greenNode = Me._questionGreaterThanToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._lessThanQuestionToken, Me._xmlKeyword, Me._version, Me._encoding, Me._standalone, Me._questionGreaterThanToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._lessThanQuestionToken, Me._xmlKeyword, Me._version, Me._encoding, Me._standalone, Me._questionGreaterThanToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._lessThanQuestionToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._xmlKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._version, IObjectWritable))
			writer.WriteValue(DirectCast(Me._encoding, IObjectWritable))
			writer.WriteValue(DirectCast(Me._standalone, IObjectWritable))
			writer.WriteValue(DirectCast(Me._questionGreaterThanToken, IObjectWritable))
		End Sub
	End Class
End Namespace