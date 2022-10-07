Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlElementEndTagSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax

		Public ReadOnly Property GreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)._greaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property LessThanSlashToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)._lessThanSlashToken, MyBase.Position, 0)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanSlashToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal greaterThanToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax(kind, errors, annotations, lessThanSlashToken, If(name IsNot Nothing, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax), Nothing), greaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlElementEndTag(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlElementEndTag(Me)
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

		Public Function Update(ByVal lessThanSlashToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax, ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			Dim xmlElementEndTagSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			If (lessThanSlashToken <> Me.LessThanSlashToken OrElse name <> Me.Name OrElse greaterThanToken <> Me.GreaterThanToken) Then
				Dim xmlElementEndTagSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlElementEndTag(lessThanSlashToken, name, greaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlElementEndTagSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlElementEndTagSyntax1, xmlElementEndTagSyntax1.WithAnnotations(annotations))
			Else
				xmlElementEndTagSyntax = Me
			End If
			Return xmlElementEndTagSyntax
		End Function

		Public Function WithGreaterThanToken(ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			Return Me.Update(Me.LessThanSlashToken, Me.Name, greaterThanToken)
		End Function

		Public Function WithLessThanSlashToken(ByVal lessThanSlashToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			Return Me.Update(lessThanSlashToken, Me.Name, Me.GreaterThanToken)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementEndTagSyntax
			Return Me.Update(Me.LessThanSlashToken, name, Me.GreaterThanToken)
		End Function
	End Class
End Namespace