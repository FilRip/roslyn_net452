using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EraseStatementSyntax : ExecutableStatementSyntax
	{
		internal SyntaxNode _expressions;

		public SyntaxToken EraseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax)base.Green)._eraseKeyword, base.Position, 0);

		public SeparatedSyntaxList<ExpressionSyntax> Expressions
		{
			get
			{
				SyntaxNode red = GetRed(ref _expressions, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionSyntax>) : new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1));
			}
		}

		internal EraseStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EraseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax eraseKeyword, SyntaxNode expressions)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax(kind, errors, annotations, eraseKeyword, expressions?.Green), null, 0)
		{
		}

		public EraseStatementSyntax WithEraseKeyword(SyntaxToken eraseKeyword)
		{
			return Update(eraseKeyword, Expressions);
		}

		public EraseStatementSyntax WithExpressions(SeparatedSyntaxList<ExpressionSyntax> expressions)
		{
			return Update(EraseKeyword, expressions);
		}

		public EraseStatementSyntax AddExpressions(params ExpressionSyntax[] items)
		{
			return WithExpressions(Expressions.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expressions;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _expressions, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEraseStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEraseStatement(this);
		}

		public EraseStatementSyntax Update(SyntaxToken eraseKeyword, SeparatedSyntaxList<ExpressionSyntax> expressions)
		{
			if (eraseKeyword != EraseKeyword || expressions != Expressions)
			{
				EraseStatementSyntax eraseStatementSyntax = SyntaxFactory.EraseStatement(eraseKeyword, expressions);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(eraseStatementSyntax, annotations);
				}
				return eraseStatementSyntax;
			}
			return this;
		}
	}
}
