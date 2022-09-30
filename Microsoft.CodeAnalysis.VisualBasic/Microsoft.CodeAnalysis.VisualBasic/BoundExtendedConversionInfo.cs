namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundExtendedConversionInfo : BoundNode
	{
		protected BoundExtendedConversionInfo(BoundKind kind, SyntaxNode syntax, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
		}

		protected BoundExtendedConversionInfo(BoundKind kind, SyntaxNode syntax)
			: base(kind, syntax)
		{
		}
	}
}
