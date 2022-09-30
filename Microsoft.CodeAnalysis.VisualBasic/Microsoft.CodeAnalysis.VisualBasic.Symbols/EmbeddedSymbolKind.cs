using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum EmbeddedSymbolKind : byte
	{
		None = 0,
		Unset = 1,
		EmbeddedAttribute = 2,
		VbCore = 4,
		XmlHelper = 8,
		All = 0xE,
		LastValue = 8
	}
}
