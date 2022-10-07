Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class ForOrForEachStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode

		Public ReadOnly Property ControlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Get
				Return Me.GetControlVariableCore()
			End Get
		End Property

		Public ReadOnly Property ForKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return Me.GetForKeywordCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetControlVariableCore() As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(Me._controlVariable, 1)
		End Function

		Friend Overridable Function GetForKeywordCore() As Microsoft.CodeAnalysis.SyntaxToken
			Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax)._forKeyword, MyBase.Position, 0)
		End Function

		Public Function WithControlVariable(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithControlVariableCore(controlVariable)
		End Function

		Friend MustOverride Function WithControlVariableCore(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax

		Public Function WithForKeyword(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
			Return Me.WithForKeywordCore(forKeyword)
		End Function

		Friend MustOverride Function WithForKeywordCore(ByVal forKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForOrForEachStatementSyntax
	End Class
End Namespace