using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLabelStatement : BoundStatement
	{
		private readonly LabelSymbol _Label;

		public LabelSymbol Label => _Label;

		public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
			: base(BoundKind.LabelStatement, syntax, hasErrors)
		{
			_Label = label;
		}

		public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label)
			: base(BoundKind.LabelStatement, syntax)
		{
			_Label = label;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLabelStatement(this);
		}

		public BoundLabelStatement Update(LabelSymbol label)
		{
			if ((object)label != Label)
			{
				BoundLabelStatement boundLabelStatement = new BoundLabelStatement(base.Syntax, label, base.HasErrors);
				boundLabelStatement.CopyAttributes(this);
				return boundLabelStatement;
			}
			return this;
		}
	}
}
