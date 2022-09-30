using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum QuickAttributes : byte
	{
		None = 0,
		Extension = 1,
		Obsolete = 2,
		MyGroupCollection = 4,
		TypeIdentifier = 8
	}
}
