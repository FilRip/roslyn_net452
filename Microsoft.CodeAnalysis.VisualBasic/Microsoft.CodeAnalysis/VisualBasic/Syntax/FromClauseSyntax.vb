Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class FromClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryClauseSyntax
		Friend _variables As SyntaxNode

		Public ReadOnly Property FromKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax)._fromKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
			Get
				Dim collectionRangeVariableSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._variables, 1)
				collectionRangeVariableSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)(red, MyBase.GetChildIndex(1)))
				Return collectionRangeVariableSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal fromKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal variables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(kind, errors, annotations, fromKeyword, If(variables IsNot Nothing, variables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitFromClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitFromClause(Me)
		End Sub

		Public Function AddVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax
			Return Me.WithVariables(Me.Variables.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._variables
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._variables, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal fromKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax
			Dim fromClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax
			If (fromKeyword <> Me.FromKeyword OrElse variables <> Me.Variables) Then
				Dim fromClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FromClause(fromKeyword, variables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				fromClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, fromClauseSyntax1, fromClauseSyntax1.WithAnnotations(annotations))
			Else
				fromClauseSyntax = Me
			End If
			Return fromClauseSyntax
		End Function

		Public Function WithFromKeyword(ByVal fromKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax
			Return Me.Update(fromKeyword, Me.Variables)
		End Function

		Public Function WithVariables(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax
			Return Me.Update(Me.FromKeyword, variables)
		End Function
	End Class
End Namespace