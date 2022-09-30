using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ErrorStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _errorNumber;

		public SyntaxToken ErrorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax)base.Green)._errorKeyword, base.Position, 0);

		public ExpressionSyntax ErrorNumber => GetRed(ref _errorNumber, 1);

		internal ErrorStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ErrorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax errorKeyword, ExpressionSyntax errorNumber)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax(kind, errors, annotations, errorKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)errorNumber.Green), null, 0)
		{
		}

		public ErrorStatementSyntax WithErrorKeyword(SyntaxToken errorKeyword)
		{
			return Update(errorKeyword, ErrorNumber);
		}

		public ErrorStatementSyntax WithErrorNumber(ExpressionSyntax errorNumber)
		{
			return Update(ErrorKeyword, errorNumber);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _errorNumber;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return ErrorNumber;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitErrorStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitErrorStatement(this);
		}

		public ErrorStatementSyntax Update(SyntaxToken errorKeyword, ExpressionSyntax errorNumber)
		{
			if (errorKeyword != ErrorKeyword || errorNumber != ErrorNumber)
			{
				ErrorStatementSyntax errorStatementSyntax = SyntaxFactory.ErrorStatement(errorKeyword, errorNumber);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(errorStatementSyntax, annotations);
				}
				return errorStatementSyntax;
			}
			return this;
		}
	}
}
