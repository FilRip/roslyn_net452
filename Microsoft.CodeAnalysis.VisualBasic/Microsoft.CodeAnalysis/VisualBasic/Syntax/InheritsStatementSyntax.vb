Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class InheritsStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsOrImplementsStatementSyntax
		Friend _types As SyntaxNode

		Public ReadOnly Property InheritsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)._inheritsKeyword, MyBase.Position, 0)
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

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal inheritsKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal types As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax(kind, errors, annotations, inheritsKeyword, If(types IsNot Nothing, types.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitInheritsStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitInheritsStatement(Me)
		End Sub

		Public Function AddTypes(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax
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

		Public Function Update(ByVal inheritsKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal types As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax
			Dim inheritsStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax
			If (inheritsKeyword <> Me.InheritsKeyword OrElse types <> Me.Types) Then
				Dim inheritsStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.InheritsStatement(inheritsKeyword, types)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				inheritsStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, inheritsStatementSyntax1, inheritsStatementSyntax1.WithAnnotations(annotations))
			Else
				inheritsStatementSyntax = Me
			End If
			Return inheritsStatementSyntax
		End Function

		Public Function WithInheritsKeyword(ByVal inheritsKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax
			Return Me.Update(inheritsKeyword, Me.Types)
		End Function

		Public Function WithTypes(ByVal types As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InheritsStatementSyntax
			Return Me.Update(Me.InheritsKeyword, types)
		End Function
	End Class
End Namespace