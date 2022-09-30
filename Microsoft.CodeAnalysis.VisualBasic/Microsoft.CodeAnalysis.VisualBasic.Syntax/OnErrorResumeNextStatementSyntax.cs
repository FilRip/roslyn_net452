using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OnErrorResumeNextStatementSyntax : ExecutableStatementSyntax
	{
		public SyntaxToken OnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)base.Green)._onKeyword, base.Position, 0);

		public SyntaxToken ErrorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)base.Green)._errorKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken ResumeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)base.Green)._resumeKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken NextKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax)base.Green)._nextKeyword, GetChildPosition(3), GetChildIndex(3));

		internal OnErrorResumeNextStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OnErrorResumeNextStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(kind, errors, annotations, onKeyword, errorKeyword, resumeKeyword, nextKeyword), null, 0)
		{
		}

		public OnErrorResumeNextStatementSyntax WithOnKeyword(SyntaxToken onKeyword)
		{
			return Update(onKeyword, ErrorKeyword, ResumeKeyword, NextKeyword);
		}

		public OnErrorResumeNextStatementSyntax WithErrorKeyword(SyntaxToken errorKeyword)
		{
			return Update(OnKeyword, errorKeyword, ResumeKeyword, NextKeyword);
		}

		public OnErrorResumeNextStatementSyntax WithResumeKeyword(SyntaxToken resumeKeyword)
		{
			return Update(OnKeyword, ErrorKeyword, resumeKeyword, NextKeyword);
		}

		public OnErrorResumeNextStatementSyntax WithNextKeyword(SyntaxToken nextKeyword)
		{
			return Update(OnKeyword, ErrorKeyword, ResumeKeyword, nextKeyword);
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
			return visitor.VisitOnErrorResumeNextStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOnErrorResumeNextStatement(this);
		}

		public OnErrorResumeNextStatementSyntax Update(SyntaxToken onKeyword, SyntaxToken errorKeyword, SyntaxToken resumeKeyword, SyntaxToken nextKeyword)
		{
			if (onKeyword != OnKeyword || errorKeyword != ErrorKeyword || resumeKeyword != ResumeKeyword || nextKeyword != NextKeyword)
			{
				OnErrorResumeNextStatementSyntax onErrorResumeNextStatementSyntax = SyntaxFactory.OnErrorResumeNextStatement(onKeyword, errorKeyword, resumeKeyword, nextKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(onErrorResumeNextStatementSyntax, annotations);
				}
				return onErrorResumeNextStatementSyntax;
			}
			return this;
		}
	}
}
