#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CatchDeclarationSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchDeclarationSyntax)base.Green).openParenToken, base.Position, 0);

        public TypeSyntax Type => GetRed(ref type, 1);

        public SyntaxToken Identifier
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken identifier = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchDeclarationSyntax)base.Green).identifier;
                if (identifier == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, identifier, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchDeclarationSyntax)base.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        internal CatchDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref type, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return type;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCatchDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchDeclaration(this);

        public CatchDeclarationSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || type != Type || identifier != Identifier || closeParenToken != CloseParenToken)
            {
                CatchDeclarationSyntax catchDeclarationSyntax = SyntaxFactory.CatchDeclaration(openParenToken, type, identifier, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return catchDeclarationSyntax;
                }
                return catchDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CatchDeclarationSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Type, Identifier, CloseParenToken);
        }

        public CatchDeclarationSyntax WithType(TypeSyntax type)
        {
            return Update(OpenParenToken, type, Identifier, CloseParenToken);
        }

        public CatchDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(OpenParenToken, Type, identifier, CloseParenToken);
        }

        public CatchDeclarationSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Type, Identifier, closeParenToken);
        }
    }
}
