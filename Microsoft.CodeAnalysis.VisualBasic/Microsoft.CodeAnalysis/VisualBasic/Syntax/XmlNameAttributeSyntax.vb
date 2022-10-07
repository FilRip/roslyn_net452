Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlNameAttributeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.BaseXmlAttributeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax

		Friend _reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Public ReadOnly Property EndQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)._endQuoteToken, Me.GetChildPosition(4), MyBase.GetChildIndex(4))
			End Get
		End Property

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)._equalsToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)(Me._name)
			End Get
		End Property

		Public ReadOnly Property Reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._reference, 3)
			End Get
		End Property

		Public ReadOnly Property StartQuoteToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)._startQuoteToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal equalsToken As PunctuationSyntax, ByVal startQuoteToken As PunctuationSyntax, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal endQuoteToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(kind, errors, annotations, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), equalsToken, startQuoteToken, DirectCast(reference.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), endQuoteToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlNameAttribute(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlNameAttribute(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._name
			ElseIf (num = 3) Then
				syntaxNode = Me._reference
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				name = Me.Name
			ElseIf (num = 3) Then
				name = Me.Reference
			Else
				name = Nothing
			End If
			Return name
		End Function

		Public Function Update(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal startQuoteToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal endQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Dim xmlNameAttributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			If (name <> Me.Name OrElse equalsToken <> Me.EqualsToken OrElse startQuoteToken <> Me.StartQuoteToken OrElse reference <> Me.Reference OrElse endQuoteToken <> Me.EndQuoteToken) Then
				Dim xmlNameAttributeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlNameAttribute(name, equalsToken, startQuoteToken, reference, endQuoteToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlNameAttributeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlNameAttributeSyntax1, xmlNameAttributeSyntax1.WithAnnotations(annotations))
			Else
				xmlNameAttributeSyntax = Me
			End If
			Return xmlNameAttributeSyntax
		End Function

		Public Function WithEndQuoteToken(ByVal endQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Return Me.Update(Me.Name, Me.EqualsToken, Me.StartQuoteToken, Me.Reference, endQuoteToken)
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Return Me.Update(Me.Name, equalsToken, Me.StartQuoteToken, Me.Reference, Me.EndQuoteToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Return Me.Update(name, Me.EqualsToken, Me.StartQuoteToken, Me.Reference, Me.EndQuoteToken)
		End Function

		Public Function WithReference(ByVal reference As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Return Me.Update(Me.Name, Me.EqualsToken, Me.StartQuoteToken, reference, Me.EndQuoteToken)
		End Function

		Public Function WithStartQuoteToken(ByVal startQuoteToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax
			Return Me.Update(Me.Name, Me.EqualsToken, startQuoteToken, Me.Reference, Me.EndQuoteToken)
		End Function
	End Class
End Namespace