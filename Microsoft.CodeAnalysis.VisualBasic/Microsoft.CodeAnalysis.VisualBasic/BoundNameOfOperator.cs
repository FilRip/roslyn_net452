using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNameOfOperator : BoundExpression
	{
		private readonly BoundExpression _Argument;

		private readonly ConstantValue _ConstantValueOpt;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Argument);

		public BoundExpression Argument => _Argument;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundNameOfOperator(SyntaxNode syntax, BoundExpression argument, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.NameOfOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(argument))
		{
			_Argument = argument;
			_ConstantValueOpt = constantValueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNameOfOperator(this);
		}

		public BoundNameOfOperator Update(BoundExpression argument, ConstantValue constantValueOpt, TypeSymbol type)
		{
			if (argument != Argument || (object)constantValueOpt != ConstantValueOpt || (object)type != base.Type)
			{
				BoundNameOfOperator boundNameOfOperator = new BoundNameOfOperator(base.Syntax, argument, constantValueOpt, type, base.HasErrors);
				boundNameOfOperator.CopyAttributes(this);
				return boundNameOfOperator;
			}
			return this;
		}
	}
}
