using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CrefReferenceSyntax : VisualBasicSyntaxNode
	{
		internal TypeSyntax _name;

		internal CrefSignatureSyntax _signature;

		internal SimpleAsClauseSyntax _asClause;

		public TypeSyntax Name => GetRedAtZero(ref _name);

		public CrefSignatureSyntax Signature => GetRed(ref _signature, 1);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 2);

		internal CrefReferenceSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CrefReferenceSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)name.Green, (signature != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)signature.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
		{
		}

		public CrefReferenceSyntax WithName(TypeSyntax name)
		{
			return Update(name, Signature, AsClause);
		}

		public CrefReferenceSyntax WithSignature(CrefSignatureSyntax signature)
		{
			return Update(Name, signature, AsClause);
		}

		public CrefReferenceSyntax AddSignatureArgumentTypes(params CrefSignaturePartSyntax[] items)
		{
			CrefSignatureSyntax crefSignatureSyntax = ((Signature != null) ? Signature : SyntaxFactory.CrefSignature());
			return WithSignature(crefSignatureSyntax.AddArgumentTypes(items));
		}

		public CrefReferenceSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Name, Signature, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _signature, 
				2 => _asClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Name, 
				1 => Signature, 
				2 => AsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCrefReference(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCrefReference(this);
		}

		public CrefReferenceSyntax Update(TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
		{
			if (name != Name || signature != Signature || asClause != AsClause)
			{
				CrefReferenceSyntax crefReferenceSyntax = SyntaxFactory.CrefReference(name, signature, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(crefReferenceSyntax, annotations);
				}
				return crefReferenceSyntax;
			}
			return this;
		}
	}
}
