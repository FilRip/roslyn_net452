using System.ComponentModel;

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
    public sealed class ArgumentSyntax : CSharpSyntaxNode
    {
        private NameColonSyntax? nameColon;

        private ExpressionSyntax? expression;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public SyntaxToken RefOrOutKeyword => RefKindKeyword;

        public NameColonSyntax? NameColon => GetRedAtZero(ref nameColon);

        public SyntaxToken RefKindKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken refKindKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ArgumentSyntax)base.Green).refKindKeyword;
                if (refKindKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, refKindKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public ExpressionSyntax Expression => GetRed(ref expression, 2);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ArgumentSyntax WithRefOrOutKeyword(SyntaxToken refOrOutKeyword)
        {
            return Update(NameColon, refOrOutKeyword, Expression);
        }

        internal ArgumentSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref nameColon),
                2 => GetRed(ref expression, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => nameColon,
                2 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArgument(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitArgument(this);

        public ArgumentSyntax Update(NameColonSyntax? nameColon, SyntaxToken refKindKeyword, ExpressionSyntax expression)
        {
            if (nameColon != NameColon || refKindKeyword != RefKindKeyword || expression != Expression)
            {
                ArgumentSyntax argumentSyntax = SyntaxFactory.Argument(nameColon, refKindKeyword, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return argumentSyntax;
                }
                return argumentSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ArgumentSyntax WithNameColon(NameColonSyntax? nameColon)
        {
            return Update(nameColon, RefKindKeyword, Expression);
        }

        public ArgumentSyntax WithRefKindKeyword(SyntaxToken refKindKeyword)
        {
            return Update(NameColon, refKindKeyword, Expression);
        }

        public ArgumentSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(NameColon, RefKindKeyword, expression);
        }
    }
}
