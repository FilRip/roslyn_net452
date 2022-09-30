using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class QualifiedCrefOperatorReferenceSyntax : NameSyntax
	{
		internal NameSyntax _left;

		internal CrefOperatorReferenceSyntax _right;

		public NameSyntax Left => GetRedAtZero(ref _left);

		public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax)base.Green)._dotToken, GetChildPosition(1), GetChildIndex(1));

		public CrefOperatorReferenceSyntax Right => GetRed(ref _right, 2);

		internal QualifiedCrefOperatorReferenceSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal QualifiedCrefOperatorReferenceSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NameSyntax left, PunctuationSyntax dotToken, CrefOperatorReferenceSyntax right)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)left.Green, dotToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax)right.Green), null, 0)
		{
		}

		public QualifiedCrefOperatorReferenceSyntax WithLeft(NameSyntax left)
		{
			return Update(left, DotToken, Right);
		}

		public QualifiedCrefOperatorReferenceSyntax WithDotToken(SyntaxToken dotToken)
		{
			return Update(Left, dotToken, Right);
		}

		public QualifiedCrefOperatorReferenceSyntax WithRight(CrefOperatorReferenceSyntax right)
		{
			return Update(Left, DotToken, right);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _left, 
				2 => _right, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Left, 
				2 => Right, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitQualifiedCrefOperatorReference(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitQualifiedCrefOperatorReference(this);
		}

		public QualifiedCrefOperatorReferenceSyntax Update(NameSyntax left, SyntaxToken dotToken, CrefOperatorReferenceSyntax right)
		{
			if (left != Left || dotToken != DotToken || right != Right)
			{
				QualifiedCrefOperatorReferenceSyntax qualifiedCrefOperatorReferenceSyntax = SyntaxFactory.QualifiedCrefOperatorReference(left, dotToken, right);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(qualifiedCrefOperatorReferenceSyntax, annotations);
				}
				return qualifiedCrefOperatorReferenceSyntax;
			}
			return this;
		}
	}
}
