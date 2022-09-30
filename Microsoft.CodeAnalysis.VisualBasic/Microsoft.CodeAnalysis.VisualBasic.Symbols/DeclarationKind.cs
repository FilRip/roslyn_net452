namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal enum DeclarationKind : byte
	{
		Namespace,
		Class,
		Interface,
		Structure,
		Enum,
		Delegate,
		Module,
		Script,
		Submission,
		ImplicitClass,
		EventSyntheticDelegate
	}
}
