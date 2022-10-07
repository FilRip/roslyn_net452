Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlPrefixNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax
		Friend ReadOnly _name As XmlNameTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Name As XmlNameTokenSyntax
			Get
				Return Me._name
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As XmlNameTokenSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal name As XmlNameTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 1
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As XmlNameTokenSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 1
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 1
			Dim xmlNameTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameTokenSyntax)
			If (xmlNameTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlNameTokenSyntax)
				Me._name = xmlNameTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlPrefixName(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlPrefixNameSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			If (i <> 0) Then
				greenNode = Nothing
			Else
				greenNode = Me._name
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._name)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._name)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
		End Sub
	End Class
End Namespace