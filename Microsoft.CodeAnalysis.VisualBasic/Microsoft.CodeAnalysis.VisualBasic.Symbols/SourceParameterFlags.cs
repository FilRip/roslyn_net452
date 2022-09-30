namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal enum SourceParameterFlags : byte
	{
		ByVal = 1,
		ByRef = 2,
		Optional = 4,
		ParamArray = 8
	}
}
