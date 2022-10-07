Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ObjectMemberInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectCreationInitializerSyntax
		Friend _initializers As SyntaxNode

		Public ReadOnly Property CloseBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)._closeBraceToken, Me.GetChildPosition(3), MyBase.GetChildIndex(3))
			End Get
		End Property

		Public ReadOnly Property Initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)
			Get
				Dim fieldInitializerSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._initializers, 2)
				fieldInitializerSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)(red, MyBase.GetChildIndex(2)))
				Return fieldInitializerSyntaxes
			End Get
		End Property

		Public ReadOnly Property OpenBraceToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)._openBraceToken, Me.GetChildPosition(1), MyBase.GetChildIndex(1))
			End Get
		End Property

		Public ReadOnly Property WithKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)._withKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal withKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal openBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax, ByVal initializers As SyntaxNode, ByVal closeBraceToken As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(kind, errors, annotations, withKeyword, openBraceToken, If(initializers IsNot Nothing, initializers.Green, Nothing), closeBraceToken), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitObjectMemberInitializer(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitObjectMemberInitializer(Me)
		End Sub

		Public Function AddInitializers(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Return Me.WithInitializers(Me.Initializers.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 2) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._initializers
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim red As SyntaxNode
			If (i <> 2) Then
				red = Nothing
			Else
				red = MyBase.GetRed(Me._initializers, 2)
			End If
			Return red
		End Function

		Public Function Update(ByVal withKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax), ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Dim objectMemberInitializerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			If (withKeyword <> Me.WithKeyword OrElse openBraceToken <> Me.OpenBraceToken OrElse initializers <> Me.Initializers OrElse closeBraceToken <> Me.CloseBraceToken) Then
				Dim objectMemberInitializerSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ObjectMemberInitializer(withKeyword, openBraceToken, initializers, closeBraceToken)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				objectMemberInitializerSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, objectMemberInitializerSyntax1, objectMemberInitializerSyntax1.WithAnnotations(annotations))
			Else
				objectMemberInitializerSyntax = Me
			End If
			Return objectMemberInitializerSyntax
		End Function

		Public Function WithCloseBraceToken(ByVal closeBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Return Me.Update(Me.WithKeyword, Me.OpenBraceToken, Me.Initializers, closeBraceToken)
		End Function

		Public Function WithInitializers(ByVal initializers As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Return Me.Update(Me.WithKeyword, Me.OpenBraceToken, initializers, Me.CloseBraceToken)
		End Function

		Public Function WithOpenBraceToken(ByVal openBraceToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Return Me.Update(Me.WithKeyword, openBraceToken, Me.Initializers, Me.CloseBraceToken)
		End Function

		Public Function WithWithKeyword(ByVal withKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ObjectMemberInitializerSyntax
			Return Me.Update(withKeyword, Me.OpenBraceToken, Me.Initializers, Me.CloseBraceToken)
		End Function
	End Class
End Namespace