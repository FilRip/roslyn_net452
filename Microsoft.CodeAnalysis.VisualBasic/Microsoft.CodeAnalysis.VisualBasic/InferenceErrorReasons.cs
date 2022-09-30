using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[Flags]
	internal enum InferenceErrorReasons : byte
	{
		Other = 0,
		Ambiguous = 1,
		NoBest = 2
	}
}
