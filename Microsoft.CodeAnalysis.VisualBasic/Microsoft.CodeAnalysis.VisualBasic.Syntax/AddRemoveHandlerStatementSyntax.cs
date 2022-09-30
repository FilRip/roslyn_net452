using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AddRemoveHandlerStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _eventExpression;

		internal ExpressionSyntax _delegateExpression;

		public SyntaxToken AddHandlerOrRemoveHandlerKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax)base.Green)._addHandlerOrRemoveHandlerKeyword, base.Position, 0);

		public ExpressionSyntax EventExpression => GetRed(ref _eventExpression, 1);

		public SyntaxToken CommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax)base.Green)._commaToken, GetChildPosition(2), GetChildIndex(2));

		public ExpressionSyntax DelegateExpression => GetRed(ref _delegateExpression, 3);

		internal AddRemoveHandlerStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AddRemoveHandlerStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(kind, errors, annotations, addHandlerOrRemoveHandlerKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)eventExpression.Green, commaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)delegateExpression.Green), null, 0)
		{
		}

		public AddRemoveHandlerStatementSyntax WithAddHandlerOrRemoveHandlerKeyword(SyntaxToken addHandlerOrRemoveHandlerKeyword)
		{
			return Update(Kind(), addHandlerOrRemoveHandlerKeyword, EventExpression, CommaToken, DelegateExpression);
		}

		public AddRemoveHandlerStatementSyntax WithEventExpression(ExpressionSyntax eventExpression)
		{
			return Update(Kind(), AddHandlerOrRemoveHandlerKeyword, eventExpression, CommaToken, DelegateExpression);
		}

		public AddRemoveHandlerStatementSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return Update(Kind(), AddHandlerOrRemoveHandlerKeyword, EventExpression, commaToken, DelegateExpression);
		}

		public AddRemoveHandlerStatementSyntax WithDelegateExpression(ExpressionSyntax delegateExpression)
		{
			return Update(Kind(), AddHandlerOrRemoveHandlerKeyword, EventExpression, CommaToken, delegateExpression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _eventExpression, 
				3 => _delegateExpression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => EventExpression, 
				3 => DelegateExpression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAddRemoveHandlerStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAddRemoveHandlerStatement(this);
		}

		public AddRemoveHandlerStatementSyntax Update(SyntaxKind kind, SyntaxToken addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, SyntaxToken commaToken, ExpressionSyntax delegateExpression)
		{
			if (kind != Kind() || addHandlerOrRemoveHandlerKeyword != AddHandlerOrRemoveHandlerKeyword || eventExpression != EventExpression || commaToken != CommaToken || delegateExpression != DelegateExpression)
			{
				AddRemoveHandlerStatementSyntax addRemoveHandlerStatementSyntax = SyntaxFactory.AddRemoveHandlerStatement(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(addRemoveHandlerStatementSyntax, annotations);
				}
				return addRemoveHandlerStatementSyntax;
			}
			return this;
		}
	}
}
