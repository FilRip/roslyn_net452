Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlNameAttributeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BaseXmlAttributeSyntax
		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax

		Friend ReadOnly _equalsToken As PunctuationSyntax

		Friend ReadOnly _startQuoteToken As PunctuationSyntax

		Friend ReadOnly _reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax

		Friend ReadOnly _endQuoteToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property EndQuoteToken As PunctuationSyntax
			Get
				Return Me._endQuoteToken
			End Get
		End Property

		Friend ReadOnly Property EqualsToken As PunctuationSyntax
			Get
				Return Me._equalsToken
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property Reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Get
				Return Me._reference
			End Get
		End Property

		Friend ReadOnly Property StartQuoteToken As PunctuationSyntax
			Get
				Return Me._startQuoteToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(startQuoteToken)
			Me._startQuoteToken = startQuoteToken
			MyBase.AdjustFlagsAndWidth(reference)
			Me._reference = reference
			MyBase.AdjustFlagsAndWidth(endQuoteToken)
			Me._endQuoteToken = endQuoteToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(startQuoteToken)
			Me._startQuoteToken = startQuoteToken
			MyBase.AdjustFlagsAndWidth(reference)
			Me._reference = reference
			MyBase.AdjustFlagsAndWidth(endQuoteToken)
			Me._endQuoteToken = endQuoteToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equalsToken)
			Me._equalsToken = equalsToken
			MyBase.AdjustFlagsAndWidth(startQuoteToken)
			Me._startQuoteToken = startQuoteToken
			MyBase.AdjustFlagsAndWidth(reference)
			Me._reference = reference
			MyBase.AdjustFlagsAndWidth(endQuoteToken)
			Me._endQuoteToken = endQuoteToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)
			If (xmlNameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNameSyntax)
				Me._name = xmlNameSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._equalsToken = punctuationSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._startQuoteToken = punctuationSyntax1
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (identifierNameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierNameSyntax)
				Me._reference = identifierNameSyntax
			End If
			Dim punctuationSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax2)
				Me._endQuoteToken = punctuationSyntax2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlNameAttribute(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._name
					Exit Select
				Case 1
					greenNode = Me._equalsToken
					Exit Select
				Case 2
					greenNode = Me._startQuoteToken
					Exit Select
				Case 3
					greenNode = Me._reference
					Exit Select
				Case 4
					greenNode = Me._endQuoteToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._name, Me._equalsToken, Me._startQuoteToken, Me._reference, Me._endQuoteToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._name, Me._equalsToken, Me._startQuoteToken, Me._reference, Me._endQuoteToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._equalsToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._startQuoteToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._reference, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endQuoteToken, IObjectWritable))
		End Sub
	End Class
End Namespace