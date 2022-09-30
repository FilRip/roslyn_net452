using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMeReference : BoundExpression
	{
		public BoundMeReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MeReference, syntax, type, hasErrors)
		{
		}

		public BoundMeReference(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.MeReference, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMeReference(this);
		}

		public BoundMeReference Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundMeReference boundMeReference = new BoundMeReference(base.Syntax, type, base.HasErrors);
				boundMeReference.CopyAttributes(this);
				return boundMeReference;
			}
			return this;
		}
	}
}
