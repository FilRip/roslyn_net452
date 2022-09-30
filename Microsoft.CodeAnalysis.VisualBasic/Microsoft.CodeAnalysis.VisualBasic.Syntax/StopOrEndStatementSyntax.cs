using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class StopOrEndStatementSyntax : ExecutableStatementSyntax
	{
		public SyntaxToken StopOrEndKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax)base.Green)._stopOrEndKeyword, base.Position, 0);

		internal StopOrEndStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal StopOrEndStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax stopOrEndKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax(kind, errors, annotations, stopOrEndKeyword), null, 0)
		{
		}

		public StopOrEndStatementSyntax WithStopOrEndKeyword(SyntaxToken stopOrEndKeyword)
		{
			return Update(Kind(), stopOrEndKeyword);
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
			return visitor.VisitStopOrEndStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitStopOrEndStatement(this);
		}

		public StopOrEndStatementSyntax Update(SyntaxKind kind, SyntaxToken stopOrEndKeyword)
		{
			if (kind != Kind() || stopOrEndKeyword != StopOrEndKeyword)
			{
				StopOrEndStatementSyntax stopOrEndStatementSyntax = SyntaxFactory.StopOrEndStatement(kind, stopOrEndKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(stopOrEndStatementSyntax, annotations);
				}
				return stopOrEndStatementSyntax;
			}
			return this;
		}
	}
}
