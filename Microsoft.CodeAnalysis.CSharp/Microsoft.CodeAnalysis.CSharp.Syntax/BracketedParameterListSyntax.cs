#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BracketedParameterListSyntax : BaseParameterListSyntax
    {
        private SyntaxNode? parameters;

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedParameterListSyntax)base.Green).openBracketToken, base.Position, 0);

        public override SeparatedSyntaxList<ParameterSyntax> Parameters
        {
            get
            {
                SyntaxNode red = GetRed(ref parameters, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<ParameterSyntax>);
                }
                return new SeparatedSyntaxList<ParameterSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BracketedParameterListSyntax)base.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        internal BracketedParameterListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitBracketedParameterList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBracketedParameterList(this);

        public BracketedParameterListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != OpenBracketToken || parameters != Parameters || closeBracketToken != CloseBracketToken)
            {
                BracketedParameterListSyntax bracketedParameterListSyntax = SyntaxFactory.BracketedParameterList(openBracketToken, parameters, closeBracketToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return bracketedParameterListSyntax;
                }
                return bracketedParameterListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BracketedParameterListSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(openBracketToken, Parameters, CloseBracketToken);
        }

        internal override BaseParameterListSyntax WithParametersCore(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            return WithParameters(parameters);
        }

        public new BracketedParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            return Update(OpenBracketToken, parameters, CloseBracketToken);
        }

        public BracketedParameterListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(OpenBracketToken, Parameters, closeBracketToken);
        }

        internal override BaseParameterListSyntax AddParametersCore(params ParameterSyntax[] items)
        {
            return AddParameters(items);
        }

        public new BracketedParameterListSyntax AddParameters(params ParameterSyntax[] items)
        {
            return WithParameters(Parameters.AddRange(items));
        }
    }
}
