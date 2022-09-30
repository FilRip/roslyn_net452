using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum DeclarationModifiers
	{
		None = 0,
		Private = 1,
		Protected = 2,
		Friend = 4,
		Public = 8,
		AllAccessibilityModifiers = 0xF,
		Shared = 0x10,
		ReadOnly = 0x20,
		WriteOnly = 0x40,
		AllWriteabilityModifiers = 0x60,
		Overrides = 0x80,
		Overridable = 0x100,
		MustOverride = 0x200,
		NotOverridable = 0x400,
		AllOverrideModifiers = 0x700,
		Overloads = 0x800,
		Shadows = 0x1000,
		AllShadowingModifiers = 0x1800,
		Default = 0x2000,
		WithEvents = 0x4000,
		Widening = 0x8000,
		Narrowing = 0x10000,
		AllConversionModifiers = 0x18000,
		Partial = 0x20000,
		MustInherit = 0x40000,
		NotInheritable = 0x80000,
		Async = 0x100000,
		Iterator = 0x200000,
		Dim = 0x400000,
		Const = 0x800000,
		Static = 0x1000000,
		InvalidInNotInheritableClass = 0x2700,
		InvalidInModule = 0x3392,
		InvalidInInterface = 0x1F
	}
}
