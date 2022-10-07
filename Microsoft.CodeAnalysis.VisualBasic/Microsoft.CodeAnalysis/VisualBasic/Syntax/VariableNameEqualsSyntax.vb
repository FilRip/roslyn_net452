Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class VariableNameEqualsSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax)(Me._asClause, 1)
			End Get
		End Property

		Public ReadOnly Property EqualsToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)._equalsToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(Me._identifier)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal equalsToken As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax(kind, errors, annotations, DirectCast(identifier.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Nothing), equalsToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitVariableNameEquals(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitVariableNameEquals(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._identifier
			ElseIf (num = 1) Then
				syntaxNode = Me._asClause
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim identifier As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				identifier = Me.Identifier
			ElseIf (num = 1) Then
				identifier = Me.AsClause
			Else
				identifier = Nothing
			End If
			Return identifier
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax, ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Dim variableNameEqualsSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			If (identifier <> Me.Identifier OrElse asClause <> Me.AsClause OrElse equalsToken <> Me.EqualsToken) Then
				Dim variableNameEqualsSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.VariableNameEquals(identifier, asClause, equalsToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				variableNameEqualsSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, variableNameEqualsSyntax1, variableNameEqualsSyntax1.WithAnnotations(annotations))
			Else
				variableNameEqualsSyntax = Me
			End If
			Return variableNameEqualsSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Return Me.Update(Me.Identifier, asClause, Me.EqualsToken)
		End Function

		Public Function WithEqualsToken(ByVal equalsToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Return Me.Update(Me.Identifier, Me.AsClause, equalsToken)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Return Me.Update(identifier, Me.AsClause, Me.EqualsToken)
		End Function
	End Class
End Namespace