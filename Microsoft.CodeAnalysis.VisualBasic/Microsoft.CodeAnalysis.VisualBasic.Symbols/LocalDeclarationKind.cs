namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal enum LocalDeclarationKind : byte
	{
		None,
		Variable,
		ImplicitVariable,
		Constant,
		Static,
		Using,
		Catch,
		For,
		ForEach,
		FunctionValue,
		AmbiguousLocals
	}
}
