using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CrefSignaturePartSyntax : VisualBasicSyntaxNode
	{
		internal TypeSyntax _type;

		public SyntaxToken Modifier
		{
			get
			{
				KeywordSyntax modifier = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax)base.Green)._modifier;
				return (modifier == null) ? default(SyntaxToken) : new SyntaxToken(this, modifier, base.Position, 0);
			}
		}

		public TypeSyntax Type => GetRed(ref _type, 1);

		internal CrefSignaturePartSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CrefSignaturePartSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax modifier, TypeSyntax type)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax(kind, errors, annotations, modifier, (type != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green) : null), null, 0)
		{
		}

		public CrefSignaturePartSyntax WithModifier(SyntaxToken modifier)
		{
			return Update(modifier, Type);
		}

		public CrefSignaturePartSyntax WithType(TypeSyntax type)
		{
			return Update(Modifier, type);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _type;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Type;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCrefSignaturePart(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCrefSignaturePart(this);
		}

		public CrefSignaturePartSyntax Update(SyntaxToken modifier, TypeSyntax type)
		{
			if (modifier != Modifier || type != Type)
			{
				CrefSignaturePartSyntax crefSignaturePartSyntax = SyntaxFactory.CrefSignaturePart(modifier, type);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(crefSignaturePartSyntax, annotations);
				}
				return crefSignaturePartSyntax;
			}
			return this;
		}
	}
}
