using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NamedTupleElementSyntax : TupleElementSyntax
	{
		internal SimpleAsClauseSyntax _asClause;

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax)base.Green)._identifier, base.Position, 0);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 1);

		internal NamedTupleElementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NamedTupleElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax(kind, errors, annotations, identifier, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
		{
		}

		public NamedTupleElementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(identifier, AsClause);
		}

		public NamedTupleElementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Identifier, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _asClause;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return AsClause;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNamedTupleElement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNamedTupleElement(this);
		}

		public NamedTupleElementSyntax Update(SyntaxToken identifier, SimpleAsClauseSyntax asClause)
		{
			if (identifier != Identifier || asClause != AsClause)
			{
				NamedTupleElementSyntax namedTupleElementSyntax = SyntaxFactory.NamedTupleElement(identifier, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(namedTupleElementSyntax, annotations);
				}
				return namedTupleElementSyntax;
			}
			return this;
		}
	}
}
