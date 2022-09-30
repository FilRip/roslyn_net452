using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundForEachStatement : BoundForStatement
	{
		private readonly BoundExpression _Collection;

		private readonly ForEachEnumeratorInfo _EnumeratorInfo;

		public BoundExpression Collection => _Collection;

		public ForEachEnumeratorInfo EnumeratorInfo => _EnumeratorInfo;

		public BoundForEachStatement(SyntaxNode syntax, BoundExpression collection, ForEachEnumeratorInfo enumeratorInfo, LocalSymbol declaredOrInferredLocalOpt, BoundExpression controlVariable, BoundStatement body, ImmutableArray<BoundExpression> nextVariablesOpt, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors = false)
			: base(BoundKind.ForEachStatement, syntax, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(collection) || BoundNodeExtensions.NonNullAndHasErrors(controlVariable) || BoundNodeExtensions.NonNullAndHasErrors(body) || BoundNodeExtensions.NonNullAndHasErrors(nextVariablesOpt))
		{
			_Collection = collection;
			_EnumeratorInfo = enumeratorInfo;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitForEachStatement(this);
		}

		public BoundForEachStatement Update(BoundExpression collection, ForEachEnumeratorInfo enumeratorInfo, LocalSymbol declaredOrInferredLocalOpt, BoundExpression controlVariable, BoundStatement body, ImmutableArray<BoundExpression> nextVariablesOpt, LabelSymbol continueLabel, LabelSymbol exitLabel)
		{
			if (collection != Collection || enumeratorInfo != EnumeratorInfo || (object)declaredOrInferredLocalOpt != base.DeclaredOrInferredLocalOpt || controlVariable != base.ControlVariable || body != base.Body || nextVariablesOpt != base.NextVariablesOpt || (object)continueLabel != base.ContinueLabel || (object)exitLabel != base.ExitLabel)
			{
				BoundForEachStatement boundForEachStatement = new BoundForEachStatement(base.Syntax, collection, enumeratorInfo, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, base.HasErrors);
				boundForEachStatement.CopyAttributes(this);
				return boundForEachStatement;
			}
			return this;
		}
	}
}
