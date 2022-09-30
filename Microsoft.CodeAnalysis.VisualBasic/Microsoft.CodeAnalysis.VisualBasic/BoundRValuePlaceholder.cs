using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRValuePlaceholder : BoundRValuePlaceholderBase
	{
		public BoundRValuePlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.RValuePlaceholder, syntax, type, hasErrors)
		{
		}

		public BoundRValuePlaceholder(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.RValuePlaceholder, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRValuePlaceholder(this);
		}

		public BoundRValuePlaceholder Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundRValuePlaceholder boundRValuePlaceholder = new BoundRValuePlaceholder(base.Syntax, type, base.HasErrors);
				boundRValuePlaceholder.CopyAttributes(this);
				return boundRValuePlaceholder;
			}
			return this;
		}
	}
}
