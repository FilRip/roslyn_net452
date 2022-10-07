Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SemanticModelBinder
		Inherits Binder
		Private ReadOnly _ignoreAccessibility As Boolean

		Public NotOverridable Overrides ReadOnly Property IsSemanticModelBinder As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Sub New(ByVal containingBinder As Binder, Optional ByVal ignoreAccessibility As Boolean = False)
			MyBase.New(containingBinder)
			Me._ignoreAccessibility = False
			Me._ignoreAccessibility = ignoreAccessibility
		End Sub

		Friend Overrides Function BinderSpecificLookupOptions(ByVal options As LookupOptions) As LookupOptions
			Dim lookupOption As LookupOptions
			lookupOption = If(Not Me._ignoreAccessibility, MyBase.BinderSpecificLookupOptions(options), MyBase.BinderSpecificLookupOptions(options) Or LookupOptions.IgnoreAccessibility)
			Return lookupOption
		End Function

		Public Shared Function Mark(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal ignoreAccessibility As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Binder
			If (binder.IsSemanticModelBinder AndAlso binder.IgnoresAccessibility = ignoreAccessibility) Then
				Return binder
			End If
			Return New SemanticModelBinder(binder, ignoreAccessibility)
		End Function
	End Class
End Namespace