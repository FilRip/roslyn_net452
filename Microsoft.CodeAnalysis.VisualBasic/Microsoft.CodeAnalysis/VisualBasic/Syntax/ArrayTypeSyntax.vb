Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ArrayTypeSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
		Friend _elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax

		Friend _rankSpecifiers As SyntaxNode

		Public ReadOnly Property ElementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax)(Me._elementType)
			End Get
		End Property

		Public ReadOnly Property RankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)(MyBase.GetRed(Me._rankSpecifiers, 1))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal rankSpecifiers As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax(kind, errors, annotations, DirectCast(elementType.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax), If(rankSpecifiers IsNot Nothing, rankSpecifiers.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitArrayType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitArrayType(Me)
		End Sub

		Public Function AddRankSpecifiers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax
			Return Me.WithRankSpecifiers(Me.RankSpecifiers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._elementType
			ElseIf (num = 1) Then
				syntaxNode = Me._rankSpecifiers
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim elementType As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				elementType = Me.ElementType
			ElseIf (num = 1) Then
				elementType = MyBase.GetRed(Me._rankSpecifiers, 1)
			Else
				elementType = Nothing
			End If
			Return elementType
		End Function

		Public Function Update(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax, ByVal rankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax
			Dim arrayTypeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax
			If (elementType <> Me.ElementType OrElse rankSpecifiers <> Me.RankSpecifiers) Then
				Dim arrayTypeSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArrayType(elementType, rankSpecifiers)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				arrayTypeSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, arrayTypeSyntax1, arrayTypeSyntax1.WithAnnotations(annotations))
			Else
				arrayTypeSyntax = Me
			End If
			Return arrayTypeSyntax
		End Function

		Public Function WithElementType(ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax
			Return Me.Update(elementType, Me.RankSpecifiers)
		End Function

		Public Function WithRankSpecifiers(ByVal rankSpecifiers As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayRankSpecifierSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArrayTypeSyntax
			Return Me.Update(Me.ElementType, rankSpecifiers)
		End Function
	End Class
End Namespace