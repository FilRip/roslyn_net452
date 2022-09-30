using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypedTupleElementSyntax : TupleElementSyntax
	{
		internal TypeSyntax _type;

		public TypeSyntax Type => GetRedAtZero(ref _type);

		internal TypedTupleElementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypedTupleElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax type)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green), null, 0)
		{
		}

		public TypedTupleElementSyntax WithType(TypeSyntax type)
		{
			return Update(type);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _type;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Type;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypedTupleElement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypedTupleElement(this);
		}

		public TypedTupleElementSyntax Update(TypeSyntax type)
		{
			if (type != Type)
			{
				TypedTupleElementSyntax typedTupleElementSyntax = SyntaxFactory.TypedTupleElement(type);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typedTupleElementSyntax, annotations);
				}
				return typedTupleElementSyntax;
			}
			return this;
		}
	}
}
