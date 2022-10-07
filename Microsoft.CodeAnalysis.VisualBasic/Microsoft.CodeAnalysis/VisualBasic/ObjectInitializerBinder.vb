Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ObjectInitializerBinder
		Inherits Binder
		Private ReadOnly _receiver As BoundExpression

		Public Sub New(ByVal containingBinder As Binder, ByVal receiver As BoundExpression)
			MyBase.New(containingBinder)
			Me._receiver = receiver
		End Sub

		Protected Overrides Function TryBindOmittedLeftForConditionalAccess(ByVal node As ConditionalAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return Nothing
		End Function

		Protected Overrides Function TryBindOmittedLeftForDictionaryAccess(ByVal node As MemberAccessExpressionSyntax, ByVal accessingBinder As Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Return Me._receiver
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForMemberAccess(ByVal node As MemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder, ByRef wholeMemberAccessExpressionBound As Boolean) As BoundExpression
			Return Me._receiver
		End Function

		Protected Friend Overrides Function TryBindOmittedLeftForXmlMemberAccess(ByVal node As XmlMemberAccessExpressionSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal accessingBinder As Binder) As BoundExpression
			Return Me._receiver
		End Function
	End Class
End Namespace