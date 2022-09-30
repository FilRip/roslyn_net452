using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class VariableNameEqualsSyntax : VisualBasicSyntaxNode
	{
		internal ModifiedIdentifierSyntax _identifier;

		internal SimpleAsClauseSyntax _asClause;

		public ModifiedIdentifierSyntax Identifier => GetRedAtZero(ref _identifier);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 1);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)base.Green)._equalsToken, GetChildPosition(2), GetChildIndex(2));

		internal VariableNameEqualsSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal VariableNameEqualsSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)identifier.Green, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, equalsToken), null, 0)
		{
		}

		public VariableNameEqualsSyntax WithIdentifier(ModifiedIdentifierSyntax identifier)
		{
			return Update(identifier, AsClause, EqualsToken);
		}

		public VariableNameEqualsSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Identifier, asClause, EqualsToken);
		}

		public VariableNameEqualsSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(Identifier, AsClause, equalsToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _identifier, 
				1 => _asClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Identifier, 
				1 => AsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitVariableNameEquals(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitVariableNameEquals(this);
		}

		public VariableNameEqualsSyntax Update(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, SyntaxToken equalsToken)
		{
			if (identifier != Identifier || asClause != AsClause || equalsToken != EqualsToken)
			{
				VariableNameEqualsSyntax variableNameEqualsSyntax = SyntaxFactory.VariableNameEquals(identifier, asClause, equalsToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(variableNameEqualsSyntax, annotations);
				}
				return variableNameEqualsSyntax;
			}
			return this;
		}
	}
}
