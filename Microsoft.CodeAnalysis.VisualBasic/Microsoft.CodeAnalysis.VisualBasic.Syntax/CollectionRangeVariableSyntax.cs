using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CollectionRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal ModifiedIdentifierSyntax _identifier;

		internal SimpleAsClauseSyntax _asClause;

		internal ExpressionSyntax _expression;

		public ModifiedIdentifierSyntax Identifier => GetRedAtZero(ref _identifier);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 1);

		public SyntaxToken InKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)base.Green)._inKeyword, GetChildPosition(2), GetChildIndex(2));

		public ExpressionSyntax Expression => GetRed(ref _expression, 3);

		internal CollectionRangeVariableSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CollectionRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)identifier.Green, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, inKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public CollectionRangeVariableSyntax WithIdentifier(ModifiedIdentifierSyntax identifier)
		{
			return Update(identifier, AsClause, InKeyword, Expression);
		}

		public CollectionRangeVariableSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Identifier, asClause, InKeyword, Expression);
		}

		public CollectionRangeVariableSyntax WithInKeyword(SyntaxToken inKeyword)
		{
			return Update(Identifier, AsClause, inKeyword, Expression);
		}

		public CollectionRangeVariableSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Identifier, AsClause, InKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _asClause, 
				3 => _expression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Identifier, 
				1 => AsClause, 
				3 => Expression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCollectionRangeVariable(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCollectionRangeVariable(this);
		}

		public CollectionRangeVariableSyntax Update(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, SyntaxToken inKeyword, ExpressionSyntax expression)
		{
			if (identifier != Identifier || asClause != AsClause || inKeyword != InKeyword || expression != Expression)
			{
				CollectionRangeVariableSyntax collectionRangeVariableSyntax = SyntaxFactory.CollectionRangeVariable(identifier, asClause, inKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(collectionRangeVariableSyntax, annotations);
				}
				return collectionRangeVariableSyntax;
			}
			return this;
		}
	}
}
