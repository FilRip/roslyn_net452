using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OnErrorGoToStatementSyntax : ExecutableStatementSyntax
	{
		internal LabelSyntax _label;

		public SyntaxToken OnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)base.Green)._onKeyword, base.Position, 0);

		public SyntaxToken ErrorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)base.Green)._errorKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken GoToKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)base.Green)._goToKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken Minus
		{
			get
			{
				PunctuationSyntax minus = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax)base.Green)._minus;
				return (minus == null) ? default(SyntaxToken) : new SyntaxToken(this, minus, GetChildPosition(3), GetChildIndex(3));
			}
		}

		public LabelSyntax Label => GetRed(ref _label, 4);

		internal OnErrorGoToStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OnErrorGoToStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(kind, errors, annotations, onKeyword, errorKeyword, goToKeyword, minus, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)label.Green), null, 0)
		{
		}

		public OnErrorGoToStatementSyntax WithOnKeyword(SyntaxToken onKeyword)
		{
			return Update(Kind(), onKeyword, ErrorKeyword, GoToKeyword, Minus, Label);
		}

		public OnErrorGoToStatementSyntax WithErrorKeyword(SyntaxToken errorKeyword)
		{
			return Update(Kind(), OnKeyword, errorKeyword, GoToKeyword, Minus, Label);
		}

		public OnErrorGoToStatementSyntax WithGoToKeyword(SyntaxToken goToKeyword)
		{
			return Update(Kind(), OnKeyword, ErrorKeyword, goToKeyword, Minus, Label);
		}

		public OnErrorGoToStatementSyntax WithMinus(SyntaxToken minus)
		{
			return Update(Kind(), OnKeyword, ErrorKeyword, GoToKeyword, minus, Label);
		}

		public OnErrorGoToStatementSyntax WithLabel(LabelSyntax label)
		{
			return Update(Kind(), OnKeyword, ErrorKeyword, GoToKeyword, Minus, label);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 4)
			{
				return _label;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 4)
			{
				return Label;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOnErrorGoToStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOnErrorGoToStatement(this);
		}

		public OnErrorGoToStatementSyntax Update(SyntaxKind kind, SyntaxToken onKeyword, SyntaxToken errorKeyword, SyntaxToken goToKeyword, SyntaxToken minus, LabelSyntax label)
		{
			if (kind != Kind() || onKeyword != OnKeyword || errorKeyword != ErrorKeyword || goToKeyword != GoToKeyword || minus != Minus || label != Label)
			{
				OnErrorGoToStatementSyntax onErrorGoToStatementSyntax = SyntaxFactory.OnErrorGoToStatement(kind, onKeyword, errorKeyword, goToKeyword, minus, label);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(onErrorGoToStatementSyntax, annotations);
				}
				return onErrorGoToStatementSyntax;
			}
			return this;
		}
	}
}
