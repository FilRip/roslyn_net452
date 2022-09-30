using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundWithStatement : BoundStatement
	{
		private readonly BoundExpression _OriginalExpression;

		private readonly BoundBlock _Body;

		private readonly WithBlockBinder _Binder;

		internal BoundValuePlaceholderBase ExpressionPlaceholder => Binder.ExpressionPlaceholder;

		internal ImmutableArray<BoundExpression> DraftInitializers => Binder.DraftInitializers;

		internal BoundExpression DraftPlaceholderSubstitute => Binder.DraftPlaceholderSubstitute;

		public BoundExpression OriginalExpression => _OriginalExpression;

		public BoundBlock Body => _Body;

		public WithBlockBinder Binder => _Binder;

		public BoundWithStatement(SyntaxNode syntax, BoundExpression originalExpression, BoundBlock body, WithBlockBinder binder, bool hasErrors = false)
			: base(BoundKind.WithStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(originalExpression) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_OriginalExpression = originalExpression;
			_Body = body;
			_Binder = binder;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitWithStatement(this);
		}

		public BoundWithStatement Update(BoundExpression originalExpression, BoundBlock body, WithBlockBinder binder)
		{
			if (originalExpression != OriginalExpression || body != Body || binder != Binder)
			{
				BoundWithStatement boundWithStatement = new BoundWithStatement(base.Syntax, originalExpression, body, binder, base.HasErrors);
				boundWithStatement.CopyAttributes(this);
				return boundWithStatement;
			}
			return this;
		}
	}
}
