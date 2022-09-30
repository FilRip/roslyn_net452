using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WithEventsPropertyEventContainerSyntax : EventContainerSyntax
	{
		internal WithEventsEventContainerSyntax _withEventsContainer;

		internal IdentifierNameSyntax _property;

		public WithEventsEventContainerSyntax WithEventsContainer => GetRedAtZero(ref _withEventsContainer);

		public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax)base.Green)._dotToken, GetChildPosition(1), GetChildIndex(1));

		public IdentifierNameSyntax Property => GetRed(ref _property, 2);

		internal WithEventsPropertyEventContainerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WithEventsPropertyEventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax)withEventsContainer.Green, dotToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)property.Green), null, 0)
		{
		}

		public WithEventsPropertyEventContainerSyntax WithWithEventsContainer(WithEventsEventContainerSyntax withEventsContainer)
		{
			return Update(withEventsContainer, DotToken, Property);
		}

		public WithEventsPropertyEventContainerSyntax WithDotToken(SyntaxToken dotToken)
		{
			return Update(WithEventsContainer, dotToken, Property);
		}

		public WithEventsPropertyEventContainerSyntax WithProperty(IdentifierNameSyntax property)
		{
			return Update(WithEventsContainer, DotToken, property);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _withEventsContainer, 
				2 => _property, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => WithEventsContainer, 
				2 => Property, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitWithEventsPropertyEventContainer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWithEventsPropertyEventContainer(this);
		}

		public WithEventsPropertyEventContainerSyntax Update(WithEventsEventContainerSyntax withEventsContainer, SyntaxToken dotToken, IdentifierNameSyntax property)
		{
			if (withEventsContainer != WithEventsContainer || dotToken != DotToken || property != Property)
			{
				WithEventsPropertyEventContainerSyntax withEventsPropertyEventContainerSyntax = SyntaxFactory.WithEventsPropertyEventContainer(withEventsContainer, dotToken, property);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(withEventsPropertyEventContainerSyntax, annotations);
				}
				return withEventsPropertyEventContainerSyntax;
			}
			return this;
		}
	}
}
