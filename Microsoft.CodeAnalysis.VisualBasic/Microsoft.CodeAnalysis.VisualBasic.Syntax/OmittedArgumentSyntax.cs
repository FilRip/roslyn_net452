using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OmittedArgumentSyntax : ArgumentSyntax
	{
		public SyntaxToken Empty => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax)base.Green)._empty, base.Position, 0);

		public sealed override bool IsNamed => false;

		internal OmittedArgumentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OmittedArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax empty)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax(kind, errors, annotations, empty), null, 0)
		{
		}

		public OmittedArgumentSyntax WithEmpty(SyntaxToken empty)
		{
			return Update(empty);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOmittedArgument(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOmittedArgument(this);
		}

		public OmittedArgumentSyntax Update(SyntaxToken empty)
		{
			if (empty != Empty)
			{
				OmittedArgumentSyntax omittedArgumentSyntax = SyntaxFactory.OmittedArgument(empty);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(omittedArgumentSyntax, annotations);
				}
				return omittedArgumentSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override ExpressionSyntax GetExpression()
		{
			return null;
		}
	}
}
