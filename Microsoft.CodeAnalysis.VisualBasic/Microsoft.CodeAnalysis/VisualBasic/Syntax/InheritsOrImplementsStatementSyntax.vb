Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class InheritsOrImplementsStatementSyntax
		Inherits DeclarationStatementSyntax
		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub
	End Class
End Namespace