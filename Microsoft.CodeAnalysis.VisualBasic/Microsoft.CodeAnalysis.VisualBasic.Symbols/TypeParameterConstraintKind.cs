using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[Flags]
	internal enum TypeParameterConstraintKind
	{
		None = 0,
		ReferenceType = 1,
		ValueType = 2,
		Constructor = 4
	}
}
