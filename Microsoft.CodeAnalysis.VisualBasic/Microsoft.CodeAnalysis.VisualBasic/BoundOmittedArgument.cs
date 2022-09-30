using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundOmittedArgument : BoundExpression
	{
		public BoundOmittedArgument(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.OmittedArgument, syntax, type, hasErrors)
		{
		}

		public BoundOmittedArgument(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.OmittedArgument, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitOmittedArgument(this);
		}

		public BoundOmittedArgument Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundOmittedArgument boundOmittedArgument = new BoundOmittedArgument(base.Syntax, type, base.HasErrors);
				boundOmittedArgument.CopyAttributes(this);
				return boundOmittedArgument;
			}
			return this;
		}
	}
}
