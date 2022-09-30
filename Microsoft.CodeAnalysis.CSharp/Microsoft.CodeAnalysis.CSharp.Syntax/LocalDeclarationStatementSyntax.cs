#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LocalDeclarationStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private VariableDeclarationSyntax? declaration;

        public bool IsConst => Modifiers.Any(SyntaxKind.ConstKeyword);

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken AwaitKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken awaitKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)base.Green).awaitKeyword;
                if (awaitKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, awaitKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken UsingKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken usingKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)base.Green).usingKeyword;
                if (usingKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, usingKeyword, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(3);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(3), GetChildIndex(3));
            }
        }

        public VariableDeclarationSyntax Declaration => GetRed(ref declaration, 4);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LocalDeclarationStatementSyntax)base.Green).semicolonToken, GetChildPosition(5), GetChildIndex(5));

        public LocalDeclarationStatementSyntax Update(SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            return Update(AwaitKeyword, UsingKeyword, modifiers, declaration, semicolonToken);
        }

        public LocalDeclarationStatementSyntax Update(SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken);
        }

        internal LocalDeclarationStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                4 => GetRed(ref declaration, 4),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                4 => declaration,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLocalDeclarationStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLocalDeclarationStatement(this);

        public LocalDeclarationStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxTokenList modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || usingKeyword != UsingKeyword || modifiers != Modifiers || declaration != Declaration || semicolonToken != SemicolonToken)
            {
                LocalDeclarationStatementSyntax localDeclarationStatementSyntax = SyntaxFactory.LocalDeclarationStatement(attributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return localDeclarationStatementSyntax;
                }
                return localDeclarationStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new LocalDeclarationStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, AwaitKeyword, UsingKeyword, Modifiers, Declaration, SemicolonToken);
        }

        public LocalDeclarationStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return Update(AttributeLists, awaitKeyword, UsingKeyword, Modifiers, Declaration, SemicolonToken);
        }

        public LocalDeclarationStatementSyntax WithUsingKeyword(SyntaxToken usingKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, usingKeyword, Modifiers, Declaration, SemicolonToken);
        }

        public LocalDeclarationStatementSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, modifiers, Declaration, SemicolonToken);
        }

        public LocalDeclarationStatementSyntax WithDeclaration(VariableDeclarationSyntax declaration)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, Modifiers, declaration, SemicolonToken);
        }

        public LocalDeclarationStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, Modifiers, Declaration, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new LocalDeclarationStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        public LocalDeclarationStatementSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public LocalDeclarationStatementSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items)
        {
            return WithDeclaration(Declaration.WithVariables(Declaration.Variables.AddRange(items)));
        }
    }
}
