using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeParameterSyntax : VisualBasicSyntaxNode
	{
		internal TypeParameterConstraintClauseSyntax _typeParameterConstraintClause;

		public SyntaxToken VarianceKeyword
		{
			get
			{
				KeywordSyntax varianceKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)base.Green)._varianceKeyword;
				return (varianceKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, varianceKeyword, base.Position, 0);
			}
		}

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax)base.Green)._identifier, GetChildPosition(1), GetChildIndex(1));

		public TypeParameterConstraintClauseSyntax TypeParameterConstraintClause => GetRed(ref _typeParameterConstraintClause, 2);

		internal TypeParameterSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeParameterSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(kind, errors, annotations, varianceKeyword, identifier, (typeParameterConstraintClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)typeParameterConstraintClause.Green) : null), null, 0)
		{
		}

		public TypeParameterSyntax WithVarianceKeyword(SyntaxToken varianceKeyword)
		{
			return Update(varianceKeyword, Identifier, TypeParameterConstraintClause);
		}

		public TypeParameterSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(VarianceKeyword, identifier, TypeParameterConstraintClause);
		}

		public TypeParameterSyntax WithTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
		{
			return Update(VarianceKeyword, Identifier, typeParameterConstraintClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _typeParameterConstraintClause;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return TypeParameterConstraintClause;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameter(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeParameter(this);
		}

		public TypeParameterSyntax Update(SyntaxToken varianceKeyword, SyntaxToken identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
		{
			if (varianceKeyword != VarianceKeyword || identifier != Identifier || typeParameterConstraintClause != TypeParameterConstraintClause)
			{
				TypeParameterSyntax typeParameterSyntax = SyntaxFactory.TypeParameter(varianceKeyword, identifier, typeParameterConstraintClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeParameterSyntax, annotations);
				}
				return typeParameterSyntax;
			}
			return this;
		}
	}
}
