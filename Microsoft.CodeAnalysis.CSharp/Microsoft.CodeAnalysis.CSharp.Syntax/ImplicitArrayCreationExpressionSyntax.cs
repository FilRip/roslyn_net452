#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ImplicitArrayCreationExpressionSyntax : ExpressionSyntax
    {
        private InitializerExpressionSyntax? initializer;

        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)base.Green).newKeyword, base.Position, 0);

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)base.Green).openBracketToken, GetChildPosition(1), GetChildIndex(1));

        public SyntaxTokenList Commas
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(2);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ImplicitArrayCreationExpressionSyntax)base.Green).closeBracketToken, GetChildPosition(3), GetChildIndex(3));

        public InitializerExpressionSyntax Initializer => GetRed(ref initializer, 4);

        internal ImplicitArrayCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 4)
            {
                return null;
            }
            return GetRed(ref initializer, 4);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 4)
            {
                return null;
            }
            return initializer;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitArrayCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitImplicitArrayCreationExpression(this);

        public ImplicitArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxToken openBracketToken, SyntaxTokenList commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (newKeyword != NewKeyword || openBracketToken != OpenBracketToken || commas != Commas || closeBracketToken != CloseBracketToken || initializer != Initializer)
            {
                ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax = SyntaxFactory.ImplicitArrayCreationExpression(newKeyword, openBracketToken, commas, closeBracketToken, initializer);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return implicitArrayCreationExpressionSyntax;
                }
                return implicitArrayCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ImplicitArrayCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return Update(newKeyword, OpenBracketToken, Commas, CloseBracketToken, Initializer);
        }

        public ImplicitArrayCreationExpressionSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(NewKeyword, openBracketToken, Commas, CloseBracketToken, Initializer);
        }

        public ImplicitArrayCreationExpressionSyntax WithCommas(SyntaxTokenList commas)
        {
            return Update(NewKeyword, OpenBracketToken, commas, CloseBracketToken, Initializer);
        }

        public ImplicitArrayCreationExpressionSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(NewKeyword, OpenBracketToken, Commas, closeBracketToken, Initializer);
        }

        public ImplicitArrayCreationExpressionSyntax WithInitializer(InitializerExpressionSyntax initializer)
        {
            return Update(NewKeyword, OpenBracketToken, Commas, CloseBracketToken, initializer);
        }

        public ImplicitArrayCreationExpressionSyntax AddCommas(params SyntaxToken[] items)
        {
            return WithCommas(Commas.AddRange(items));
        }

        public ImplicitArrayCreationExpressionSyntax AddInitializerExpressions(params ExpressionSyntax[] items)
        {
            return WithInitializer(Initializer.WithExpressions(Initializer.Expressions.AddRange(items)));
        }
    }
}
