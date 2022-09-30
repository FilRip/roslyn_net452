using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeParameterListSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _parameters;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)base.Green)._openParenToken, base.Position, 0);

		public SyntaxToken OfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)base.Green)._ofKeyword, GetChildPosition(1), GetChildIndex(1));

		public SeparatedSyntaxList<TypeParameterSyntax> Parameters
		{
			get
			{
				SyntaxNode red = GetRed(ref _parameters, 2);
				return (red == null) ? default(SeparatedSyntaxList<TypeParameterSyntax>) : new SeparatedSyntaxList<TypeParameterSyntax>(red, GetChildIndex(2));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal TypeParameterListSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeParameterListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, SyntaxNode parameters, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax(kind, errors, annotations, openParenToken, ofKeyword, parameters?.Green, closeParenToken), null, 0)
		{
		}

		public TypeParameterListSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, OfKeyword, Parameters, CloseParenToken);
		}

		public TypeParameterListSyntax WithOfKeyword(SyntaxToken ofKeyword)
		{
			return Update(OpenParenToken, ofKeyword, Parameters, CloseParenToken);
		}

		public TypeParameterListSyntax WithParameters(SeparatedSyntaxList<TypeParameterSyntax> parameters)
		{
			return Update(OpenParenToken, OfKeyword, parameters, CloseParenToken);
		}

		public TypeParameterListSyntax AddParameters(params TypeParameterSyntax[] items)
		{
			return WithParameters(Parameters.AddRange(items));
		}

		public TypeParameterListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, OfKeyword, Parameters, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _parameters;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _parameters, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameterList(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeParameterList(this);
		}

		public TypeParameterListSyntax Update(SyntaxToken openParenToken, SyntaxToken ofKeyword, SeparatedSyntaxList<TypeParameterSyntax> parameters, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || ofKeyword != OfKeyword || parameters != Parameters || closeParenToken != CloseParenToken)
			{
				TypeParameterListSyntax typeParameterListSyntax = SyntaxFactory.TypeParameterList(openParenToken, ofKeyword, parameters, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeParameterListSyntax, annotations);
				}
				return typeParameterListSyntax;
			}
			return this;
		}
	}
}
