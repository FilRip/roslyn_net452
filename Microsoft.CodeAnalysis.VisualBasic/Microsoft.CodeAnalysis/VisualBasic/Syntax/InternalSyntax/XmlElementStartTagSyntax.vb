Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlElementStartTagSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _lessThanToken As PunctuationSyntax

		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax

		Friend ReadOnly _attributes As GreenNode

		Friend ReadOnly _greaterThanToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Attributes As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(Me._attributes)
			End Get
		End Property

		Friend ReadOnly Property GreaterThanToken As PunctuationSyntax
			Get
				Return Me._greaterThanToken
			End Get
		End Property

		Friend ReadOnly Property LessThanToken As PunctuationSyntax
			Get
				Return Me._lessThanToken
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
			Get
				Return Me._name
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As GreenNode, ByVal greaterThanToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As GreenNode, ByVal greaterThanToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax, ByVal attributes As GreenNode, ByVal greaterThanToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			If (attributes IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributes)
				Me._attributes = attributes
			End If
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._lessThanToken = punctuationSyntax
			End If
			Dim xmlNodeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			If (xmlNodeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNodeSyntax)
				Me._name = xmlNodeSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._attributes = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._greaterThanToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlElementStartTag(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._lessThanToken
					Exit Select
				Case 1
					greenNode = Me._name
					Exit Select
				Case 2
					greenNode = Me._attributes
					Exit Select
				Case 3
					greenNode = Me._greaterThanToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._lessThanToken, Me._name, Me._attributes, Me._greaterThanToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._lessThanToken, Me._name, Me._attributes, Me._greaterThanToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._lessThanToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._attributes, IObjectWritable))
			writer.WriteValue(DirectCast(Me._greaterThanToken, IObjectWritable))
		End Sub
	End Class
End Namespace