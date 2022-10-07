Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class GenericNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
		Friend _typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax

		Public Shadows ReadOnly Property Identifier As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax)._identifier, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property TypeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax)(Me._typeArgumentList, 1)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(kind, errors, annotations, identifier, DirectCast(typeArgumentList.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitGenericName(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitGenericName(Me)
		End Sub

		Public Function AddTypeArgumentListArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax
			Return Me.WithTypeArgumentList(If(Me.TypeArgumentList IsNot Nothing, Me.TypeArgumentList, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TypeArgumentList(New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax(-1) {})).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._typeArgumentList
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetIdentifierCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return Me.Identifier
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim typeArgumentList As SyntaxNode
			If (i <> 1) Then
				typeArgumentList = Nothing
			Else
				typeArgumentList = Me.TypeArgumentList
			End If
			Return typeArgumentList
		End Function

		Public Function Update(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax
			Dim genericNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax
			If (identifier <> Me.Identifier OrElse typeArgumentList <> Me.TypeArgumentList) Then
				Dim genericNameSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.GenericName(identifier, typeArgumentList)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				genericNameSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, genericNameSyntax1, genericNameSyntax1.WithAnnotations(annotations))
			Else
				genericNameSyntax = Me
			End If
			Return genericNameSyntax
		End Function

		Public Shadows Function WithIdentifier(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax
			Return Me.Update(identifier, Me.TypeArgumentList)
		End Function

		Friend Overrides Function WithIdentifierCore(ByVal identifier As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleNameSyntax
			Return Me.WithIdentifier(identifier)
		End Function

		Public Function WithTypeArgumentList(ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax
			Return Me.Update(Me.Identifier, typeArgumentList)
		End Function
	End Class
End Namespace