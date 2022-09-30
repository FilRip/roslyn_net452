using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundMidResult : BoundExpression
	{
		private readonly BoundExpression _Original;

		private readonly BoundExpression _Start;

		private readonly BoundExpression _LengthOpt;

		private readonly BoundExpression _Source;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Original, (BoundNode)Start, (BoundNode)LengthOpt, (BoundNode)Source);

		public BoundExpression Original => _Original;

		public BoundExpression Start => _Start;

		public BoundExpression LengthOpt => _LengthOpt;

		public BoundExpression Source => _Source;

		public BoundMidResult(SyntaxNode syntax, BoundExpression original, BoundExpression start, BoundExpression lengthOpt, BoundExpression source, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.MidResult, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(original) || BoundNodeExtensions.NonNullAndHasErrors(start) || BoundNodeExtensions.NonNullAndHasErrors(lengthOpt) || BoundNodeExtensions.NonNullAndHasErrors(source))
		{
			_Original = original;
			_Start = start;
			_LengthOpt = lengthOpt;
			_Source = source;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitMidResult(this);
		}

		public BoundMidResult Update(BoundExpression original, BoundExpression start, BoundExpression lengthOpt, BoundExpression source, TypeSymbol type)
		{
			if (original != Original || start != Start || lengthOpt != LengthOpt || source != Source || (object)type != base.Type)
			{
				BoundMidResult boundMidResult = new BoundMidResult(base.Syntax, original, start, lengthOpt, source, type, base.HasErrors);
				boundMidResult.CopyAttributes(this);
				return boundMidResult;
			}
			return this;
		}
	}
}
