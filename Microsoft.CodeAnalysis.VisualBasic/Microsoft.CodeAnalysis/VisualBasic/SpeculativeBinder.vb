Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SpeculativeBinder
		Inherits SemanticModelBinder
		Private Sub New(ByVal containingBinder As Binder)
			MyBase.New(containingBinder, False)
		End Sub

		Friend Overrides Function BindFunctionAggregationExpression(ByVal [function] As FunctionAggregationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return MyBase.ContainingBinder.BindFunctionAggregationExpression([function], diagnostics)
		End Function

		Friend Overrides Function BindGroupAggregationExpression(ByVal group As GroupAggregationSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return MyBase.ContainingBinder.BindGroupAggregationExpression(group, diagnostics)
		End Function

		Public Shared Function Create(ByVal containingBinder As Binder) As SpeculativeBinder
			If (containingBinder.ImplicitVariableDeclarationAllowed) Then
				containingBinder = New ImplicitVariableBinder(containingBinder, containingBinder.ContainingMember)
			End If
			Return New SpeculativeBinder(containingBinder)
		End Function

		Public Overrides Function GetSyntaxReference(ByVal node As VisualBasicSyntaxNode) As SyntaxReference
			Throw New NotSupportedException()
		End Function
	End Class
End Namespace