Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ObjectCollectionInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax
		Friend _initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax

		Public ReadOnly Property FromKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax)._fromKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)(Me._initializer, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal fromKeyword As KeywordSyntax, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax(kind, errors, annotations, fromKeyword, DirectCast(initializer.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitObjectCollectionInitializer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitObjectCollectionInitializer(Me)
		End Sub

		Public Function AddInitializerInitializers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax
			Return Me.WithInitializer(If(Me.Initializer IsNot Nothing, Me.Initializer, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.CollectionInitializer()).AddInitializers(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._initializer
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim initializer As SyntaxNode
			If (i <> 1) Then
				initializer = Nothing
			Else
				initializer = Me.Initializer
			End If
			Return initializer
		End Function

		Public Function Update(ByVal fromKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax
			Dim objectCollectionInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax
			If (fromKeyword <> Me.FromKeyword OrElse initializer <> Me.Initializer) Then
				Dim objectCollectionInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ObjectCollectionInitializer(fromKeyword, initializer)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				objectCollectionInitializerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, objectCollectionInitializerSyntax1, objectCollectionInitializerSyntax1.WithAnnotations(annotations))
			Else
				objectCollectionInitializerSyntax = Me
			End If
			Return objectCollectionInitializerSyntax
		End Function

		Public Function WithFromKeyword(ByVal fromKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax
			Return Me.Update(fromKeyword, Me.Initializer)
		End Function

		Public Function WithInitializer(ByVal initializer As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCollectionInitializerSyntax
			Return Me.Update(Me.FromKeyword, initializer)
		End Function
	End Class
End Namespace