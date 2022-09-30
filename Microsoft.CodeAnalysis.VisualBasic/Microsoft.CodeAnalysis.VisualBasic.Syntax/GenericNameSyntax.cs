using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GenericNameSyntax : SimpleNameSyntax
	{
		internal TypeArgumentListSyntax _typeArgumentList;

		public new SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax)base.Green)._identifier, base.Position, 0);

		public TypeArgumentListSyntax TypeArgumentList => GetRed(ref _typeArgumentList, 1);

		internal GenericNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GenericNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(kind, errors, annotations, identifier, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)typeArgumentList.Green), null, 0)
		{
		}

		internal override SyntaxToken GetIdentifierCore()
		{
			return Identifier;
		}

		internal override SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier)
		{
			return WithIdentifier(identifier);
		}

		public new GenericNameSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(identifier, TypeArgumentList);
		}

		public GenericNameSyntax WithTypeArgumentList(TypeArgumentListSyntax typeArgumentList)
		{
			return Update(Identifier, typeArgumentList);
		}

		public GenericNameSyntax AddTypeArgumentListArguments(params TypeSyntax[] items)
		{
			TypeArgumentListSyntax typeArgumentListSyntax = ((TypeArgumentList != null) ? TypeArgumentList : SyntaxFactory.TypeArgumentList());
			return WithTypeArgumentList(typeArgumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _typeArgumentList;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return TypeArgumentList;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitGenericName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGenericName(this);
		}

		public GenericNameSyntax Update(SyntaxToken identifier, TypeArgumentListSyntax typeArgumentList)
		{
			if (identifier != Identifier || typeArgumentList != TypeArgumentList)
			{
				GenericNameSyntax genericNameSyntax = SyntaxFactory.GenericName(identifier, typeArgumentList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(genericNameSyntax, annotations);
				}
				return genericNameSyntax;
			}
			return this;
		}
	}
}
