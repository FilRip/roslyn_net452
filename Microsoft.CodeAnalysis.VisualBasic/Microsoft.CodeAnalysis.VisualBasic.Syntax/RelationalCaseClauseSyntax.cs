using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class RelationalCaseClauseSyntax : CaseClauseSyntax
	{
		internal ExpressionSyntax _value;

		public SyntaxToken IsKeyword
		{
			get
			{
				KeywordSyntax isKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)base.Green)._isKeyword;
				return (isKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, isKeyword, base.Position, 0);
			}
		}

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax)base.Green)._operatorToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Value => GetRed(ref _value, 2);

		internal RelationalCaseClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal RelationalCaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax(kind, errors, annotations, isKeyword, operatorToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)value.Green), null, 0)
		{
		}

		public RelationalCaseClauseSyntax WithIsKeyword(SyntaxToken isKeyword)
		{
			return Update(Kind(), isKeyword, OperatorToken, Value);
		}

		public RelationalCaseClauseSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(Kind(), IsKeyword, operatorToken, Value);
		}

		public RelationalCaseClauseSyntax WithValue(ExpressionSyntax value)
		{
			return Update(Kind(), IsKeyword, OperatorToken, value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _value;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Value;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitRelationalCaseClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitRelationalCaseClause(this);
		}

		public RelationalCaseClauseSyntax Update(SyntaxKind kind, SyntaxToken isKeyword, SyntaxToken operatorToken, ExpressionSyntax value)
		{
			if (kind != Kind() || isKeyword != IsKeyword || operatorToken != OperatorToken || value != Value)
			{
				RelationalCaseClauseSyntax relationalCaseClauseSyntax = SyntaxFactory.RelationalCaseClause(kind, isKeyword, operatorToken, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(relationalCaseClauseSyntax, annotations);
				}
				return relationalCaseClauseSyntax;
			}
			return this;
		}
	}
}
