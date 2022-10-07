Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class IgnoreAccessibilityBinder
		Inherits Binder
		Public Sub New(ByVal containingBinder As Binder)
			MyBase.New(containingBinder)
		End Sub

		Friend Overrides Function BinderSpecificLookupOptions(ByVal options As LookupOptions) As LookupOptions
			Return MyBase.ContainingBinder.BinderSpecificLookupOptions(options) Or LookupOptions.IgnoreAccessibility
		End Function

		Public Overrides Function CheckAccessibility(ByVal sym As Symbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal accessThroughType As TypeSymbol = Nothing, Optional ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = Nothing) As AccessCheckResult
			Return AccessCheckResult.Accessible
		End Function
	End Class
End Namespace