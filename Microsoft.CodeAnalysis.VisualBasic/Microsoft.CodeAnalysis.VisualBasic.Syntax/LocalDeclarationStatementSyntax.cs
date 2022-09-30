using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LocalDeclarationStatementSyntax : ExecutableStatementSyntax
	{
		internal SyntaxNode _declarators;

		public SyntaxTokenList Modifiers
		{
			get
			{
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, base.Position, 0);
			}
		}

		public SeparatedSyntaxList<VariableDeclaratorSyntax> Declarators
		{
			get
			{
				SyntaxNode red = GetRed(ref _declarators, 1);
				return (red == null) ? default(SeparatedSyntaxList<VariableDeclaratorSyntax>) : new SeparatedSyntaxList<VariableDeclaratorSyntax>(red, GetChildIndex(1));
			}
		}

		internal LocalDeclarationStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LocalDeclarationStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode modifiers, SyntaxNode declarators)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(kind, errors, annotations, modifiers, declarators?.Green), null, 0)
		{
		}

		public LocalDeclarationStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(modifiers, Declarators);
		}

		public LocalDeclarationStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		public LocalDeclarationStatementSyntax WithDeclarators(SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
		{
			return Update(Modifiers, declarators);
		}

		public LocalDeclarationStatementSyntax AddDeclarators(params VariableDeclaratorSyntax[] items)
		{
			return WithDeclarators(Declarators.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _declarators;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _declarators, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitLocalDeclarationStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLocalDeclarationStatement(this);
		}

		public LocalDeclarationStatementSyntax Update(SyntaxTokenList modifiers, SeparatedSyntaxList<VariableDeclaratorSyntax> declarators)
		{
			if (modifiers != Modifiers || declarators != Declarators)
			{
				LocalDeclarationStatementSyntax localDeclarationStatementSyntax = SyntaxFactory.LocalDeclarationStatement(modifiers, declarators);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(localDeclarationStatementSyntax, annotations);
				}
				return localDeclarationStatementSyntax;
			}
			return this;
		}
	}
}
