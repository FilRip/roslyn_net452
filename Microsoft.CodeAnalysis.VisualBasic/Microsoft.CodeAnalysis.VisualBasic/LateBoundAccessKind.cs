using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum LateBoundAccessKind
	{
		Unknown = 0,
		Get = 1,
		Set = 2,
		Call = 4
	}
}
