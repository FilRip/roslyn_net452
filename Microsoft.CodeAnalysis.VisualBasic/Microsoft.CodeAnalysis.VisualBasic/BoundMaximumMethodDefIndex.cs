using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMaximumMethodDefIndex : BoundExpression
	{
		public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MaximumMethodDefIndex, syntax, type, hasErrors)
		{
		}

		public BoundMaximumMethodDefIndex(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.MaximumMethodDefIndex, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMaximumMethodDefIndex(this);
		}

		public BoundMaximumMethodDefIndex Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundMaximumMethodDefIndex boundMaximumMethodDefIndex = new BoundMaximumMethodDefIndex(base.Syntax, type, base.HasErrors);
				boundMaximumMethodDefIndex.CopyAttributes(this);
				return boundMaximumMethodDefIndex;
			}
			return this;
		}
	}
}
