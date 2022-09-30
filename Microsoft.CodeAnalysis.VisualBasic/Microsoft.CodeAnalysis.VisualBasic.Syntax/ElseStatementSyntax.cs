using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ElseStatementSyntax : StatementSyntax
	{
		public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)base.Green)._elseKeyword, base.Position, 0);

		internal ElseStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ElseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax(kind, errors, annotations, elseKeyword), null, 0)
		{
		}

		public ElseStatementSyntax WithElseKeyword(SyntaxToken elseKeyword)
		{
			return Update(elseKeyword);
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
			return visitor.VisitElseStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitElseStatement(this);
		}

		public ElseStatementSyntax Update(SyntaxToken elseKeyword)
		{
			if (elseKeyword != ElseKeyword)
			{
				ElseStatementSyntax elseStatementSyntax = SyntaxFactory.ElseStatement(elseKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(elseStatementSyntax, annotations);
				}
				return elseStatementSyntax;
			}
			return this;
		}
	}
}
