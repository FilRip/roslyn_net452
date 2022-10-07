Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ModifiedIdentifierSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Friend _arrayRankSpecifiers As SyntaxNode

		Public ReadOnly Property ArrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._arrayBounds, 2)
			End Get
		End Property

		Public ReadOnly Property ArrayRankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(MyBase.GetRed(Me._arrayRankSpecifiers, 3))
			End Get
		End Property

		Public ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Nullable As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As PunctuationSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)._nullable
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(1), MyBase.GetChildIndex(1)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax, ByVal nullable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(kind, errors, annotations, identifier, nullable, If(arrayBounds IsNot Nothing, DirectCast(arrayBounds.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax), Nothing), If(arrayRankSpecifiers IsNot Nothing, arrayRankSpecifiers.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitModifiedIdentifier(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitModifiedIdentifier(Me)
		End Sub

		Public Function AddArrayBoundsArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.WithArrayBounds(If(Me.ArrayBounds IsNot Nothing, Me.ArrayBounds, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Public Function AddArrayRankSpecifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.WithArrayRankSpecifiers(Me.ArrayRankSpecifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				syntaxNode = Me._arrayBounds
			ElseIf (num = 3) Then
				syntaxNode = Me._arrayRankSpecifiers
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim arrayBounds As SyntaxNode
			Dim num As Integer = i
			If (num = 2) Then
				arrayBounds = Me.ArrayBounds
			ElseIf (num = 3) Then
				arrayBounds = MyBase.GetRed(Me._arrayRankSpecifiers, 3)
			Else
				arrayBounds = Nothing
			End If
			Return arrayBounds
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal nullable As Microsoft.CodeAnalysis.SyntaxToken, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax, ByVal arrayRankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			If (identifier <> Me.Identifier OrElse nullable <> Me.Nullable OrElse arrayBounds <> Me.ArrayBounds OrElse arrayRankSpecifiers <> Me.ArrayRankSpecifiers) Then
				Dim modifiedIdentifierSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ModifiedIdentifier(identifier, nullable, arrayBounds, arrayRankSpecifiers)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				modifiedIdentifierSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, modifiedIdentifierSyntax1, modifiedIdentifierSyntax1.WithAnnotations(annotations))
			Else
				modifiedIdentifierSyntax = Me
			End If
			Return modifiedIdentifierSyntax
		End Function

		Public Function WithArrayBounds(ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.Update(Me.Identifier, Me.Nullable, arrayBounds, Me.ArrayRankSpecifiers)
		End Function

		Public Function WithArrayRankSpecifiers(ByVal arrayRankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.Update(Me.Identifier, Me.Nullable, Me.ArrayBounds, arrayRankSpecifiers)
		End Function

		Public Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.Update(identifier, Me.Nullable, Me.ArrayBounds, Me.ArrayRankSpecifiers)
		End Function

		Public Function WithNullable(ByVal nullable As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax
			Return Me.Update(Me.Identifier, nullable, Me.ArrayBounds, Me.ArrayRankSpecifiers)
		End Function
	End Class
End Namespace