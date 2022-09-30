using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRedimClause : BoundStatement
	{
		private readonly BoundExpression _Operand;

		private readonly ImmutableArray<BoundExpression> _Indices;

		private readonly ArrayTypeSymbol _ArrayTypeOpt;

		private readonly bool _Preserve;

		public BoundExpression Operand => _Operand;

		public ImmutableArray<BoundExpression> Indices => _Indices;

		public ArrayTypeSymbol ArrayTypeOpt => _ArrayTypeOpt;

		public bool Preserve => _Preserve;

		public BoundRedimClause(SyntaxNode syntax, BoundExpression operand, ImmutableArray<BoundExpression> indices, ArrayTypeSymbol arrayTypeOpt, bool preserve, bool hasErrors = false)
			: base(BoundKind.RedimClause, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand) || BoundNodeExtensions.NonNullAndHasErrors(indices))
		{
			_Operand = operand;
			_Indices = indices;
			_ArrayTypeOpt = arrayTypeOpt;
			_Preserve = preserve;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRedimClause(this);
		}

		public BoundRedimClause Update(BoundExpression operand, ImmutableArray<BoundExpression> indices, ArrayTypeSymbol arrayTypeOpt, bool preserve)
		{
			if (operand != Operand || indices != Indices || (object)arrayTypeOpt != ArrayTypeOpt || preserve != Preserve)
			{
				BoundRedimClause boundRedimClause = new BoundRedimClause(base.Syntax, operand, indices, arrayTypeOpt, preserve, base.HasErrors);
				boundRedimClause.CopyAttributes(this);
				return boundRedimClause;
			}
			return this;
		}
	}
}
