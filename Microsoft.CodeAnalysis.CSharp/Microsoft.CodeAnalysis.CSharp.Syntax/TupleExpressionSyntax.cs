#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TupleExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? arguments;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleExpressionSyntax)base.Green).openParenToken, base.Position, 0);

        public SeparatedSyntaxList<ArgumentSyntax> Arguments
        {
            get
            {
                SyntaxNode red = GetRed(ref arguments, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ArgumentSyntax>);
                }
                return new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleExpressionSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal TupleExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref arguments, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return arguments;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTupleExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleExpression(this);

        public TupleExpressionSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || arguments != Arguments || closeParenToken != CloseParenToken)
            {
                TupleExpressionSyntax tupleExpressionSyntax = SyntaxFactory.TupleExpression(openParenToken, arguments, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return tupleExpressionSyntax;
                }
                return tupleExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TupleExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Arguments, CloseParenToken);
        }

        public TupleExpressionSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return Update(OpenParenToken, arguments, CloseParenToken);
        }

        public TupleExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Arguments, closeParenToken);
        }

        public TupleExpressionSyntax AddArguments(params ArgumentSyntax[] items)
        {
            return WithArguments(Arguments.AddRange(items));
        }
    }
}
