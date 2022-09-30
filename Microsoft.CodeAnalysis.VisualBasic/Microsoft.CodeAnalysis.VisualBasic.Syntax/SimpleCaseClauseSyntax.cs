using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SimpleCaseClauseSyntax : CaseClauseSyntax
	{
		internal ExpressionSyntax _value;

		public ExpressionSyntax Value => GetRedAtZero(ref _value);

		internal SimpleCaseClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SimpleCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)value.Green), null, 0)
		{
		}

		public SimpleCaseClauseSyntax WithValue(ExpressionSyntax value)
		{
			return Update(value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _value;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Value;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSimpleCaseClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSimpleCaseClause(this);
		}

		public SimpleCaseClauseSyntax Update(ExpressionSyntax value)
		{
			if (value != Value)
			{
				SimpleCaseClauseSyntax simpleCaseClauseSyntax = SyntaxFactory.SimpleCaseClause(value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(simpleCaseClauseSyntax, annotations);
				}
				return simpleCaseClauseSyntax;
			}
			return this;
		}
	}
}
