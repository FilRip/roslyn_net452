using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundExitStatement : BoundStatement
	{
		private readonly LabelSymbol _Label;

		public LabelSymbol Label => _Label;

		public BoundExitStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
			: base(BoundKind.ExitStatement, syntax, hasErrors)
		{
			_Label = label;
		}

		public BoundExitStatement(SyntaxNode syntax, LabelSymbol label)
			: base(BoundKind.ExitStatement, syntax)
		{
			_Label = label;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitExitStatement(this);
		}

		public BoundExitStatement Update(LabelSymbol label)
		{
			if ((object)label != Label)
			{
				BoundExitStatement boundExitStatement = new BoundExitStatement(base.Syntax, label, base.HasErrors);
				boundExitStatement.CopyAttributes(this);
				return boundExitStatement;
			}
			return this;
		}
	}
}
