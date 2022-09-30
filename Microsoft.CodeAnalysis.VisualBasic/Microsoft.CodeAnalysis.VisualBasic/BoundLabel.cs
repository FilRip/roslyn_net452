using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLabel : BoundExpression
	{
		private readonly LabelSymbol _Label;

		public override Symbol ExpressionSymbol => Label;

		public LabelSymbol Label => _Label;

		public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol type, bool hasErrors)
			: base(BoundKind.Label, syntax, type, hasErrors)
		{
			_Label = label;
		}

		public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol type)
			: base(BoundKind.Label, syntax, type)
		{
			_Label = label;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLabel(this);
		}

		public BoundLabel Update(LabelSymbol label, TypeSymbol type)
		{
			if ((object)label != Label || (object)type != base.Type)
			{
				BoundLabel boundLabel = new BoundLabel(base.Syntax, label, type, base.HasErrors);
				boundLabel.CopyAttributes(this);
				return boundLabel;
			}
			return this;
		}
	}
}
