using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TupleExpressionSyntax : ExpressionSyntax
	{
		internal SyntaxNode _arguments;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)base.Green)._openParenToken, base.Position, 0);

		public SeparatedSyntaxList<SimpleArgumentSyntax> Arguments
		{
			get
			{
				SyntaxNode red = GetRed(ref _arguments, 1);
				return (red == null) ? default(SeparatedSyntaxList<SimpleArgumentSyntax>) : new SeparatedSyntaxList<SimpleArgumentSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal TupleExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TupleExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, SyntaxNode arguments, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax(kind, errors, annotations, openParenToken, arguments?.Green, closeParenToken), null, 0)
		{
		}

		public TupleExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, Arguments, CloseParenToken);
		}

		public TupleExpressionSyntax WithArguments(SeparatedSyntaxList<SimpleArgumentSyntax> arguments)
		{
			return Update(OpenParenToken, arguments, CloseParenToken);
		}

		public TupleExpressionSyntax AddArguments(params SimpleArgumentSyntax[] items)
		{
			return WithArguments(Arguments.AddRange(items));
		}

		public TupleExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, Arguments, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _arguments;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _arguments, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTupleExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTupleExpression(this);
		}

		public TupleExpressionSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<SimpleArgumentSyntax> arguments, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || arguments != Arguments || closeParenToken != CloseParenToken)
			{
				TupleExpressionSyntax tupleExpressionSyntax = SyntaxFactory.TupleExpression(openParenToken, arguments, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(tupleExpressionSyntax, annotations);
				}
				return tupleExpressionSyntax;
			}
			return this;
		}
	}
}
