#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AliasQualifiedNameSyntax : NameSyntax
    {
        private IdentifierNameSyntax? alias;

        private SimpleNameSyntax? name;

        public IdentifierNameSyntax Alias => GetRedAtZero(ref alias);

        public SyntaxToken ColonColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AliasQualifiedNameSyntax)base.Green).colonColonToken, GetChildPosition(1), GetChildIndex(1));

        public SimpleNameSyntax Name => GetRed(ref name, 2);

        internal override SimpleNameSyntax GetUnqualifiedName()
        {
            return Name;
        }

        internal override string ErrorDisplayName()
        {
            return Alias.ErrorDisplayName() + "::" + Name.ErrorDisplayName();
        }

        internal AliasQualifiedNameSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref alias),
                2 => GetRed(ref name, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => alias,
                2 => name,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAliasQualifiedName(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAliasQualifiedName(this);

        public AliasQualifiedNameSyntax Update(IdentifierNameSyntax alias, SyntaxToken colonColonToken, SimpleNameSyntax name)
        {
            if (alias != Alias || colonColonToken != ColonColonToken || name != Name)
            {
                AliasQualifiedNameSyntax aliasQualifiedNameSyntax = SyntaxFactory.AliasQualifiedName(alias, colonColonToken, name);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return aliasQualifiedNameSyntax;
                }
                return aliasQualifiedNameSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AliasQualifiedNameSyntax WithAlias(IdentifierNameSyntax alias)
        {
            return Update(alias, ColonColonToken, Name);
        }

        public AliasQualifiedNameSyntax WithColonColonToken(SyntaxToken colonColonToken)
        {
            return Update(Alias, colonColonToken, Name);
        }

        public AliasQualifiedNameSyntax WithName(SimpleNameSyntax name)
        {
            return Update(Alias, ColonColonToken, name);
        }
    }
}
