using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTupleLiteral : BoundTupleExpression
	{
		private readonly TupleTypeSymbol _InferredType;

		private readonly ImmutableArray<string> _ArgumentNamesOpt;

		private readonly ImmutableArray<bool> _InferredNamesOpt;

		public TupleTypeSymbol InferredType => _InferredType;

		public ImmutableArray<string> ArgumentNamesOpt => _ArgumentNamesOpt;

		public ImmutableArray<bool> InferredNamesOpt => _InferredNamesOpt;

		public BoundTupleLiteral(SyntaxNode syntax, TupleTypeSymbol inferredType, ImmutableArray<string> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TupleLiteral, syntax, arguments, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(arguments))
		{
			_InferredType = inferredType;
			_ArgumentNamesOpt = argumentNamesOpt;
			_InferredNamesOpt = inferredNamesOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTupleLiteral(this);
		}

		public BoundTupleLiteral Update(TupleTypeSymbol inferredType, ImmutableArray<string> argumentNamesOpt, ImmutableArray<bool> inferredNamesOpt, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
		{
			if ((object)inferredType != InferredType || argumentNamesOpt != ArgumentNamesOpt || inferredNamesOpt != InferredNamesOpt || arguments != base.Arguments || (object)type != base.Type)
			{
				BoundTupleLiteral boundTupleLiteral = new BoundTupleLiteral(base.Syntax, inferredType, argumentNamesOpt, inferredNamesOpt, arguments, type, base.HasErrors);
				boundTupleLiteral.CopyAttributes(this);
				return boundTupleLiteral;
			}
			return this;
		}
	}
}
