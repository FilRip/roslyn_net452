using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class HandlesClauseItemSyntax : VisualBasicSyntaxNode
	{
		internal EventContainerSyntax _eventContainer;

		internal IdentifierNameSyntax _eventMember;

		public EventContainerSyntax EventContainer => GetRedAtZero(ref _eventContainer);

		public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax)base.Green)._dotToken, GetChildPosition(1), GetChildIndex(1));

		public IdentifierNameSyntax EventMember => GetRed(ref _eventMember, 2);

		internal HandlesClauseItemSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal HandlesClauseItemSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax)eventContainer.Green, dotToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)eventMember.Green), null, 0)
		{
		}

		public HandlesClauseItemSyntax WithEventContainer(EventContainerSyntax eventContainer)
		{
			return Update(eventContainer, DotToken, EventMember);
		}

		public HandlesClauseItemSyntax WithDotToken(SyntaxToken dotToken)
		{
			return Update(EventContainer, dotToken, EventMember);
		}

		public HandlesClauseItemSyntax WithEventMember(IdentifierNameSyntax eventMember)
		{
			return Update(EventContainer, DotToken, eventMember);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _eventContainer, 
				2 => _eventMember, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => EventContainer, 
				2 => EventMember, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitHandlesClauseItem(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitHandlesClauseItem(this);
		}

		public HandlesClauseItemSyntax Update(EventContainerSyntax eventContainer, SyntaxToken dotToken, IdentifierNameSyntax eventMember)
		{
			if (eventContainer != EventContainer || dotToken != DotToken || eventMember != EventMember)
			{
				HandlesClauseItemSyntax handlesClauseItemSyntax = SyntaxFactory.HandlesClauseItem(eventContainer, dotToken, eventMember);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(handlesClauseItemSyntax, annotations);
				}
				return handlesClauseItemSyntax;
			}
			return this;
		}
	}
}
