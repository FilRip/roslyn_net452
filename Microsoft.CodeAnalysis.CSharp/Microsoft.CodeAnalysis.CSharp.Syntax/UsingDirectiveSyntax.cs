#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class UsingDirectiveSyntax : CSharpSyntaxNode
    {
        private NameEqualsSyntax? alias;

        private NameSyntax? name;

        public SyntaxToken GlobalKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken globalKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax)base.Green).globalKeyword;
                if (globalKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, globalKeyword, base.Position, 0);
            }
        }

        public SyntaxToken UsingKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax)base.Green).usingKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken StaticKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken staticKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax)base.Green).staticKeyword;
                if (staticKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, staticKeyword, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public NameEqualsSyntax? Alias => GetRed(ref alias, 3);

        public NameSyntax Name => GetRed(ref name, 4);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingDirectiveSyntax)base.Green).semicolonToken, GetChildPosition(5), GetChildIndex(5));

        public UsingDirectiveSyntax Update(SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken)
        {
            return Update(GlobalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken);
        }

        internal UsingDirectiveSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                3 => GetRed(ref alias, 3),
                4 => GetRed(ref name, 4),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                3 => alias,
                4 => name,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitUsingDirective(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUsingDirective(this);

        public UsingDirectiveSyntax Update(SyntaxToken globalKeyword, SyntaxToken usingKeyword, SyntaxToken staticKeyword, NameEqualsSyntax? alias, NameSyntax name, SyntaxToken semicolonToken)
        {
            if (globalKeyword != GlobalKeyword || usingKeyword != UsingKeyword || staticKeyword != StaticKeyword || alias != Alias || name != Name || semicolonToken != SemicolonToken)
            {
                UsingDirectiveSyntax usingDirectiveSyntax = SyntaxFactory.UsingDirective(globalKeyword, usingKeyword, staticKeyword, alias, name, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return usingDirectiveSyntax;
                }
                return usingDirectiveSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public UsingDirectiveSyntax WithGlobalKeyword(SyntaxToken globalKeyword)
        {
            return Update(globalKeyword, UsingKeyword, StaticKeyword, Alias, Name, SemicolonToken);
        }

        public UsingDirectiveSyntax WithUsingKeyword(SyntaxToken usingKeyword)
        {
            return Update(GlobalKeyword, usingKeyword, StaticKeyword, Alias, Name, SemicolonToken);
        }

        public UsingDirectiveSyntax WithStaticKeyword(SyntaxToken staticKeyword)
        {
            return Update(GlobalKeyword, UsingKeyword, staticKeyword, Alias, Name, SemicolonToken);
        }

        public UsingDirectiveSyntax WithAlias(NameEqualsSyntax? alias)
        {
            return Update(GlobalKeyword, UsingKeyword, StaticKeyword, alias, Name, SemicolonToken);
        }

        public UsingDirectiveSyntax WithName(NameSyntax name)
        {
            return Update(GlobalKeyword, UsingKeyword, StaticKeyword, Alias, name, SemicolonToken);
        }

        public UsingDirectiveSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(GlobalKeyword, UsingKeyword, StaticKeyword, Alias, Name, semicolonToken);
        }
    }
}
