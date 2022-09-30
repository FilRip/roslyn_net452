using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InferredFieldInitializerSyntax : FieldInitializerSyntax
	{
		internal ExpressionSyntax _expression;

		public new SyntaxToken KeyKeyword
		{
			get
			{
				KeywordSyntax keyKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax)base.Green)._keyKeyword;
				return (keyKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, keyKeyword, base.Position, 0);
			}
		}

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal InferredFieldInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InferredFieldInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax(kind, errors, annotations, keyKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		internal override SyntaxToken GetKeyKeywordCore()
		{
			return KeyKeyword;
		}

		internal override FieldInitializerSyntax WithKeyKeywordCore(SyntaxToken keyKeyword)
		{
			return WithKeyKeyword(keyKeyword);
		}

		public new InferredFieldInitializerSyntax WithKeyKeyword(SyntaxToken keyKeyword)
		{
			return Update(keyKeyword, Expression);
		}

		public InferredFieldInitializerSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(KeyKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInferredFieldInitializer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInferredFieldInitializer(this);
		}

		public InferredFieldInitializerSyntax Update(SyntaxToken keyKeyword, ExpressionSyntax expression)
		{
			if (keyKeyword != KeyKeyword || expression != Expression)
			{
				InferredFieldInitializerSyntax inferredFieldInitializerSyntax = SyntaxFactory.InferredFieldInitializer(keyKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(inferredFieldInitializerSyntax, annotations);
				}
				return inferredFieldInitializerSyntax;
			}
			return this;
		}
	}
}
