using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AttributesStatementSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		internal AttributesStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AttributesStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax(kind, errors, annotations, attributeLists?.Green), null, 0)
		{
		}

		public AttributesStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists);
		}

		public AttributesStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _attributeLists;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return GetRedAtZero(ref _attributeLists);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAttributesStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAttributesStatement(this);
		}

		public AttributesStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists)
		{
			if (attributeLists != AttributeLists)
			{
				AttributesStatementSyntax attributesStatementSyntax = SyntaxFactory.AttributesStatement(attributeLists);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(attributesStatementSyntax, annotations);
				}
				return attributesStatementSyntax;
			}
			return this;
		}
	}
}
