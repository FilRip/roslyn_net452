using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WithEventsEventContainerSyntax : EventContainerSyntax
	{
		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)base.Green)._identifier, base.Position, 0);

		internal WithEventsEventContainerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WithEventsEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax(kind, errors, annotations, identifier), null, 0)
		{
		}

		public WithEventsEventContainerSyntax WithIdentifier(SyntaxToken identifier)
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
			return visitor.VisitWithEventsEventContainer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWithEventsEventContainer(this);
		}

		public WithEventsEventContainerSyntax Update(SyntaxToken identifier)
		{
			if (identifier != Identifier)
			{
				WithEventsEventContainerSyntax withEventsEventContainerSyntax = SyntaxFactory.WithEventsEventContainer(identifier);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(withEventsEventContainerSyntax, annotations);
				}
				return withEventsEventContainerSyntax;
			}
			return this;
		}
	}
}
