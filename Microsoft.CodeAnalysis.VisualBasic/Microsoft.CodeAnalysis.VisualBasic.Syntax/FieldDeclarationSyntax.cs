using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class FieldDeclarationSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		internal SyntaxNode _declarators;

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		public SyntaxTokenList Modifiers
		{
			get
			{
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SeparatedSyntaxList<VariableDeclaratorSyntax> Declarators
		{
			get
			{
				SyntaxNode red = GetRed(ref _declarators, 2);
				return (red == null) ? default(SeparatedSyntaxList<VariableDeclaratorSyntax>) : new SeparatedSyntaxList<VariableDeclaratorSyntax>(red, GetChildIndex(2));
			}
		}

		internal FieldDeclarationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal FieldDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, SyntaxNode declarators)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, declarators?.Green), null, 0)
		{
		}

		public FieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, Declarators);
		}

		public FieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public FieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, Declarators);
		}

		public FieldDeclarationSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		public FieldDeclarationSyntax WithDeclarators(SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
		{
			return Update(AttributeLists, Modifiers, declarators);
		}

		public FieldDeclarationSyntax AddDeclarators(params VariableDeclaratorSyntax[] items)
		{
			return WithDeclarators(Declarators.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				2 => _declarators, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				2 => GetRed(ref _declarators, 2), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitFieldDeclaration(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitFieldDeclaration(this);
		}

		public FieldDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || declarators != Declarators)
			{
				FieldDeclarationSyntax fieldDeclarationSyntax = SyntaxFactory.FieldDeclaration(attributeLists, modifiers, declarators);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(fieldDeclarationSyntax, annotations);
				}
				return fieldDeclarationSyntax;
			}
			return this;
		}
	}
}
