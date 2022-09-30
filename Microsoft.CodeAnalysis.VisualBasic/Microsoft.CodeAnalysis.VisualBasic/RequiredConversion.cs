namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal enum RequiredConversion
	{
		Identity,
		Any,
		AnyReverse,
		AnyAndReverse,
		ArrayElement,
		Reference,
		ReverseReference,
		None,
		Count
	}
}
