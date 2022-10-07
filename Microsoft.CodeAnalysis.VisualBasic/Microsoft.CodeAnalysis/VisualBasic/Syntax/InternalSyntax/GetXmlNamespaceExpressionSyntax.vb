Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class GetXmlNamespaceExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _getXmlNamespaceKeyword As KeywordSyntax

		Friend ReadOnly _openParenToken As PunctuationSyntax

		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax

		Friend ReadOnly _closeParenToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property CloseParenToken As PunctuationSyntax
			Get
				Return Me._closeParenToken
			End Get
		End Property

		Friend ReadOnly Property GetXmlNamespaceKeyword As KeywordSyntax
			Get
				Return Me._getXmlNamespaceKeyword
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property OpenParenToken As PunctuationSyntax
			Get
				Return Me._openParenToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal getXmlNamespaceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(getXmlNamespaceKeyword)
			Me._getXmlNamespaceKeyword = getXmlNamespaceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (name IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(name)
				Me._name = name
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal getXmlNamespaceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax, ByVal closeParenToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(getXmlNamespaceKeyword)
			Me._getXmlNamespaceKeyword = getXmlNamespaceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (name IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(name)
				Me._name = name
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal getXmlNamespaceKeyword As KeywordSyntax, ByVal openParenToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax, ByVal closeParenToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(getXmlNamespaceKeyword)
			Me._getXmlNamespaceKeyword = getXmlNamespaceKeyword
			MyBase.AdjustFlagsAndWidth(openParenToken)
			Me._openParenToken = openParenToken
			If (name IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(name)
				Me._name = name
			End If
			MyBase.AdjustFlagsAndWidth(closeParenToken)
			Me._closeParenToken = closeParenToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._getXmlNamespaceKeyword = keywordSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openParenToken = punctuationSyntax
			End If
			Dim xmlPrefixNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax)
			If (xmlPrefixNameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(xmlPrefixNameSyntax)
				Me._name = xmlPrefixNameSyntax
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._closeParenToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitGetXmlNamespaceExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.GetXmlNamespaceExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._getXmlNamespaceKeyword
					Exit Select
				Case 1
					greenNode = Me._openParenToken
					Exit Select
				Case 2
					greenNode = Me._name
					Exit Select
				Case 3
					greenNode = Me._closeParenToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._getXmlNamespaceKeyword, Me._openParenToken, Me._name, Me._closeParenToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._getXmlNamespaceKeyword, Me._openParenToken, Me._name, Me._closeParenToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._getXmlNamespaceKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openParenToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeParenToken, IObjectWritable))
		End Sub
	End Class
End Namespace