#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FunctionPointerParameterListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? parameters;

        public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerParameterListSyntax)base.Green).lessThanToken, base.Position, 0);

        public SeparatedSyntaxList<FunctionPointerParameterSyntax> Parameters
        {
            get
            {
                SyntaxNode red = GetRed(ref parameters, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<FunctionPointerParameterSyntax>);
                }
                return new SeparatedSyntaxList<FunctionPointerParameterSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerParameterListSyntax)base.Green).greaterThanToken, GetChildPosition(2), GetChildIndex(2));

        internal FunctionPointerParameterListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref parameters, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return parameters;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerParameterList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerParameterList(this);

        public FunctionPointerParameterListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken != LessThanToken || parameters != Parameters || greaterThanToken != GreaterThanToken)
            {
                FunctionPointerParameterListSyntax functionPointerParameterListSyntax = SyntaxFactory.FunctionPointerParameterList(lessThanToken, parameters, greaterThanToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerParameterListSyntax;
                }
                return functionPointerParameterListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FunctionPointerParameterListSyntax WithLessThanToken(SyntaxToken lessThanToken)
        {
            return Update(lessThanToken, Parameters, GreaterThanToken);
        }

        public FunctionPointerParameterListSyntax WithParameters(SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters)
        {
            return Update(LessThanToken, parameters, GreaterThanToken);
        }

        public FunctionPointerParameterListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
        {
            return Update(LessThanToken, Parameters, greaterThanToken);
        }

        public FunctionPointerParameterListSyntax AddParameters(params FunctionPointerParameterSyntax[] items)
        {
            return WithParameters(Parameters.AddRange(items));
        }
    }
}
