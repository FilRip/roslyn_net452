#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class VariableDeclaratorSyntax : CSharpSyntaxNode
    {
        private BracketedArgumentListSyntax? argumentList;

        private EqualsValueClauseSyntax? initializer;

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclaratorSyntax)base.Green).identifier, base.Position, 0);

        public BracketedArgumentListSyntax? ArgumentList => GetRed(ref argumentList, 1);

        public EqualsValueClauseSyntax? Initializer => GetRed(ref initializer, 2);

        internal VariableDeclaratorSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref argumentList, 1),
                2 => GetRed(ref initializer, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => argumentList,
                2 => initializer,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitVariableDeclarator(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitVariableDeclarator(this);

        public VariableDeclaratorSyntax Update(SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer)
        {
            if (identifier != Identifier || argumentList != ArgumentList || initializer != Initializer)
            {
                VariableDeclaratorSyntax variableDeclaratorSyntax = SyntaxFactory.VariableDeclarator(identifier, argumentList, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return variableDeclaratorSyntax;
                }
                return variableDeclaratorSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public VariableDeclaratorSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(identifier, ArgumentList, Initializer);
        }

        public VariableDeclaratorSyntax WithArgumentList(BracketedArgumentListSyntax? argumentList)
        {
            return Update(Identifier, argumentList, Initializer);
        }

        public VariableDeclaratorSyntax WithInitializer(EqualsValueClauseSyntax? initializer)
        {
            return Update(Identifier, ArgumentList, initializer);
        }

        public VariableDeclaratorSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            BracketedArgumentListSyntax bracketedArgumentListSyntax = ArgumentList ?? SyntaxFactory.BracketedArgumentList();
            return WithArgumentList(bracketedArgumentListSyntax.WithArguments(bracketedArgumentListSyntax.Arguments.AddRange(items)));
        }
    }
}
