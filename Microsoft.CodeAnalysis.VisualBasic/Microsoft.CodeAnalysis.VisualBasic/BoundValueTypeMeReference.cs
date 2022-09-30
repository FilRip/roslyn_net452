using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundValueTypeMeReference : BoundExpression
	{
		public override bool IsLValue => true;

		public BoundValueTypeMeReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.ValueTypeMeReference, syntax, type, hasErrors)
		{
		}

		public BoundValueTypeMeReference(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.ValueTypeMeReference, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitValueTypeMeReference(this);
		}

		public BoundValueTypeMeReference Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundValueTypeMeReference boundValueTypeMeReference = new BoundValueTypeMeReference(base.Syntax, type, base.HasErrors);
				boundValueTypeMeReference.CopyAttributes(this);
				return boundValueTypeMeReference;
			}
			return this;
		}
	}
}
