using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ResumeStatementSyntax : ExecutableStatementSyntax
	{
		internal LabelSyntax _label;

		public SyntaxToken ResumeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax)base.Green)._resumeKeyword, base.Position, 0);

		public LabelSyntax Label => GetRed(ref _label, 1);

		internal ResumeStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ResumeStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax resumeKeyword, LabelSyntax label)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(kind, errors, annotations, resumeKeyword, (label != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)label.Green) : null), null, 0)
		{
		}

		public ResumeStatementSyntax WithResumeKeyword(SyntaxToken resumeKeyword)
		{
			return Update(Kind(), resumeKeyword, Label);
		}

		public ResumeStatementSyntax WithLabel(LabelSyntax label)
		{
			return Update(Kind(), ResumeKeyword, label);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _label;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Label;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitResumeStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitResumeStatement(this);
		}

		public ResumeStatementSyntax Update(SyntaxKind kind, SyntaxToken resumeKeyword, LabelSyntax label)
		{
			if (kind != Kind() || resumeKeyword != ResumeKeyword || label != Label)
			{
				ResumeStatementSyntax resumeStatementSyntax = SyntaxFactory.ResumeStatement(kind, resumeKeyword, label);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(resumeStatementSyntax, annotations);
				}
				return resumeStatementSyntax;
			}
			return this;
		}
	}
}
