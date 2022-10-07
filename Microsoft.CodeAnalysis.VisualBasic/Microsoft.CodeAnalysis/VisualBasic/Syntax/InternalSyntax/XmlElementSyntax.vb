Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlElementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax

		Friend ReadOnly _content As GreenNode

		Friend ReadOnly _endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Content As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)(Me._content)
			End Get
		End Property

		Friend ReadOnly Property EndTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax
			Get
				Return Me._endTag
			End Get
		End Property

		Friend ReadOnly Property StartTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax
			Get
				Return Me._startTag
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax, ByVal content As GreenNode, ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(startTag)
			Me._startTag = startTag
			If (content IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(content)
				Me._content = content
			End If
			MyBase.AdjustFlagsAndWidth(endTag)
			Me._endTag = endTag
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax, ByVal content As GreenNode, ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(startTag)
			Me._startTag = startTag
			If (content IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(content)
				Me._content = content
			End If
			MyBase.AdjustFlagsAndWidth(endTag)
			Me._endTag = endTag
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal startTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax, ByVal content As GreenNode, ByVal endTag As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(startTag)
			Me._startTag = startTag
			If (content IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(content)
				Me._content = content
			End If
			MyBase.AdjustFlagsAndWidth(endTag)
			Me._endTag = endTag
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim xmlElementStartTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)
			If (xmlElementStartTagSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlElementStartTagSyntax)
				Me._startTag = xmlElementStartTagSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._content = greenNode
			End If
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)
			If (xmlElementEndTagSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlElementEndTagSyntax)
				Me._endTag = xmlElementEndTagSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlElement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._startTag
					Exit Select
				Case 1
					greenNode = Me._content
					Exit Select
				Case 2
					greenNode = Me._endTag
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._startTag, Me._content, Me._endTag)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._startTag, Me._content, Me._endTag)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._startTag, IObjectWritable))
			writer.WriteValue(DirectCast(Me._content, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endTag, IObjectWritable))
		End Sub
	End Class
End Namespace