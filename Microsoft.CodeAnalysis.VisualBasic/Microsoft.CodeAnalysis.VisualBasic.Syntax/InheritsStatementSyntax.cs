using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InheritsStatementSyntax : InheritsOrImplementsStatementSyntax
	{
		internal SyntaxNode _types;

		public SyntaxToken InheritsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)base.Green)._inheritsKeyword, base.Position, 0);

		public SeparatedSyntaxList<TypeSyntax> Types
		{
			get
			{
				SyntaxNode red = GetRed(ref _types, 1);
				return (red == null) ? default(SeparatedSyntaxList<TypeSyntax>) : new SeparatedSyntaxList<TypeSyntax>(red, GetChildIndex(1));
			}
		}

		internal InheritsStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InheritsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax inheritsKeyword, SyntaxNode types)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax(kind, errors, annotations, inheritsKeyword, types?.Green), null, 0)
		{
		}

		public InheritsStatementSyntax WithInheritsKeyword(SyntaxToken inheritsKeyword)
		{
			return Update(inheritsKeyword, Types);
		}

		public InheritsStatementSyntax WithTypes(SeparatedSyntaxList<TypeSyntax> types)
		{
			return Update(InheritsKeyword, types);
		}

		public InheritsStatementSyntax AddTypes(params TypeSyntax[] items)
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
			return visitor.VisitInheritsStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInheritsStatement(this);
		}

		public InheritsStatementSyntax Update(SyntaxToken inheritsKeyword, SeparatedSyntaxList<TypeSyntax> types)
		{
			if (inheritsKeyword != InheritsKeyword || types != Types)
			{
				InheritsStatementSyntax inheritsStatementSyntax = SyntaxFactory.InheritsStatement(inheritsKeyword, types);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(inheritsStatementSyntax, annotations);
				}
				return inheritsStatementSyntax;
			}
			return this;
		}
	}
}
