using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MidExpressionSyntax : ExpressionSyntax
	{
		internal ArgumentListSyntax _argumentList;

		public SyntaxToken Mid => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax)base.Green)._mid, base.Position, 0);

		public ArgumentListSyntax ArgumentList => GetRed(ref _argumentList, 1);

		internal MidExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MidExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax mid, ArgumentListSyntax argumentList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax(kind, errors, annotations, mid, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green), null, 0)
		{
		}

		public MidExpressionSyntax WithMid(SyntaxToken mid)
		{
			return Update(mid, ArgumentList);
		}

		public MidExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList)
		{
			return Update(Mid, argumentList);
		}

		public MidExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArgumentList != null) ? ArgumentList : SyntaxFactory.ArgumentList());
			return WithArgumentList(argumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _argumentList;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return ArgumentList;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMidExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMidExpression(this);
		}

		public MidExpressionSyntax Update(SyntaxToken mid, ArgumentListSyntax argumentList)
		{
			if (mid != Mid || argumentList != ArgumentList)
			{
				MidExpressionSyntax midExpressionSyntax = SyntaxFactory.MidExpression(mid, argumentList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(midExpressionSyntax, annotations);
				}
				return midExpressionSyntax;
			}
			return this;
		}
	}
}
