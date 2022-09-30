using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class HandlesClauseSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _events;

		public SyntaxToken HandlesKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)base.Green)._handlesKeyword, base.Position, 0);

		public SeparatedSyntaxList<HandlesClauseItemSyntax> Events
		{
			get
			{
				SyntaxNode red = GetRed(ref _events, 1);
				return (red == null) ? default(SeparatedSyntaxList<HandlesClauseItemSyntax>) : new SeparatedSyntaxList<HandlesClauseItemSyntax>(red, GetChildIndex(1));
			}
		}

		internal HandlesClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal HandlesClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax handlesKeyword, SyntaxNode events)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax(kind, errors, annotations, handlesKeyword, events?.Green), null, 0)
		{
		}

		public HandlesClauseSyntax WithHandlesKeyword(SyntaxToken handlesKeyword)
		{
			return Update(handlesKeyword, Events);
		}

		public HandlesClauseSyntax WithEvents(SeparatedSyntaxList<HandlesClauseItemSyntax> events)
		{
			return Update(HandlesKeyword, events);
		}

		public HandlesClauseSyntax AddEvents(params HandlesClauseItemSyntax[] items)
		{
			return WithEvents(Events.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _events;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _events, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitHandlesClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitHandlesClause(this);
		}

		public HandlesClauseSyntax Update(SyntaxToken handlesKeyword, SeparatedSyntaxList<HandlesClauseItemSyntax> events)
		{
			if (handlesKeyword != HandlesKeyword || events != Events)
			{
				HandlesClauseSyntax handlesClauseSyntax = SyntaxFactory.HandlesClause(handlesKeyword, events);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(handlesClauseSyntax, annotations);
				}
				return handlesClauseSyntax;
			}
			return this;
		}
	}
}
