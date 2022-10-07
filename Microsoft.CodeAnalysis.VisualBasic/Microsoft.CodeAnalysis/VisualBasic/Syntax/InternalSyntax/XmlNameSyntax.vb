Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax

		Friend ReadOnly _localName As XmlNameTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property LocalName As XmlNameTokenSyntax
			Get
				Return Me._localName
			End Get
		End Property

		Friend ReadOnly Property Prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax
			Get
				Return Me._prefix
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			If (prefix IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(prefix)
				Me._prefix = prefix
			End If
			MyBase.AdjustFlagsAndWidth(localName)
			Me._localName = localName
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			If (prefix IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(prefix)
				Me._prefix = prefix
			End If
			MyBase.AdjustFlagsAndWidth(localName)
			Me._localName = localName
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal prefix As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax, ByVal localName As XmlNameTokenSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			If (prefix IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(prefix)
				Me._prefix = prefix
			End If
			MyBase.AdjustFlagsAndWidth(localName)
			Me._localName = localName
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim xmlPrefixSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax)
			If (xmlPrefixSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlPrefixSyntax)
				Me._prefix = xmlPrefixSyntax
			End If
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (xmlNameTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNameTokenSyntax)
				Me._localName = xmlNameTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlName(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._prefix
			ElseIf (num = 1) Then
				greenNode = Me._localName
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._prefix, Me._localName)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._prefix, Me._localName)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._prefix, IObjectWritable))
			writer.WriteValue(DirectCast(Me._localName, IObjectWritable))
		End Sub
	End Class
End Namespace