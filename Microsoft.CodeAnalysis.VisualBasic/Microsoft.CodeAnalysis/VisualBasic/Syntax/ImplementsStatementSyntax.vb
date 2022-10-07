Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ImplementsStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax
		Friend _types As SyntaxNode

		Public ReadOnly Property ImplementsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)._implementsKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Types As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
			Get
				Dim typeSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._types, 1)
				typeSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(red, MyBase.GetChildIndex(1)))
				Return typeSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal implementsKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal types As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax(kind, errors, annotations, implementsKeyword, If(types IsNot Nothing, types.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitImplementsStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitImplementsStatement(Me)
		End Sub

		Public Function AddTypes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax
			Return Me.WithTypes(Me.Types.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._types
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._types, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal implementsKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal types As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax
			Dim implementsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax
			If (implementsKeyword <> Me.ImplementsKeyword OrElse types <> Me.Types) Then
				Dim implementsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ImplementsStatement(implementsKeyword, types)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				implementsStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, implementsStatementSyntax1, implementsStatementSyntax1.WithAnnotations(annotations))
			Else
				implementsStatementSyntax = Me
			End If
			Return implementsStatementSyntax
		End Function

		Public Function WithImplementsKeyword(ByVal implementsKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax
			Return Me.Update(implementsKeyword, Me.Types)
		End Function

		Public Function WithTypes(ByVal types As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsStatementSyntax
			Return Me.Update(Me.ImplementsKeyword, types)
		End Function
	End Class
End Namespace