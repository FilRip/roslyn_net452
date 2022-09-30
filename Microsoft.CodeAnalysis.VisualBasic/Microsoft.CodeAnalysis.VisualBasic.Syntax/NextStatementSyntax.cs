using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NextStatementSyntax : StatementSyntax
	{
		internal SyntaxNode _controlVariables;

		public SyntaxToken NextKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)base.Green)._nextKeyword, base.Position, 0);

		public SeparatedSyntaxList<ExpressionSyntax> ControlVariables
		{
			get
			{
				SyntaxNode red = GetRed(ref _controlVariables, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionSyntax>) : new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1));
			}
		}

		internal NextStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NextStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax nextKeyword, SyntaxNode controlVariables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax(kind, errors, annotations, nextKeyword, controlVariables?.Green), null, 0)
		{
		}

		public NextStatementSyntax WithNextKeyword(SyntaxToken nextKeyword)
		{
			return Update(nextKeyword, ControlVariables);
		}

		public NextStatementSyntax WithControlVariables(SeparatedSyntaxList<ExpressionSyntax> controlVariables)
		{
			return Update(NextKeyword, controlVariables);
		}

		public NextStatementSyntax AddControlVariables(params ExpressionSyntax[] items)
		{
			return WithControlVariables(ControlVariables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _controlVariables;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _controlVariables, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNextStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNextStatement(this);
		}

		public NextStatementSyntax Update(SyntaxToken nextKeyword, SeparatedSyntaxList<ExpressionSyntax> controlVariables)
		{
			if (nextKeyword != NextKeyword || controlVariables != ControlVariables)
			{
				NextStatementSyntax nextStatementSyntax = SyntaxFactory.NextStatement(nextKeyword, controlVariables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(nextStatementSyntax, annotations);
				}
				return nextStatementSyntax;
			}
			return this;
		}
	}
}
