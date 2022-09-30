using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class IdentifierNameSyntax : SimpleNameSyntax
	{
		public new SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)base.Green)._identifier, base.Position, 0);

		internal IdentifierNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal IdentifierNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax(kind, errors, annotations, identifier), null, 0)
		{
		}

		internal override SyntaxToken GetIdentifierCore()
		{
			return Identifier;
		}

		internal override SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier)
		{
			return WithIdentifier(identifier);
		}

		public new IdentifierNameSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(identifier);
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
			return visitor.VisitIdentifierName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitIdentifierName(this);
		}

		public IdentifierNameSyntax Update(SyntaxToken identifier)
		{
			if (identifier != Identifier)
			{
				IdentifierNameSyntax identifierNameSyntax = SyntaxFactory.IdentifierName(identifier);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(identifierNameSyntax, annotations);
				}
				return identifierNameSyntax;
			}
			return this;
		}
	}
}
