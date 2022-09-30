using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SimpleAsClauseSyntax : AsClauseSyntax
	{
		internal SyntaxNode _attributeLists;

		internal TypeSyntax _type;

		public new SyntaxToken AsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)base.Green)._asKeyword, base.Position, 0);

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributeLists, 1);
				return new SyntaxList<AttributeListSyntax>(red);
			}
		}

		public TypeSyntax Type => GetRed(ref _type, 2);

		internal SimpleAsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SimpleAsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax asKeyword, SyntaxNode attributeLists, TypeSyntax type)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(kind, errors, annotations, asKeyword, attributeLists?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green), null, 0)
		{
		}

		internal override SyntaxToken GetAsKeywordCore()
		{
			return AsKeyword;
		}

		internal override AsClauseSyntax WithAsKeywordCore(SyntaxToken asKeyword)
		{
			return WithAsKeyword(asKeyword);
		}

		public new SimpleAsClauseSyntax WithAsKeyword(SyntaxToken asKeyword)
		{
			return Update(asKeyword, AttributeLists, Type);
		}

		public SimpleAsClauseSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(AsKeyword, attributeLists, Type);
		}

		public SimpleAsClauseSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public SimpleAsClauseSyntax WithType(TypeSyntax type)
		{
			return Update(AsKeyword, AttributeLists, type);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _attributeLists, 
				2 => _type, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _attributeLists, 1), 
				2 => Type, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSimpleAsClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSimpleAsClause(this);
		}

		public SimpleAsClauseSyntax Update(SyntaxToken asKeyword, SyntaxList<AttributeListSyntax> attributeLists, TypeSyntax type)
		{
			if (asKeyword != AsKeyword || attributeLists != AttributeLists || type != Type)
			{
				SimpleAsClauseSyntax simpleAsClauseSyntax = SyntaxFactory.SimpleAsClause(asKeyword, attributeLists, type);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(simpleAsClauseSyntax, annotations);
				}
				return simpleAsClauseSyntax;
			}
			return this;
		}
	}
}
