using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ForEachStatementSyntax : ForOrForEachStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public new SyntaxToken ForKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green)._forKeyword, base.Position, 0);

		public SyntaxToken EachKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green)._eachKeyword, GetChildPosition(1), GetChildIndex(1));

		public new VisualBasicSyntaxNode ControlVariable => GetRed(ref _controlVariable, 2);

		public SyntaxToken InKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green)._inKeyword, GetChildPosition(3), GetChildIndex(3));

		public ExpressionSyntax Expression => GetRed(ref _expression, 4);

		internal ForEachStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ForEachStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(kind, errors, annotations, forKeyword, eachKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)controlVariable.Green, inKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		internal override SyntaxToken GetForKeywordCore()
		{
			return ForKeyword;
		}

		internal override ForOrForEachStatementSyntax WithForKeywordCore(SyntaxToken forKeyword)
		{
			return WithForKeyword(forKeyword);
		}

		public new ForEachStatementSyntax WithForKeyword(SyntaxToken forKeyword)
		{
			return Update(forKeyword, EachKeyword, ControlVariable, InKeyword, Expression);
		}

		public ForEachStatementSyntax WithEachKeyword(SyntaxToken eachKeyword)
		{
			return Update(ForKeyword, eachKeyword, ControlVariable, InKeyword, Expression);
		}

		internal override VisualBasicSyntaxNode GetControlVariableCore()
		{
			return ControlVariable;
		}

		internal override ForOrForEachStatementSyntax WithControlVariableCore(VisualBasicSyntaxNode controlVariable)
		{
			return WithControlVariable(controlVariable);
		}

		public new ForEachStatementSyntax WithControlVariable(VisualBasicSyntaxNode controlVariable)
		{
			return Update(ForKeyword, EachKeyword, controlVariable, InKeyword, Expression);
		}

		public ForEachStatementSyntax WithInKeyword(SyntaxToken inKeyword)
		{
			return Update(ForKeyword, EachKeyword, ControlVariable, inKeyword, Expression);
		}

		public ForEachStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(ForKeyword, EachKeyword, ControlVariable, InKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _controlVariable, 
				4 => _expression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => ControlVariable, 
				4 => Expression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitForEachStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitForEachStatement(this);
		}

		public ForEachStatementSyntax Update(SyntaxToken forKeyword, SyntaxToken eachKeyword, VisualBasicSyntaxNode controlVariable, SyntaxToken inKeyword, ExpressionSyntax expression)
		{
			if (forKeyword != ForKeyword || eachKeyword != EachKeyword || controlVariable != ControlVariable || inKeyword != InKeyword || expression != Expression)
			{
				ForEachStatementSyntax forEachStatementSyntax = SyntaxFactory.ForEachStatement(forKeyword, eachKeyword, controlVariable, inKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(forEachStatementSyntax, annotations);
				}
				return forEachStatementSyntax;
			}
			return this;
		}
	}
}
