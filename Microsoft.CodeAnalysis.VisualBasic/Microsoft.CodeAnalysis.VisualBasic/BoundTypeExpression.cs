using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTypeExpression : BoundExpression
	{
		private readonly BoundExpression _UnevaluatedReceiverOpt;

		private readonly AliasSymbol _AliasOpt;

		public override Symbol ExpressionSymbol => (Symbol)(((object)AliasOpt) ?? ((object)base.Type));

		public BoundExpression UnevaluatedReceiverOpt => _UnevaluatedReceiverOpt;

		public AliasSymbol AliasOpt => _AliasOpt;

		public BoundTypeExpression(SyntaxNode syntax, TypeSymbol type, bool hasErrors = false)
			: this(syntax, null, null, type, hasErrors)
		{
		}

		public BoundTypeExpression(SyntaxNode syntax, BoundExpression unevaluatedReceiverOpt, AliasSymbol aliasOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TypeExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(unevaluatedReceiverOpt))
		{
			_UnevaluatedReceiverOpt = unevaluatedReceiverOpt;
			_AliasOpt = aliasOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTypeExpression(this);
		}

		public BoundTypeExpression Update(BoundExpression unevaluatedReceiverOpt, AliasSymbol aliasOpt, TypeSymbol type)
		{
			if (unevaluatedReceiverOpt != UnevaluatedReceiverOpt || (object)aliasOpt != AliasOpt || (object)type != base.Type)
			{
				BoundTypeExpression boundTypeExpression = new BoundTypeExpression(base.Syntax, unevaluatedReceiverOpt, aliasOpt, type, base.HasErrors);
				boundTypeExpression.CopyAttributes(this);
				return boundTypeExpression;
			}
			return this;
		}
	}
}
