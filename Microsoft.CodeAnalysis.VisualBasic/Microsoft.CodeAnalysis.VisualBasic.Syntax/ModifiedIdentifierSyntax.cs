using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ModifiedIdentifierSyntax : VisualBasicSyntaxNode
	{
		internal ArgumentListSyntax _arrayBounds;

		internal SyntaxNode _arrayRankSpecifiers;

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)base.Green)._identifier, base.Position, 0);

		public SyntaxToken Nullable
		{
			get
			{
				PunctuationSyntax nullable = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)base.Green)._nullable;
				return (nullable == null) ? default(SyntaxToken) : new SyntaxToken(this, nullable, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public ArgumentListSyntax ArrayBounds => GetRed(ref _arrayBounds, 2);

		public SyntaxList<ArrayRankSpecifierSyntax> ArrayRankSpecifiers
		{
			get
			{
				SyntaxNode red = GetRed(ref _arrayRankSpecifiers, 3);
				return new SyntaxList<ArrayRankSpecifierSyntax>(red);
			}
		}

		internal ModifiedIdentifierSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ModifiedIdentifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, SyntaxNode arrayRankSpecifiers)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax(kind, errors, annotations, identifier, nullable, (arrayBounds != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)arrayBounds.Green) : null, arrayRankSpecifiers?.Green), null, 0)
		{
		}

		public ModifiedIdentifierSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(identifier, Nullable, ArrayBounds, ArrayRankSpecifiers);
		}

		public ModifiedIdentifierSyntax WithNullable(SyntaxToken nullable)
		{
			return Update(Identifier, nullable, ArrayBounds, ArrayRankSpecifiers);
		}

		public ModifiedIdentifierSyntax WithArrayBounds(ArgumentListSyntax arrayBounds)
		{
			return Update(Identifier, Nullable, arrayBounds, ArrayRankSpecifiers);
		}

		public ModifiedIdentifierSyntax AddArrayBoundsArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArrayBounds != null) ? ArrayBounds : SyntaxFactory.ArgumentList());
			return WithArrayBounds(argumentListSyntax.AddArguments(items));
		}

		public ModifiedIdentifierSyntax WithArrayRankSpecifiers(SyntaxList<ArrayRankSpecifierSyntax> arrayRankSpecifiers)
		{
			return Update(Identifier, Nullable, ArrayBounds, arrayRankSpecifiers);
		}

		public ModifiedIdentifierSyntax AddArrayRankSpecifiers(params ArrayRankSpecifierSyntax[] items)
		{
			return WithArrayRankSpecifiers(ArrayRankSpecifiers.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _arrayBounds, 
				3 => _arrayRankSpecifiers, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => ArrayBounds, 
				3 => GetRed(ref _arrayRankSpecifiers, 3), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitModifiedIdentifier(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitModifiedIdentifier(this);
		}

		public ModifiedIdentifierSyntax Update(SyntaxToken identifier, SyntaxToken nullable, ArgumentListSyntax arrayBounds, SyntaxList<ArrayRankSpecifierSyntax> arrayRankSpecifiers)
		{
			if (identifier != Identifier || nullable != Nullable || arrayBounds != ArrayBounds || arrayRankSpecifiers != ArrayRankSpecifiers)
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = SyntaxFactory.ModifiedIdentifier(identifier, nullable, arrayBounds, arrayRankSpecifiers);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(modifiedIdentifierSyntax, annotations);
				}
				return modifiedIdentifierSyntax;
			}
			return this;
		}
	}
}
