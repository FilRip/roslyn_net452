Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EnumBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclarationStatementSyntax
		Friend _enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax

		Friend _members As SyntaxNode

		Friend _endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax

		Public ReadOnly Property EndEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)(Me._endEnumStatement, 2)
			End Get
		End Property

		Public ReadOnly Property EnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax)(Me._enumStatement)
			End Get
		End Property

		Public ReadOnly Property Members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._members, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax, ByVal members As SyntaxNode, ByVal endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax(kind, errors, annotations, DirectCast(enumStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax), If(members IsNot Nothing, members.Green, Nothing), DirectCast(endEnumStatement.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEnumBlock(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEnumBlock(Me)
		End Sub

		Public Function AddMembers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			Return Me.WithMembers(Me.Members.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._enumStatement
					Exit Select
				Case 1
					syntaxNode = Me._members
					Exit Select
				Case 2
					syntaxNode = Me._endEnumStatement
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim enumStatement As SyntaxNode
			Select Case i
				Case 0
					enumStatement = Me.EnumStatement
					Exit Select
				Case 1
					enumStatement = MyBase.GetRed(Me._members, 1)
					Exit Select
				Case 2
					enumStatement = Me.EndEnumStatement
					Exit Select
				Case Else
					enumStatement = Nothing
					Exit Select
			End Select
			Return enumStatement
		End Function

		Public Function Update(ByVal enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax, ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			Dim enumBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			If (enumStatement <> Me.EnumStatement OrElse members <> Me.Members OrElse endEnumStatement <> Me.EndEnumStatement) Then
				Dim enumBlockSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EnumBlock(enumStatement, members, endEnumStatement)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				enumBlockSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, enumBlockSyntax1, enumBlockSyntax1.WithAnnotations(annotations))
			Else
				enumBlockSyntax = Me
			End If
			Return enumBlockSyntax
		End Function

		Public Function WithEndEnumStatement(ByVal endEnumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			Return Me.Update(Me.EnumStatement, Me.Members, endEnumStatement)
		End Function

		Public Function WithEnumStatement(ByVal enumStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			Return Me.Update(enumStatement, Me.Members, Me.EndEnumStatement)
		End Function

		Public Function WithMembers(ByVal members As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnumBlockSyntax
			Return Me.Update(Me.EnumStatement, members, Me.EndEnumStatement)
		End Function
	End Class
End Namespace