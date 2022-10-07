Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module LookupOptionExtensions
		Friend Const QueryOperatorLookupOptions As LookupOptions = LookupOptions.MustBeInstance Or LookupOptions.AllMethodsOfAnyArity Or LookupOptions.MethodsOnly

		Friend Const ConsiderationMask As LookupOptions = LookupOptions.NamespacesOrTypesOnly Or LookupOptions.LabelsOnly

		<Extension>
		Friend Function IsAttributeTypeLookup(ByVal options As LookupOptions) As Boolean
			Return (options And LookupOptions.AttributeTypeOnly) = LookupOptions.AttributeTypeOnly
		End Function

		<Extension>
		Friend Function IsValid(ByVal options As LookupOptions) As Boolean
			Dim lookupOption As LookupOptions = LookupOptions.MustBeInstance Or LookupOptions.MustNotBeInstance
			Return If((options And lookupOption) <> lookupOption, True, False)
		End Function

		<Extension>
		Friend Function ShouldLookupExtensionMethods(ByVal options As LookupOptions) As Boolean
			Return (options And (LookupOptions.NamespacesOrTypesOnly Or LookupOptions.MustNotBeInstance Or LookupOptions.IgnoreExtensionMethods Or LookupOptions.AttributeTypeOnly)) = LookupOptions.[Default]
		End Function

		<Extension>
		Friend Sub ThrowIfInvalid(ByVal options As LookupOptions)
			If (Not options.IsValid()) Then
				Throw New ArgumentException("LookupOptions has an invalid combination of options")
			End If
		End Sub
	End Module
End Namespace