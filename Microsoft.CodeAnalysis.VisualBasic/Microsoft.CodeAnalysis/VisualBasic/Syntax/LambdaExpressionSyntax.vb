Imports Microsoft.CodeAnalysis
Imports System
Imports System.ComponentModel

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public MustInherit Class LambdaExpressionSyntax
		Inherits ExpressionSyntax
		Friend _subOrFunctionHeader As LambdaHeaderSyntax

		<EditorBrowsable(EditorBrowsableState.Never)>
		<Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", True)>
		Public ReadOnly Property Begin As LambdaHeaderSyntax
			Get
				Return Me.SubOrFunctionHeader
			End Get
		End Property

		Public ReadOnly Property SubOrFunctionHeader As LambdaHeaderSyntax
			Get
				Return Me.GetSubOrFunctionHeaderCore()
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Overridable Function GetSubOrFunctionHeaderCore() As LambdaHeaderSyntax
			Return MyBase.GetRedAtZero(Of LambdaHeaderSyntax)(Me._subOrFunctionHeader)
		End Function

		Public Function WithSubOrFunctionHeader(ByVal subOrFunctionHeader As LambdaHeaderSyntax) As LambdaExpressionSyntax
			Return Me.WithSubOrFunctionHeaderCore(subOrFunctionHeader)
		End Function

		Friend MustOverride Function WithSubOrFunctionHeaderCore(ByVal subOrFunctionHeader As LambdaHeaderSyntax) As LambdaExpressionSyntax
	End Class
End Namespace