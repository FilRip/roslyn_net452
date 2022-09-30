using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PredefinedTypeSyntax : TypeSyntax
	{
		public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax)base.Green)._keyword, base.Position, 0);

		internal PredefinedTypeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PredefinedTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax(kind, errors, annotations, keyword), null, 0)
		{
		}

		public PredefinedTypeSyntax WithKeyword(SyntaxToken keyword)
		{
			return Update(keyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPredefinedType(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPredefinedType(this);
		}

		public PredefinedTypeSyntax Update(SyntaxToken keyword)
		{
			if (keyword != Keyword)
			{
				PredefinedTypeSyntax predefinedTypeSyntax = SyntaxFactory.PredefinedType(keyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(predefinedTypeSyntax, annotations);
				}
				return predefinedTypeSyntax;
			}
			return this;
		}
	}
}
