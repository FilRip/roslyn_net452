using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TernaryConditionalExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _condition;

		internal ExpressionSyntax _whenTrue;

		internal ExpressionSyntax _whenFalse;

		public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)base.Green)._ifKeyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Condition => GetRed(ref _condition, 2);

		public SyntaxToken FirstCommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)base.Green)._firstCommaToken, GetChildPosition(3), GetChildIndex(3));

		public ExpressionSyntax WhenTrue => GetRed(ref _whenTrue, 4);

		public SyntaxToken SecondCommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)base.Green)._secondCommaToken, GetChildPosition(5), GetChildIndex(5));

		public ExpressionSyntax WhenFalse => GetRed(ref _whenFalse, 6);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(7), GetChildIndex(7));

		internal TernaryConditionalExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TernaryConditionalExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax(kind, errors, annotations, ifKeyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)condition.Green, firstCommaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)whenTrue.Green, secondCommaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)whenFalse.Green, closeParenToken), null, 0)
		{
		}

		public TernaryConditionalExpressionSyntax WithIfKeyword(SyntaxToken ifKeyword)
		{
			return Update(ifKeyword, OpenParenToken, Condition, FirstCommaToken, WhenTrue, SecondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(IfKeyword, openParenToken, Condition, FirstCommaToken, WhenTrue, SecondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithCondition(ExpressionSyntax condition)
		{
			return Update(IfKeyword, OpenParenToken, condition, FirstCommaToken, WhenTrue, SecondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithFirstCommaToken(SyntaxToken firstCommaToken)
		{
			return Update(IfKeyword, OpenParenToken, Condition, firstCommaToken, WhenTrue, SecondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithWhenTrue(ExpressionSyntax whenTrue)
		{
			return Update(IfKeyword, OpenParenToken, Condition, FirstCommaToken, whenTrue, SecondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithSecondCommaToken(SyntaxToken secondCommaToken)
		{
			return Update(IfKeyword, OpenParenToken, Condition, FirstCommaToken, WhenTrue, secondCommaToken, WhenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithWhenFalse(ExpressionSyntax whenFalse)
		{
			return Update(IfKeyword, OpenParenToken, Condition, FirstCommaToken, WhenTrue, SecondCommaToken, whenFalse, CloseParenToken);
		}

		public TernaryConditionalExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(IfKeyword, OpenParenToken, Condition, FirstCommaToken, WhenTrue, SecondCommaToken, WhenFalse, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _condition, 
				4 => _whenTrue, 
				6 => _whenFalse, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => Condition, 
				4 => WhenTrue, 
				6 => WhenFalse, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTernaryConditionalExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTernaryConditionalExpression(this);
		}

		public TernaryConditionalExpressionSyntax Update(SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken firstCommaToken, ExpressionSyntax whenTrue, SyntaxToken secondCommaToken, ExpressionSyntax whenFalse, SyntaxToken closeParenToken)
		{
			if (ifKeyword != IfKeyword || openParenToken != OpenParenToken || condition != Condition || firstCommaToken != FirstCommaToken || whenTrue != WhenTrue || secondCommaToken != SecondCommaToken || whenFalse != WhenFalse || closeParenToken != CloseParenToken)
			{
				TernaryConditionalExpressionSyntax ternaryConditionalExpressionSyntax = SyntaxFactory.TernaryConditionalExpression(ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(ternaryConditionalExpressionSyntax, annotations);
				}
				return ternaryConditionalExpressionSyntax;
			}
			return this;
		}
	}
}
