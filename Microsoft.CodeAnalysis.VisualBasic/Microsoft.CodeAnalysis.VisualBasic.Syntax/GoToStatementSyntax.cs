using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GoToStatementSyntax : ExecutableStatementSyntax
	{
		internal LabelSyntax _label;

		public SyntaxToken GoToKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax)base.Green)._goToKeyword, base.Position, 0);

		public LabelSyntax Label => GetRed(ref _label, 1);

		internal GoToStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GoToStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax goToKeyword, LabelSyntax label)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax(kind, errors, annotations, goToKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)label.Green), null, 0)
		{
		}

		public GoToStatementSyntax WithGoToKeyword(SyntaxToken goToKeyword)
		{
			return Update(goToKeyword, Label);
		}

		public GoToStatementSyntax WithLabel(LabelSyntax label)
		{
			return Update(GoToKeyword, label);
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
			return visitor.VisitGoToStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGoToStatement(this);
		}

		public GoToStatementSyntax Update(SyntaxToken goToKeyword, LabelSyntax label)
		{
			if (goToKeyword != GoToKeyword || label != Label)
			{
				GoToStatementSyntax goToStatementSyntax = SyntaxFactory.GoToStatement(goToKeyword, label);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(goToStatementSyntax, annotations);
				}
				return goToStatementSyntax;
			}
			return this;
		}
	}
}
