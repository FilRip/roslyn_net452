Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class CaseClauseSyntax
		Inherits VisualBasicSyntaxNode
		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub
	End Class
End Namespace