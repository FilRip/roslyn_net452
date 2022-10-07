Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NamedTupleElementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TupleElementSyntax
		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 1)
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax(kind, errors, annotations, identifier, If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNamedTupleElement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNamedTupleElement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._asClause
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim asClause As SyntaxNode
			If (i <> 1) Then
				asClause = Nothing
			Else
				asClause = Me.AsClause
			End If
			Return asClause
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax
			Dim namedTupleElementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax
			If (identifier <> Me.Identifier OrElse asClause <> Me.AsClause) Then
				Dim namedTupleElementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NamedTupleElement(identifier, asClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				namedTupleElementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, namedTupleElementSyntax1, namedTupleElementSyntax1.WithAnnotations(annotations))
			Else
				namedTupleElementSyntax = Me
			End If
			Return namedTupleElementSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax
			Return Me.Update(Me.Identifier, asClause)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamedTupleElementSyntax
			Return Me.Update(identifier, Me.AsClause)
		End Function
	End Class
End Namespace