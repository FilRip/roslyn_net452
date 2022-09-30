using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ImplementsStatementSyntax : InheritsOrImplementsStatementSyntax
	{
		internal SyntaxNode _types;

		public SyntaxToken ImplementsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)base.Green)._implementsKeyword, base.Position, 0);

		public SeparatedSyntaxList<TypeSyntax> Types
		{
			get
			{
				SyntaxNode red = GetRed(ref _types, 1);
				return (red == null) ? default(SeparatedSyntaxList<TypeSyntax>) : new SeparatedSyntaxList<TypeSyntax>(red, GetChildIndex(1));
			}
		}

		internal ImplementsStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ImplementsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax implementsKeyword, SyntaxNode types)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax(kind, errors, annotations, implementsKeyword, types?.Green), null, 0)
		{
		}

		public ImplementsStatementSyntax WithImplementsKeyword(SyntaxToken implementsKeyword)
		{
			return Update(implementsKeyword, Types);
		}

		public ImplementsStatementSyntax WithTypes(SeparatedSyntaxList<TypeSyntax> types)
		{
			return Update(ImplementsKeyword, types);
		}

		public ImplementsStatementSyntax AddTypes(params TypeSyntax[] items)
		{
			return WithTypes(Types.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _types;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _types, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitImplementsStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitImplementsStatement(this);
		}

		public ImplementsStatementSyntax Update(SyntaxToken implementsKeyword, SeparatedSyntaxList<TypeSyntax> types)
		{
			if (implementsKeyword != ImplementsKeyword || types != Types)
			{
				ImplementsStatementSyntax implementsStatementSyntax = SyntaxFactory.ImplementsStatement(implementsKeyword, types);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(implementsStatementSyntax, annotations);
				}
				return implementsStatementSyntax;
			}
			return this;
		}
	}
}
