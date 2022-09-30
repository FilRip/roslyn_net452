using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMyClassReference : BoundExpression
	{
		public sealed override bool SuppressVirtualCalls => true;

		public BoundMyClassReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.MyClassReference, syntax, type, hasErrors)
		{
		}

		public BoundMyClassReference(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.MyClassReference, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMyClassReference(this);
		}

		public BoundMyClassReference Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundMyClassReference boundMyClassReference = new BoundMyClassReference(base.Syntax, type, base.HasErrors);
				boundMyClassReference.CopyAttributes(this);
				return boundMyClassReference;
			}
			return this;
		}
	}
}
