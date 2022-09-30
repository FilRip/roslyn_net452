using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLValuePlaceholder : BoundLValuePlaceholderBase
	{
		public BoundLValuePlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.LValuePlaceholder, syntax, type, hasErrors)
		{
		}

		public BoundLValuePlaceholder(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.LValuePlaceholder, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLValuePlaceholder(this);
		}

		public BoundLValuePlaceholder Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundLValuePlaceholder boundLValuePlaceholder = new BoundLValuePlaceholder(base.Syntax, type, base.HasErrors);
				boundLValuePlaceholder.CopyAttributes(this);
				return boundLValuePlaceholder;
			}
			return this;
		}
	}
}
