using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundWithLValueExpressionPlaceholder : BoundLValuePlaceholderBase
	{
		public BoundWithLValueExpressionPlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.WithLValueExpressionPlaceholder, syntax, type, hasErrors)
		{
		}

		public BoundWithLValueExpressionPlaceholder(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.WithLValueExpressionPlaceholder, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitWithLValueExpressionPlaceholder(this);
		}

		public BoundWithLValueExpressionPlaceholder Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundWithLValueExpressionPlaceholder boundWithLValueExpressionPlaceholder = new BoundWithLValueExpressionPlaceholder(base.Syntax, type, base.HasErrors);
				boundWithLValueExpressionPlaceholder.CopyAttributes(this);
				return boundWithLValueExpressionPlaceholder;
			}
			return this;
		}
	}
}
