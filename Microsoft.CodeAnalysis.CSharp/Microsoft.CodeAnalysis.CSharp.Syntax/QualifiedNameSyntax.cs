#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class QualifiedNameSyntax : NameSyntax
    {
        private NameSyntax? left;

        private SimpleNameSyntax? right;

        public NameSyntax Left => GetRedAtZero(ref left);

        public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QualifiedNameSyntax)base.Green).dotToken, GetChildPosition(1), GetChildIndex(1));

        public SimpleNameSyntax Right => GetRed(ref right, 2);

        internal override SimpleNameSyntax GetUnqualifiedName()
        {
            return Right;
        }

        internal override string ErrorDisplayName()
        {
            return Left.ErrorDisplayName() + "." + Right.ErrorDisplayName();
        }

        internal QualifiedNameSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref left),
                2 => GetRed(ref right, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => left,
                2 => right,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQualifiedName(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQualifiedName(this);

        public QualifiedNameSyntax Update(NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
        {
            if (left != Left || dotToken != DotToken || right != Right)
            {
                QualifiedNameSyntax qualifiedNameSyntax = SyntaxFactory.QualifiedName(left, dotToken, right);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return qualifiedNameSyntax;
                }
                return qualifiedNameSyntax.WithAnnotations(annotations);
            }
            return this;
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
    }
}
