using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OptionStatementSyntax : DeclarationStatementSyntax
	{
		public SyntaxToken OptionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)base.Green)._optionKeyword, base.Position, 0);

		public SyntaxToken NameKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)base.Green)._nameKeyword, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken ValueKeyword
		{
			get
			{
				KeywordSyntax valueKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax)base.Green)._valueKeyword;
				return (valueKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, valueKeyword, GetChildPosition(2), GetChildIndex(2));
			}
		}

		internal OptionStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OptionStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(kind, errors, annotations, optionKeyword, nameKeyword, valueKeyword), null, 0)
		{
		}

		public OptionStatementSyntax WithOptionKeyword(SyntaxToken optionKeyword)
		{
			return Update(optionKeyword, NameKeyword, ValueKeyword);
		}

		public OptionStatementSyntax WithNameKeyword(SyntaxToken nameKeyword)
		{
			return Update(OptionKeyword, nameKeyword, ValueKeyword);
		}

		public OptionStatementSyntax WithValueKeyword(SyntaxToken valueKeyword)
		{
			return Update(OptionKeyword, NameKeyword, valueKeyword);
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
			return visitor.VisitOptionStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOptionStatement(this);
		}

		public OptionStatementSyntax Update(SyntaxToken optionKeyword, SyntaxToken nameKeyword, SyntaxToken valueKeyword)
		{
			if (optionKeyword != OptionKeyword || nameKeyword != NameKeyword || valueKeyword != ValueKeyword)
			{
				OptionStatementSyntax optionStatementSyntax = SyntaxFactory.OptionStatement(optionKeyword, nameKeyword, valueKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(optionStatementSyntax, annotations);
				}
				return optionStatementSyntax;
			}
			return this;
		}
	}
}
