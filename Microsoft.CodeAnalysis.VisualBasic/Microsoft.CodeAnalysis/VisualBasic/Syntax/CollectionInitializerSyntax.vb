Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class CollectionInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
		Friend _initializers As SyntaxNode

		Public ReadOnly Property CloseBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)._closeBraceToken, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Public ReadOnly Property Initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			Get
				Dim expressionSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._initializers, 1)
				expressionSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(red, MyBase.GetChildIndex(1)))
				Return expressionSyntaxes
			End Get
		End Property

		Public ReadOnly Property OpenBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)._openBraceToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal openBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal initializers As SyntaxNode, ByVal closeBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(kind, errors, annotations, openBraceToken, If(initializers IsNot Nothing, initializers.Green, Nothing), closeBraceToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitCollectionInitializer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitCollectionInitializer(Me)
		End Sub

		Public Function AddInitializers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Return Me.WithInitializers(Me.Initializers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._initializers
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 1) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._initializers, 1)
			End If
			Return red
		End Function

		Public Function Update(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax), ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Dim collectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			If (openBraceToken <> Me.OpenBraceToken OrElse initializers <> Me.Initializers OrElse closeBraceToken <> Me.CloseBraceToken) Then
				Dim collectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CollectionInitializer(openBraceToken, initializers, closeBraceToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				collectionInitializerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, collectionInitializerSyntax1, collectionInitializerSyntax1.WithAnnotations(annotations))
			Else
				collectionInitializerSyntax = Me
			End If
			Return collectionInitializerSyntax
		End Function

		Public Function WithCloseBraceToken(ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Return Me.Update(Me.OpenBraceToken, Me.Initializers, closeBraceToken)
		End Function

		Public Function WithInitializers(ByVal initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Return Me.Update(Me.OpenBraceToken, initializers, Me.CloseBraceToken)
		End Function

		Public Function WithOpenBraceToken(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Return Me.Update(openBraceToken, Me.Initializers, Me.CloseBraceToken)
		End Function
	End Class
End Namespace