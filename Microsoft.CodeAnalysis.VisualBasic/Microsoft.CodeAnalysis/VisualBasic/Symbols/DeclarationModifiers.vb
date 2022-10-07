Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	<Flags>
	Friend Enum DeclarationModifiers
		None = 0
		[Private] = 1
		[Protected] = 2
		[Friend] = 4
		[Public] = 8
		AllAccessibilityModifiers = 15
		[Shared] = 16
		InvalidInInterface = 31
		[ReadOnly] = 32
		[WriteOnly] = 64
		AllWriteabilityModifiers = 96
		[Overrides] = 128
		[Overridable] = 256
		[MustOverride] = 512
		[NotOverridable] = 1024
		AllOverrideModifiers = 1792
		[Overloads] = 2048
		[Shadows] = 4096
		AllShadowingModifiers = 6144
		[Default] = 8192
		InvalidInNotInheritableClass = 9984
		InvalidInModule = 13202
		[WithEvents] = 16384
		[Widening] = 32768
		[Narrowing] = 65536
		AllConversionModifiers = 98304
		[Partial] = 131072
		[MustInherit] = 262144
		[NotInheritable] = 524288
		Async = 1048576
		Iterator = 2097152
		[Dim] = 4194304
		[Const] = 8388608
		[Static] = 16777216
	End Enum
End Namespace