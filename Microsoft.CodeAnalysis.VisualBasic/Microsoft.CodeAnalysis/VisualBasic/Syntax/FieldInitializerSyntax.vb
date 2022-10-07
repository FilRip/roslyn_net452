Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class FieldInitializerSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property KeyKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetKeyKeywordCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetKeyKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
			Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)._keyKeyword
			syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, MyBase.Position, 0))
			Return syntaxToken
		End Function

		Public Function WithKeyKeyword(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
			Return Me.WithKeyKeywordCore(keyKeyword)
		End Function

		Friend MustOverride Function WithKeyKeywordCore(ByVal keyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FieldInitializerSyntax
	End Class
End Namespace