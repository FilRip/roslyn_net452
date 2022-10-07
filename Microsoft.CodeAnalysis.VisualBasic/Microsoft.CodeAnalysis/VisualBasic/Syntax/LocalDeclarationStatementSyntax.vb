Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class LocalDeclarationStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _declarators As SyntaxNode

		Public ReadOnly Property Declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
			Get
				Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._declarators, 1)
				variableDeclaratorSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(red, MyBase.GetChildIndex(1)))
				Return variableDeclaratorSyntaxes
			End Get
		End Property

		Public ReadOnly Property Modifiers As SyntaxTokenList
			Get
				Dim syntaxTokenLists As SyntaxTokenList
				Dim green As GreenNode = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)._modifiers
				syntaxTokenLists = If(green Is Nothing, New SyntaxTokenList(), New SyntaxTokenList(Me, green, MyBase.Position, 0))
				Return syntaxTokenLists
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal modifiers As Microsoft.CodeAnalysis.GreenNode, ByVal declarators As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(kind, errors, annotations, modifiers, If(declarators IsNot Nothing, declarators.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitLocalDeclarationStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitLocalDeclarationStatement(Me)
		End Sub

		Public Function AddDeclarators(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			Return Me.WithDeclarators(Me.Declarators.AddRange(items))
		End Function

		Public Function AddModifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.SyntaxToken()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			Return Me.WithModifiers(Me.Modifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._declarators
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._declarators, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal modifiers As SyntaxTokenList, ByVal declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			Dim localDeclarationStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			If (modifiers <> Me.Modifiers OrElse declarators <> Me.Declarators) Then
				Dim localDeclarationStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.LocalDeclarationStatement(modifiers, declarators)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				localDeclarationStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, localDeclarationStatementSyntax1, localDeclarationStatementSyntax1.WithAnnotations(annotations))
			Else
				localDeclarationStatementSyntax = Me
			End If
			Return localDeclarationStatementSyntax
		End Function

		Public Function WithDeclarators(ByVal declarators As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			Return Me.Update(Me.Modifiers, declarators)
		End Function

		Public Function WithModifiers(ByVal modifiers As SyntaxTokenList) As Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax
			Return Me.Update(modifiers, Me.Declarators)
		End Function
	End Class
End Namespace