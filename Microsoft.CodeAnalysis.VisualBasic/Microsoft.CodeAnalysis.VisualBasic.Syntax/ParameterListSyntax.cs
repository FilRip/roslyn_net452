using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ParameterListSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _parameters;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)base.Green)._openParenToken, base.Position, 0);

		public SeparatedSyntaxList<ParameterSyntax> Parameters
		{
			get
			{
				SyntaxNode red = GetRed(ref _parameters, 1);
				return (red == null) ? default(SeparatedSyntaxList<ParameterSyntax>) : new SeparatedSyntaxList<ParameterSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal ParameterListSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ParameterListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, SyntaxNode parameters, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax(kind, errors, annotations, openParenToken, parameters?.Green, closeParenToken), null, 0)
		{
		}

		public ParameterListSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, Parameters, CloseParenToken);
		}

		public ParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters)
		{
			return Update(OpenParenToken, parameters, CloseParenToken);
		}

		public ParameterListSyntax AddParameters(params ParameterSyntax[] items)
		{
			return WithParameters(Parameters.AddRange(items));
		}

		public ParameterListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, Parameters, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _parameters;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _parameters, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitParameterList(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitParameterList(this);
		}

		public ParameterListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || parameters != Parameters || closeParenToken != CloseParenToken)
			{
				ParameterListSyntax parameterListSyntax = SyntaxFactory.ParameterList(openParenToken, parameters, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(parameterListSyntax, annotations);
				}
				return parameterListSyntax;
			}
			return this;
		}
	}
}
