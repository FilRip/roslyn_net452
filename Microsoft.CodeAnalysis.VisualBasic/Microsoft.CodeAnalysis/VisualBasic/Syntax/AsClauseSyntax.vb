Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class AsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Public ReadOnly Property AsKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetAsKeywordCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetAsKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)._asKeyword, MyBase.Position, 0)
		End Function

		Public Function WithAsKeyword(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
			Return Me.WithAsKeywordCore(asKeyword)
		End Function

		Friend MustOverride Function WithAsKeywordCore(ByVal asKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax
	End Class
End Namespace