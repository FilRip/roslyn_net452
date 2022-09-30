using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum DirectConstraintConflictKind
	{
		None = 0,
		DuplicateTypeConstraint = 1,
		RedundantConstraint = 2,
		All = 3
	}
}
