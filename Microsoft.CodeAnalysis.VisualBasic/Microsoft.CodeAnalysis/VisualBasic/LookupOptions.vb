Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	<Flags>
	Friend Enum LookupOptions
		[Default] = 0
		NamespacesOrTypesOnly = 2
		LabelsOnly = 4
		MustBeInstance = 8
		MustNotBeInstance = 16
		MustNotBeReturnValueVariable = 32
		NoBaseClassLookup = 64
		IgnoreAccessibility = 128
		AllMethodsOfAnyArity = 256
		IgnoreExtensionMethods = 512
		EagerlyLookupExtensionMethods = 1024
		MethodsOnly = 2048
		UseBaseReferenceAccessibility = 4096
		AttributeTypeOnly = 8194
		MustNotBeLocalOrParameter = 16384
		EventsOnly = 32768
		NoSystemObjectLookupForInterfaces = 65536
		AllowIntrinsicAliases = 131072
	End Enum
End Namespace