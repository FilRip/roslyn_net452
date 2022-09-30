#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ParameterListSyntax : BaseParameterListSyntax
    {
        private SyntaxNode? parameters;

        internal int ParameterCount
        {
            get
            {
                int num = 0;
                SeparatedSyntaxList<ParameterSyntax>.Enumerator enumerator = Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.IsArgList)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)base.Green).openParenToken, base.Position, 0);

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

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal ParameterListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitParameterList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParameterList(this);

        public ParameterListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || parameters != Parameters || closeParenToken != CloseParenToken)
            {
                ParameterListSyntax parameterListSyntax = SyntaxFactory.ParameterList(openParenToken, parameters, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return parameterListSyntax;
                }
                return parameterListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ParameterListSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Parameters, CloseParenToken);
        }

        internal override BaseParameterListSyntax WithParametersCore(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            return WithParameters(parameters);
        }

        public new ParameterListSyntax WithParameters(SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            return Update(OpenParenToken, parameters, CloseParenToken);
        }

        public ParameterListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Parameters, closeParenToken);
        }

        internal override BaseParameterListSyntax AddParametersCore(params ParameterSyntax[] items)
        {
            return AddParameters(items);
        }

        public new ParameterListSyntax AddParameters(params ParameterSyntax[] items)
        {
            return WithParameters(Parameters.AddRange(items));
        }
    }
}
