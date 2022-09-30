using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NameColonEqualsSyntax : VisualBasicSyntaxNode
	{
		internal IdentifierNameSyntax _name;

		public IdentifierNameSyntax Name => GetRedAtZero(ref _name);

		public SyntaxToken ColonEqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax)base.Green)._colonEqualsToken, GetChildPosition(1), GetChildIndex(1));

		internal NameColonEqualsSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NameColonEqualsSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)name.Green, colonEqualsToken), null, 0)
		{
		}

		public NameColonEqualsSyntax WithName(IdentifierNameSyntax name)
		{
			return Update(name, ColonEqualsToken);
		}

		public NameColonEqualsSyntax WithColonEqualsToken(SyntaxToken colonEqualsToken)
		{
			return Update(Name, colonEqualsToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _name;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Name;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNameColonEquals(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNameColonEquals(this);
		}

		public NameColonEqualsSyntax Update(IdentifierNameSyntax name, SyntaxToken colonEqualsToken)
		{
			if (name != Name || colonEqualsToken != ColonEqualsToken)
			{
				NameColonEqualsSyntax nameColonEqualsSyntax = SyntaxFactory.NameColonEquals(name, colonEqualsToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(nameColonEqualsSyntax, annotations);
				}
				return nameColonEqualsSyntax;
			}
			return this;
		}
	}
}
