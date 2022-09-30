using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundLValuePlaceholderBase : BoundValuePlaceholderBase
	{
		public sealed override bool IsLValue => true;

		protected sealed override BoundExpression MakeRValueImpl()
		{
			return BoundNodeExtensions.MakeCompilerGenerated(new BoundLValueToRValueWrapper(base.Syntax, this, base.Type));
		}

		protected BoundLValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, type, hasErrors)
		{
		}

		protected BoundLValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol type)
			: base(kind, syntax, type)
		{
		}
	}
}
