namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundFieldOrPropertyInitializer : BoundInitializer
	{
		private readonly BoundExpression _MemberAccessExpressionOpt;

		private readonly BoundExpression _InitialValue;

		public BoundExpression MemberAccessExpressionOpt => _MemberAccessExpressionOpt;

		public BoundExpression InitialValue => _InitialValue;

		protected BoundFieldOrPropertyInitializer(BoundKind kind, SyntaxNode syntax, BoundExpression memberAccessExpressionOpt, BoundExpression initialValue, bool hasErrors = false)
			: base(kind, syntax, hasErrors)
		{
			_MemberAccessExpressionOpt = memberAccessExpressionOpt;
			_InitialValue = initialValue;
		}
	}
}
