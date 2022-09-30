#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class QualifiedCrefSyntax : CrefSyntax
    {
        private TypeSyntax? container;

        private MemberCrefSyntax? member;

        public TypeSyntax Container => GetRedAtZero(ref container);

        public SyntaxToken DotToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.QualifiedCrefSyntax)base.Green).dotToken, GetChildPosition(1), GetChildIndex(1));

        public MemberCrefSyntax Member => GetRed(ref member, 2);

        internal QualifiedCrefSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref container),
                2 => GetRed(ref member, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => container,
                2 => member,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQualifiedCref(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitQualifiedCref(this);

        public QualifiedCrefSyntax Update(TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member)
        {
            if (container != Container || dotToken != DotToken || member != Member)
            {
                QualifiedCrefSyntax qualifiedCrefSyntax = SyntaxFactory.QualifiedCref(container, dotToken, member);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return qualifiedCrefSyntax;
                }
                return qualifiedCrefSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public QualifiedCrefSyntax WithContainer(TypeSyntax container)
        {
            return Update(container, DotToken, Member);
        }

        public QualifiedCrefSyntax WithDotToken(SyntaxToken dotToken)
        {
            return Update(Container, dotToken, Member);
        }

        public QualifiedCrefSyntax WithMember(MemberCrefSyntax member)
        {
            return Update(Container, DotToken, member);
        }
    }
}
