Imports Microsoft.CodeAnalysis
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class PseudoVariableExpressions
		Protected Sub New()
			MyBase.New()
		End Sub

		Friend MustOverride Function GetAddress(ByVal variable As BoundPseudoVariable) As BoundExpression

		Friend MustOverride Function GetValue(ByVal variable As BoundPseudoVariable, ByVal diagnostics As DiagnosticBag) As BoundExpression
	End Class
End Namespace