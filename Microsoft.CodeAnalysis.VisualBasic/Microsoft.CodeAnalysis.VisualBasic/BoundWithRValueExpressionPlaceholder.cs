using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundWithRValueExpressionPlaceholder : BoundRValuePlaceholderBase
	{
		public BoundWithRValueExpressionPlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.WithRValueExpressionPlaceholder, syntax, type, hasErrors)
		{
		}

		public BoundWithRValueExpressionPlaceholder(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.WithRValueExpressionPlaceholder, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitWithRValueExpressionPlaceholder(this);
		}

		public BoundWithRValueExpressionPlaceholder Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundWithRValueExpressionPlaceholder boundWithRValueExpressionPlaceholder = new BoundWithRValueExpressionPlaceholder(base.Syntax, type, base.HasErrors);
				boundWithRValueExpressionPlaceholder.CopyAttributes(this);
				return boundWithRValueExpressionPlaceholder;
			}
			return this;
		}
	}
}
