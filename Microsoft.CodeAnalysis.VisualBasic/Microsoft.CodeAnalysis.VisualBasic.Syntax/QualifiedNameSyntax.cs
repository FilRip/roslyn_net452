using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class QualifiedNameSyntax : NameSyntax
	{
		internal NameSyntax _left;

		internal SimpleNameSyntax _right;

		public NameSyntax Left => GetRedAtZero(ref _left);

		public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax)base.Green)._dotToken, GetChildPosition(1), GetChildIndex(1));

		public SimpleNameSyntax Right => GetRed(ref _right, 2);

		internal QualifiedNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal QualifiedNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)left.Green, dotToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)right.Green), null, 0)
		{
		}

		public QualifiedNameSyntax WithLeft(NameSyntax left)
		{
			return Update(left, DotToken, Right);
		}

		public QualifiedNameSyntax WithDotToken(SyntaxToken dotToken)
		{
			return Update(Left, dotToken, Right);
		}

		public QualifiedNameSyntax WithRight(SimpleNameSyntax right)
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
			return visitor.VisitQualifiedName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitQualifiedName(this);
		}

		public QualifiedNameSyntax Update(NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
		{
			if (left != Left || dotToken != DotToken || right != Right)
			{
				QualifiedNameSyntax qualifiedNameSyntax = SyntaxFactory.QualifiedName(left, dotToken, right);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(qualifiedNameSyntax, annotations);
				}
				return qualifiedNameSyntax;
			}
			return this;
		}
	}
}
