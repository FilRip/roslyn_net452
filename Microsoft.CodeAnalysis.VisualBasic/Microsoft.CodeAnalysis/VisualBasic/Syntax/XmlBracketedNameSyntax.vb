Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlBracketedNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax

		Public ReadOnly Property GreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)._greaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property LessThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)._lessThanToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)(Me._name, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal greaterThanToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax(kind, errors, annotations, lessThanToken, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), greaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlBracketedName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlBracketedName(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._name
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			If (i <> 1) Then
				name = Nothing
			Else
				name = Me.Name
			End If
			Return name
		End Function

		Public Function Update(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax
			Dim xmlBracketedNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax
			If (lessThanToken <> Me.LessThanToken OrElse name <> Me.Name OrElse greaterThanToken <> Me.GreaterThanToken) Then
				Dim xmlBracketedNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlBracketedName(lessThanToken, name, greaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlBracketedNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlBracketedNameSyntax1, xmlBracketedNameSyntax1.WithAnnotations(annotations))
			Else
				xmlBracketedNameSyntax = Me
			End If
			Return xmlBracketedNameSyntax
		End Function

		Public Function WithGreaterThanToken(ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax
			Return Me.Update(Me.LessThanToken, Me.Name, greaterThanToken)
		End Function

		Public Function WithLessThanToken(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax
			Return Me.Update(lessThanToken, Me.Name, Me.GreaterThanToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax
			Return Me.Update(Me.LessThanToken, name, Me.GreaterThanToken)
		End Function
	End Class
End Namespace