Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlDeclarationOptionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _name As XmlNameTokenSyntax

		Friend ReadOnly _equals As PunctuationSyntax

		Friend ReadOnly _value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Equals As PunctuationSyntax
			Get
				Return Me._equals
			End Get
		End Property

		Friend ReadOnly Property Name As XmlNameTokenSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property Value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax
			Get
				Return Me._value
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equals)
			Me._equals = equals
			MyBase.AdjustFlagsAndWidth(value)
			Me._value = value
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equals)
			Me._equals = equals
			MyBase.AdjustFlagsAndWidth(value)
			Me._value = value
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As XmlNameTokenSyntax, ByVal equals As PunctuationSyntax, ByVal value As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
			MyBase.AdjustFlagsAndWidth(equals)
			Me._equals = equals
			MyBase.AdjustFlagsAndWidth(value)
			Me._value = value
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (xmlNameTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNameTokenSyntax)
				Me._name = xmlNameTokenSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._equals = punctuationSyntax
			End If
			Dim xmlStringSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)
			If (xmlStringSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlStringSyntax)
				Me._value = xmlStringSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlDeclarationOption(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._name
					Exit Select
				Case 1
					greenNode = Me._equals
					Exit Select
				Case 2
					greenNode = Me._value
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._name, Me._equals, Me._value)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._name, Me._equals, Me._value)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._equals, IObjectWritable))
			writer.WriteValue(DirectCast(Me._value, IObjectWritable))
		End Sub
	End Class
End Namespace