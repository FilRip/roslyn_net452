Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class XmlDocumentSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
		Friend _declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax

		Friend _precedingMisc As SyntaxNode

		Friend _root As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax

		Friend _followingMisc As SyntaxNode

		Public ReadOnly Property Declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax)(Me._declaration)
			End Get
		End Property

		Public ReadOnly Property FollowingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRed(Me._followingMisc, 3))
			End Get
		End Property

		Public ReadOnly Property PrecedingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(MyBase.GetRed(Me._precedingMisc, 1))
			End Get
		End Property

		Public ReadOnly Property Root As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)(Me._root, 2)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax, ByVal precedingMisc As SyntaxNode, ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal followingMisc As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax(kind, errors, annotations, DirectCast(declaration.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax), If(precedingMisc IsNot Nothing, precedingMisc.Green, Nothing), DirectCast(root.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax), If(followingMisc IsNot Nothing, followingMisc.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitXmlDocument(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitXmlDocument(Me)
		End Sub

		Public Function AddFollowingMisc(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.WithFollowingMisc(Me.FollowingMisc.AddRange(items))
		End Function

		Public Function AddPrecedingMisc(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.WithPrecedingMisc(Me.PrecedingMisc.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 0
					syntaxNode = Me._declaration
					Exit Select
				Case 1
					syntaxNode = Me._precedingMisc
					Exit Select
				Case 2
					syntaxNode = Me._root
					Exit Select
				Case 3
					syntaxNode = Me._followingMisc
					Exit Select
				Case Else
					syntaxNode = Nothing
					Exit Select
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim declaration As SyntaxNode
			Select Case i
				Case 0
					declaration = Me.Declaration
					Exit Select
				Case 1
					declaration = MyBase.GetRed(Me._precedingMisc, 1)
					Exit Select
				Case 2
					declaration = Me.Root
					Exit Select
				Case 3
					declaration = MyBase.GetRed(Me._followingMisc, 3)
					Exit Select
				Case Else
					declaration = Nothing
					Exit Select
			End Select
			Return declaration
		End Function

		Public Function Update(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax, ByVal precedingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax), ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax, ByVal followingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Dim xmlDocumentSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			If (declaration <> Me.Declaration OrElse precedingMisc <> Me.PrecedingMisc OrElse root <> Me.Root OrElse followingMisc <> Me.FollowingMisc) Then
				Dim xmlDocumentSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.XmlDocument(declaration, precedingMisc, root, followingMisc)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				xmlDocumentSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, xmlDocumentSyntax1, xmlDocumentSyntax1.WithAnnotations(annotations))
			Else
				xmlDocumentSyntax = Me
			End If
			Return xmlDocumentSyntax
		End Function

		Public Function WithDeclaration(ByVal declaration As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.Update(declaration, Me.PrecedingMisc, Me.Root, Me.FollowingMisc)
		End Function

		Public Function WithFollowingMisc(ByVal followingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.Update(Me.Declaration, Me.PrecedingMisc, Me.Root, followingMisc)
		End Function

		Public Function WithPrecedingMisc(ByVal precedingMisc As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.Update(Me.Declaration, precedingMisc, Me.Root, Me.FollowingMisc)
		End Function

		Public Function WithRoot(ByVal root As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDocumentSyntax
			Return Me.Update(Me.Declaration, Me.PrecedingMisc, root, Me.FollowingMisc)
		End Function
	End Class
End Namespace