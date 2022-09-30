using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum AttributeLocation
	{
		None = 0,
		Assembly = 1,
		Module = 2,
		Type = 4,
		Method = 8,
		Field = 0x10,
		Property = 0x20,
		Event = 0x40,
		Parameter = 0x80,
		Return = 0x100,
		TypeParameter = 0x200
	}
}
