using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GlobalNameSyntax : NameSyntax
	{
		public SyntaxToken GlobalKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax)base.Green)._globalKeyword, base.Position, 0);

		internal GlobalNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GlobalNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax globalKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax(kind, errors, annotations, globalKeyword), null, 0)
		{
		}

		public GlobalNameSyntax WithGlobalKeyword(SyntaxToken globalKeyword)
		{
			return Update(globalKeyword);
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
			return visitor.VisitGlobalName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGlobalName(this);
		}

		public GlobalNameSyntax Update(SyntaxToken globalKeyword)
		{
			if (globalKeyword != GlobalKeyword)
			{
				GlobalNameSyntax globalNameSyntax = SyntaxFactory.GlobalName(globalKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(globalNameSyntax, annotations);
				}
				return globalNameSyntax;
			}
			return this;
		}
	}
}
