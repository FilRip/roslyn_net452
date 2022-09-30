using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CallStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _invocation;

		public SyntaxToken CallKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax)base.Green)._callKeyword, base.Position, 0);

		public ExpressionSyntax Invocation => GetRed(ref _invocation, 1);

		internal CallStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CallStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax callKeyword, ExpressionSyntax invocation)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax(kind, errors, annotations, callKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)invocation.Green), null, 0)
		{
		}

		public CallStatementSyntax WithCallKeyword(SyntaxToken callKeyword)
		{
			return Update(callKeyword, Invocation);
		}

		public CallStatementSyntax WithInvocation(ExpressionSyntax invocation)
		{
			return Update(CallKeyword, invocation);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _invocation;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Invocation;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCallStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCallStatement(this);
		}

		public CallStatementSyntax Update(SyntaxToken callKeyword, ExpressionSyntax invocation)
		{
			if (callKeyword != CallKeyword || invocation != Invocation)
			{
				CallStatementSyntax callStatementSyntax = SyntaxFactory.CallStatement(callKeyword, invocation);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(callStatementSyntax, annotations);
				}
				return callStatementSyntax;
			}
			return this;
		}
	}
}
