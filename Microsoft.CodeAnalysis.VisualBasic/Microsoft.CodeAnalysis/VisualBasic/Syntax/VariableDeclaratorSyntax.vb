Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class VariableDeclaratorSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _names As SyntaxNode

		Friend _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax

		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax

		Public ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax)(Me._asClause, 1)
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)(Me._initializer, 2)
			End Get
		End Property

		Public ReadOnly Property Names As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
			Get
				Dim modifiedIdentifierSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
				Dim redAtZero As SyntaxNode = MyBase.GetRedAtZero(Me._names)
				modifiedIdentifierSyntaxes = If(redAtZero Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)(redAtZero, 0))
				Return modifiedIdentifierSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal names As SyntaxNode, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax(kind, errors, annotations, If(names IsNot Nothing, names.Green, Nothing), If(asClause IsNot Nothing, DirectCast(asClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax), Nothing), If(initializer IsNot Nothing, DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitVariableDeclarator(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitVariableDeclarator(Me)
		End Sub

		Public Function AddNames(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			Return Me.WithNames(Me.Names.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._names
					Exit Select
				Case 1
					syntaxNode = Me._asClause
					Exit Select
				Case 2
					syntaxNode = Me._initializer
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim redAtZero As SyntaxNode
			Select Case i
				Case 0
					redAtZero = MyBase.GetRedAtZero(Me._names)
					Exit Select
				Case 1
					redAtZero = Me.AsClause
					Exit Select
				Case 2
					redAtZero = Me.Initializer
					Exit Select
				Case Else
					redAtZero = Nothing
					Exit Select
			End Select
			Return redAtZero
		End Function

		Public Function Update(ByVal names As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax), ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			If (names <> Me.Names OrElse asClause <> Me.AsClause OrElse initializer <> Me.Initializer) Then
				Dim variableDeclaratorSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.VariableDeclarator(names, asClause, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				variableDeclaratorSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, variableDeclaratorSyntax1, variableDeclaratorSyntax1.WithAnnotations(annotations))
			Else
				variableDeclaratorSyntax = Me
			End If
			Return variableDeclaratorSyntax
		End Function

		Public Function WithAsClause(ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			Return Me.Update(Me.Names, asClause, Me.Initializer)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			Return Me.Update(Me.Names, Me.AsClause, initializer)
		End Function

		Public Function WithNames(ByVal names As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax
			Return Me.Update(names, Me.AsClause, Me.Initializer)
		End Function
	End Class
End Namespace