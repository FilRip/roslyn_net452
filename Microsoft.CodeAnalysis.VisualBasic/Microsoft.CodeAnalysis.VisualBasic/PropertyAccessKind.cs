using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum PropertyAccessKind
	{
		Unknown = 0,
		Get = 1,
		Set = 2
	}
}
