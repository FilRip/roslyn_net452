Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class NameSyntax
		Inherits TypeSyntax
		Public ReadOnly Property Arity As Integer
			Get
				Dim num As Integer
				num = If(Not TypeOf Me Is GenericNameSyntax, 0, DirectCast(Me, GenericNameSyntax).TypeArgumentList.Arguments.Count)
				Return num
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub
	End Class
End Namespace