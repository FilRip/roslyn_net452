Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class NamespaceBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax

		Friend _members As SyntaxNode

		Friend _endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endNamespaceStatement, 2)
			End Get
		End Property

		Public ReadOnly Property Members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._members, 1))
			End Get
		End Property

		Public ReadOnly Property NamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax)(Me._namespaceStatement)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax, ByVal members As SyntaxNode, ByVal endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax(kind, errors, annotations, DirectCast(namespaceStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax), If(members IsNot Nothing, members.Green, Nothing), DirectCast(endNamespaceStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitNamespaceBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitNamespaceBlock(Me)
		End Sub

		Public Function AddMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Return Me.WithMembers(Me.Members.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._namespaceStatement
					Exit Select
				Case 1
					syntaxNode = Me._members
					Exit Select
				Case 2
					syntaxNode = Me._endNamespaceStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim namespaceStatement As SyntaxNode
			Select Case i
				Case 0
					namespaceStatement = Me.NamespaceStatement
					Exit Select
				Case 1
					namespaceStatement = MyBase.GetRed(Me._members, 1)
					Exit Select
				Case 2
					namespaceStatement = Me.EndNamespaceStatement
					Exit Select
				Case Else
					namespaceStatement = Nothing
					Exit Select
			End Select
			Return namespaceStatement
		End Function

		Public Function Update(ByVal namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax, ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Dim namespaceBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			If (namespaceStatement <> Me.NamespaceStatement OrElse members <> Me.Members OrElse endNamespaceStatement <> Me.EndNamespaceStatement) Then
				Dim namespaceBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.NamespaceBlock(namespaceStatement, members, endNamespaceStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				namespaceBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, namespaceBlockSyntax1, namespaceBlockSyntax1.WithAnnotations(annotations))
			Else
				namespaceBlockSyntax = Me
			End If
			Return namespaceBlockSyntax
		End Function

		Public Function WithEndNamespaceStatement(ByVal endNamespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Return Me.Update(Me.NamespaceStatement, Me.Members, endNamespaceStatement)
		End Function

		Public Function WithMembers(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Return Me.Update(Me.NamespaceStatement, members, Me.EndNamespaceStatement)
		End Function

		Public Function WithNamespaceStatement(ByVal namespaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.NamespaceBlockSyntax
			Return Me.Update(namespaceStatement, Me.Members, Me.EndNamespaceStatement)
		End Function
	End Class
End Namespace