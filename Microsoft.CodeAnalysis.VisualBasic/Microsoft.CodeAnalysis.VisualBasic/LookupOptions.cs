using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum LookupOptions
	{
		Default = 0,
		NamespacesOrTypesOnly = 2,
		LabelsOnly = 4,
		MustBeInstance = 8,
		MustNotBeInstance = 0x10,
		MustNotBeReturnValueVariable = 0x20,
		NoBaseClassLookup = 0x40,
		IgnoreAccessibility = 0x80,
		AllMethodsOfAnyArity = 0x100,
		IgnoreExtensionMethods = 0x200,
		EagerlyLookupExtensionMethods = 0x400,
		MethodsOnly = 0x800,
		UseBaseReferenceAccessibility = 0x1000,
		AttributeTypeOnly = 0x2002,
		MustNotBeLocalOrParameter = 0x4000,
		EventsOnly = 0x8000,
		NoSystemObjectLookupForInterfaces = 0x10000,
		AllowIntrinsicAliases = 0x20000
	}
}
