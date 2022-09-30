using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeConstraintSyntax : ConstraintSyntax
	{
		internal TypeSyntax _type;

		public TypeSyntax Type => GetRedAtZero(ref _type);

		internal TypeConstraintSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeConstraintSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax type)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green), null, 0)
		{
		}

		public TypeConstraintSyntax WithType(TypeSyntax type)
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
			return visitor.VisitTypeConstraint(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeConstraint(this);
		}

		public TypeConstraintSyntax Update(TypeSyntax type)
		{
			if (type != Type)
			{
				TypeConstraintSyntax typeConstraintSyntax = SyntaxFactory.TypeConstraint(type);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeConstraintSyntax, annotations);
				}
				return typeConstraintSyntax;
			}
			return this;
		}
	}
}
