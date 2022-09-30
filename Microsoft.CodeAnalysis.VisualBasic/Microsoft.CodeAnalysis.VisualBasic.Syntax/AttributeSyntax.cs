using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AttributeSyntax : VisualBasicSyntaxNode
	{
		internal AttributeTargetSyntax _target;

		internal TypeSyntax _name;

		internal ArgumentListSyntax _argumentList;

		public AttributeTargetSyntax Target => GetRedAtZero(ref _target);

		public TypeSyntax Name => GetRed(ref _name, 1);

		public ArgumentListSyntax ArgumentList => GetRed(ref _argumentList, 2);

		internal AttributeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax(kind, errors, annotations, (target != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)target.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)name.Green, (argumentList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green) : null), null, 0)
		{
		}

		public AttributeSyntax WithTarget(AttributeTargetSyntax target)
		{
			return Update(target, Name, ArgumentList);
		}

		public AttributeSyntax WithName(TypeSyntax name)
		{
			return Update(Target, name, ArgumentList);
		}

		public AttributeSyntax WithArgumentList(ArgumentListSyntax argumentList)
		{
			return Update(Target, Name, argumentList);
		}

		public AttributeSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArgumentList != null) ? ArgumentList : SyntaxFactory.ArgumentList());
			return WithArgumentList(argumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _target, 
				1 => _name, 
				2 => _argumentList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Target, 
				1 => Name, 
				2 => ArgumentList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAttribute(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAttribute(this);
		}

		public AttributeSyntax Update(AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
		{
			if (target != Target || name != Name || argumentList != ArgumentList)
			{
				AttributeSyntax attributeSyntax = SyntaxFactory.Attribute(target, name, argumentList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(attributeSyntax, annotations);
				}
				return attributeSyntax;
			}
			return this;
		}
	}
}
