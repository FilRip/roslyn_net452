using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TupleTypeSyntax : TypeSyntax
	{
		internal SyntaxNode _elements;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)base.Green)._openParenToken, base.Position, 0);

		public SeparatedSyntaxList<TupleElementSyntax> Elements
		{
			get
			{
				SyntaxNode red = GetRed(ref _elements, 1);
				return (red == null) ? default(SeparatedSyntaxList<TupleElementSyntax>) : new SeparatedSyntaxList<TupleElementSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal TupleTypeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TupleTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, SyntaxNode elements, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax(kind, errors, annotations, openParenToken, elements?.Green, closeParenToken), null, 0)
		{
		}

		public TupleTypeSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, Elements, CloseParenToken);
		}

		public TupleTypeSyntax WithElements(SeparatedSyntaxList<TupleElementSyntax> elements)
		{
			return Update(OpenParenToken, elements, CloseParenToken);
		}

		public TupleTypeSyntax AddElements(params TupleElementSyntax[] items)
		{
			return WithElements(Elements.AddRange(items));
		}

		public TupleTypeSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, Elements, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _elements;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _elements, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTupleType(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTupleType(this);
		}

		public TupleTypeSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<TupleElementSyntax> elements, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || elements != Elements || closeParenToken != CloseParenToken)
			{
				TupleTypeSyntax tupleTypeSyntax = SyntaxFactory.TupleType(openParenToken, elements, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(tupleTypeSyntax, annotations);
				}
				return tupleTypeSyntax;
			}
			return this;
		}
	}
}
