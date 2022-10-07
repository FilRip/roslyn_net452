Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ReDimStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _clauses As SyntaxNode

		Public ReadOnly Property Clauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)
			Get
				Dim redimClauseSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._clauses, 2)
				redimClauseSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)(red, MyBase.GetChildIndex(2)))
				Return redimClauseSyntaxes
			End Get
		End Property

		Public ReadOnly Property PreserveKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)._preserveKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxToken
			End Get
		End Property

		Public ReadOnly Property ReDimKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)._reDimKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal reDimKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal preserveKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal clauses As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(kind, errors, annotations, reDimKeyword, preserveKeyword, If(clauses IsNot Nothing, clauses.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitReDimStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitReDimStatement(Me)
		End Sub

		Public Function AddClauses(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			Return Me.WithClauses(Me.Clauses.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._clauses
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._clauses, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal kind As SyntaxKind, ByVal reDimKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal preserveKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal clauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			Dim reDimStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			If (kind <> MyBase.Kind() OrElse reDimKeyword <> Me.ReDimKeyword OrElse preserveKeyword <> Me.PreserveKeyword OrElse clauses <> Me.Clauses) Then
				Dim reDimStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ReDimStatement(kind, reDimKeyword, preserveKeyword, clauses)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				reDimStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, reDimStatementSyntax1, reDimStatementSyntax1.WithAnnotations(annotations))
			Else
				reDimStatementSyntax = Me
			End If
			Return reDimStatementSyntax
		End Function

		Public Function WithClauses(ByVal clauses As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.ReDimKeyword, Me.PreserveKeyword, clauses)
		End Function

		Public Function WithPreserveKeyword(ByVal preserveKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			Return Me.Update(MyBase.Kind(), Me.ReDimKeyword, preserveKeyword, Me.Clauses)
		End Function

		Public Function WithReDimKeyword(ByVal reDimKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax
			Return Me.Update(MyBase.Kind(), reDimKeyword, Me.PreserveKeyword, Me.Clauses)
		End Function
	End Class
End Namespace