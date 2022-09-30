using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundForStatement : BoundLoopStatement
	{
		private readonly LocalSymbol _DeclaredOrInferredLocalOpt;

		private readonly BoundExpression _ControlVariable;

		private readonly BoundStatement _Body;

		private readonly ImmutableArray<BoundExpression> _NextVariablesOpt;

		public LocalSymbol DeclaredOrInferredLocalOpt => _DeclaredOrInferredLocalOpt;

		public BoundExpression ControlVariable => _ControlVariable;

		public BoundStatement Body => _Body;

		public ImmutableArray<BoundExpression> NextVariablesOpt => _NextVariablesOpt;

		protected BoundForStatement(BoundKind kind, SyntaxNode syntax, LocalSymbol declaredOrInferredLocalOpt, BoundExpression controlVariable, BoundStatement body, ImmutableArray<BoundExpression> nextVariablesOpt, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors = false)
			: base(kind, syntax, continueLabel, exitLabel, hasErrors)
		{
			_DeclaredOrInferredLocalOpt = declaredOrInferredLocalOpt;
			_ControlVariable = controlVariable;
			_Body = body;
			_NextVariablesOpt = nextVariablesOpt;
		}
	}
}
