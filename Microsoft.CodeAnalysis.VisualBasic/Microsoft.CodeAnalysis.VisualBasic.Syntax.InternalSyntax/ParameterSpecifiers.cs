using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[Flags]
	internal enum ParameterSpecifiers
	{
		ByRef = 1,
		ByVal = 2,
		Optional = 4,
		ParamArray = 8
	}
}
