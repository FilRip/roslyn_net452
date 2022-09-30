using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundContinueStatement : BoundStatement
	{
		private readonly LabelSymbol _Label;

		public LabelSymbol Label => _Label;

		public BoundContinueStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
			: base(BoundKind.ContinueStatement, syntax, hasErrors)
		{
			_Label = label;
		}

		public BoundContinueStatement(SyntaxNode syntax, LabelSymbol label)
			: base(BoundKind.ContinueStatement, syntax)
		{
			_Label = label;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitContinueStatement(this);
		}

		public BoundContinueStatement Update(LabelSymbol label)
		{
			if ((object)label != Label)
			{
				BoundContinueStatement boundContinueStatement = new BoundContinueStatement(base.Syntax, label, base.HasErrors);
				boundContinueStatement.CopyAttributes(this);
				return boundContinueStatement;
			}
			return this;
		}
	}
}
