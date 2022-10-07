Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlNamespaceImportsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ImportsClauseSyntax
		Friend _xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax

		Public ReadOnly Property GreaterThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)._greaterThanToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property LessThanToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)._lessThanToken, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property XmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax)(Me._xmlNamespace, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal lessThanToken As PunctuationSyntax, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax, ByVal greaterThanToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(kind, errors, annotations, lessThanToken, DirectCast(xmlNamespace.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax), greaterThanToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlNamespaceImportsClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlNamespaceImportsClause(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._xmlNamespace
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim xmlNamespace As SyntaxNode
			If (i <> 1) Then
				xmlNamespace = Nothing
			Else
				xmlNamespace = Me.XmlNamespace
			End If
			Return xmlNamespace
		End Function

		Public Function Update(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax, ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax
			Dim xmlNamespaceImportsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax
			If (lessThanToken <> Me.LessThanToken OrElse xmlNamespace <> Me.XmlNamespace OrElse greaterThanToken <> Me.GreaterThanToken) Then
				Dim xmlNamespaceImportsClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlNamespaceImportsClause(lessThanToken, xmlNamespace, greaterThanToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlNamespaceImportsClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlNamespaceImportsClauseSyntax1, xmlNamespaceImportsClauseSyntax1.WithAnnotations(annotations))
			Else
				xmlNamespaceImportsClauseSyntax = Me
			End If
			Return xmlNamespaceImportsClauseSyntax
		End Function

		Public Function WithGreaterThanToken(ByVal greaterThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax
			Return Me.Update(Me.LessThanToken, Me.XmlNamespace, greaterThanToken)
		End Function

		Public Function WithLessThanToken(ByVal lessThanToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax
			Return Me.Update(lessThanToken, Me.XmlNamespace, Me.GreaterThanToken)
		End Function

		Public Function WithXmlNamespace(ByVal xmlNamespace As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNamespaceImportsClauseSyntax
			Return Me.Update(Me.LessThanToken, xmlNamespace, Me.GreaterThanToken)
		End Function
	End Class
End Namespace