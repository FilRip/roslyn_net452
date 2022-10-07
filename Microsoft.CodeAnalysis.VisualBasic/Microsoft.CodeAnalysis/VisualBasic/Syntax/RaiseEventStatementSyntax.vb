Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class RaiseEventStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax

		Friend _argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Public ReadOnly Property ArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._argumentList, 2)
			End Get
		End Property

		Public ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)(Me._name, 1)
			End Get
		End Property

		Public ReadOnly Property RaiseEventKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax)._raiseEventKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal raiseEventKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax(kind, errors, annotations, raiseEventKeyword, DirectCast(name.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax), If(argumentList IsNot Nothing, DirectCast(argumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitRaiseEventStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitRaiseEventStatement(Me)
		End Sub

		Public Function AddArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			Return Me.WithArgumentList(If(Me.ArgumentList IsNot Nothing, Me.ArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._name
			ElseIf (num = 2) Then
				syntaxNode = Me._argumentList
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim name As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				name = Me.Name
			ElseIf (num = 2) Then
				name = Me.ArgumentList
			Else
				name = Nothing
			End If
			Return name
		End Function

		Public Function Update(ByVal raiseEventKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax, ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			Dim raiseEventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			If (raiseEventKeyword <> Me.RaiseEventKeyword OrElse name <> Me.Name OrElse argumentList <> Me.ArgumentList) Then
				Dim raiseEventStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.RaiseEventStatement(raiseEventKeyword, name, argumentList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				raiseEventStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, raiseEventStatementSyntax1, raiseEventStatementSyntax1.WithAnnotations(annotations))
			Else
				raiseEventStatementSyntax = Me
			End If
			Return raiseEventStatementSyntax
		End Function

		Public Function WithArgumentList(ByVal argumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			Return Me.Update(Me.RaiseEventKeyword, Me.Name, argumentList)
		End Function

		Public Function WithName(ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			Return Me.Update(Me.RaiseEventKeyword, name, Me.ArgumentList)
		End Function

		Public Function WithRaiseEventKeyword(ByVal raiseEventKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax
			Return Me.Update(raiseEventKeyword, Me.Name, Me.ArgumentList)
		End Function
	End Class
End Namespace