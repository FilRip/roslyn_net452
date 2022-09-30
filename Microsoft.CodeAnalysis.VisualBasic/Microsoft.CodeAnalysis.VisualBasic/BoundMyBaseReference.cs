using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMyBaseReference : BoundExpression
	{
		public sealed override bool SuppressVirtualCalls => true;

		public BoundMyBaseReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MyBaseReference, syntax, type, hasErrors)
		{
		}

		public BoundMyBaseReference(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.MyBaseReference, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMyBaseReference(this);
		}

		public BoundMyBaseReference Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundMyBaseReference boundMyBaseReference = new BoundMyBaseReference(base.Syntax, type, base.HasErrors);
				boundMyBaseReference.CopyAttributes(this);
				return boundMyBaseReference;
			}
			return this;
		}
	}
}
