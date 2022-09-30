using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundConversionOrCast : BoundExpression
	{
		public abstract BoundExpression Operand { get; }

		public abstract ConversionKind ConversionKind { get; }

		public abstract bool ExplicitCastInCode { get; }

		protected BoundConversionOrCast(BoundKind kind, SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(kind, syntax, type, hasErrors)
		{
		}

		protected BoundConversionOrCast(BoundKind kind, SyntaxNode syntax, TypeSymbol type)
			: base(kind, syntax, type)
		{
		}
	}
}
