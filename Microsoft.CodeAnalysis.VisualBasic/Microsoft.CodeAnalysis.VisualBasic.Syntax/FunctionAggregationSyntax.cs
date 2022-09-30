using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class FunctionAggregationSyntax : AggregationSyntax
	{
		internal ExpressionSyntax _argument;

		public SyntaxToken FunctionName => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)base.Green)._functionName, base.Position, 0);

		public SyntaxToken OpenParenToken
		{
			get
			{
				PunctuationSyntax openParenToken = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)base.Green)._openParenToken;
				return (openParenToken == null) ? default(SyntaxToken) : new SyntaxToken(this, openParenToken, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public ExpressionSyntax Argument => GetRed(ref _argument, 2);

		public SyntaxToken CloseParenToken
		{
			get
			{
				PunctuationSyntax closeParenToken = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax)base.Green)._closeParenToken;
				return (closeParenToken == null) ? default(SyntaxToken) : new SyntaxToken(this, closeParenToken, GetChildPosition(3), GetChildIndex(3));
			}
		}

		internal FunctionAggregationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal FunctionAggregationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax(kind, errors, annotations, functionName, openParenToken, (argument != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)argument.Green) : null, closeParenToken), null, 0)
		{
		}

		public FunctionAggregationSyntax WithFunctionName(SyntaxToken functionName)
		{
			return Update(functionName, OpenParenToken, Argument, CloseParenToken);
		}

		public FunctionAggregationSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(FunctionName, openParenToken, Argument, CloseParenToken);
		}

		public FunctionAggregationSyntax WithArgument(ExpressionSyntax argument)
		{
			return Update(FunctionName, OpenParenToken, argument, CloseParenToken);
		}

		public FunctionAggregationSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(FunctionName, OpenParenToken, Argument, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _argument;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Argument;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitFunctionAggregation(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitFunctionAggregation(this);
		}

		public FunctionAggregationSyntax Update(SyntaxToken functionName, SyntaxToken openParenToken, ExpressionSyntax argument, SyntaxToken closeParenToken)
		{
			if (functionName != FunctionName || openParenToken != OpenParenToken || argument != Argument || closeParenToken != CloseParenToken)
			{
				FunctionAggregationSyntax functionAggregationSyntax = SyntaxFactory.FunctionAggregation(functionName, openParenToken, argument, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(functionAggregationSyntax, annotations);
				}
				return functionAggregationSyntax;
			}
			return this;
		}
	}
}
