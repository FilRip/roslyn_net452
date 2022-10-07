Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class XmlNamespaceImportsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax
		Friend ReadOnly _lessThanToken As PunctuationSyntax

		Friend ReadOnly _xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax

		Friend ReadOnly _greaterThanToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

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

		Friend ReadOnly Property XmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax
			Get
				Return Me._xmlNamespace
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(xmlNamespace)
			Me._xmlNamespace = xmlNamespace
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(xmlNamespace)
			Me._xmlNamespace = xmlNamespace
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(lessThanToken)
			Me._lessThanToken = lessThanToken
			MyBase.AdjustFlagsAndWidth(xmlNamespace)
			Me._xmlNamespace = xmlNamespace
			MyBase.AdjustFlagsAndWidth(greaterThanToken)
			Me._greaterThanToken = greaterThanToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._lessThanToken = punctuationSyntax
			End If
			Dim xmlAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)
			If (xmlAttributeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlAttributeSyntax)
				Me._xmlNamespace = xmlAttributeSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._greaterThanToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitXmlNamespaceImportsClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._lessThanToken
					Exit Select
				Case 1
					greenNode = Me._xmlNamespace
					Exit Select
				Case 2
					greenNode = Me._greaterThanToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._lessThanToken, Me._xmlNamespace, Me._greaterThanToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._lessThanToken, Me._xmlNamespace, Me._greaterThanToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._lessThanToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._xmlNamespace, IObjectWritable))
			writer.WriteValue(DirectCast(Me._greaterThanToken, IObjectWritable))
		End Sub
	End Class
End Namespace