#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ConversionOperatorMemberCrefSyntax : MemberCrefSyntax
    {
        private TypeSyntax? type;

        private CrefParameterListSyntax? parameters;

        public SyntaxToken ImplicitOrExplicitKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConversionOperatorMemberCrefSyntax)base.Green).implicitOrExplicitKeyword, base.Position, 0);

        public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConversionOperatorMemberCrefSyntax)base.Green).operatorKeyword, GetChildPosition(1), GetChildIndex(1));

        public TypeSyntax Type => GetRed(ref type, 2);

        public CrefParameterListSyntax? Parameters => GetRed(ref parameters, 3);

        internal ConversionOperatorMemberCrefSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                2 => GetRed(ref type, 2),
                3 => GetRed(ref parameters, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                2 => type,
                3 => parameters,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConversionOperatorMemberCref(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConversionOperatorMemberCref(this);

        public ConversionOperatorMemberCrefSyntax Update(SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, CrefParameterListSyntax? parameters)
        {
            if (implicitOrExplicitKeyword != ImplicitOrExplicitKeyword || operatorKeyword != OperatorKeyword || type != Type || parameters != Parameters)
            {
                ConversionOperatorMemberCrefSyntax conversionOperatorMemberCrefSyntax = SyntaxFactory.ConversionOperatorMemberCref(implicitOrExplicitKeyword, operatorKeyword, type, parameters);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return conversionOperatorMemberCrefSyntax;
                }
                return conversionOperatorMemberCrefSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ConversionOperatorMemberCrefSyntax WithImplicitOrExplicitKeyword(SyntaxToken implicitOrExplicitKeyword)
        {
            return Update(implicitOrExplicitKeyword, OperatorKeyword, Type, Parameters);
        }

        public ConversionOperatorMemberCrefSyntax WithOperatorKeyword(SyntaxToken operatorKeyword)
        {
            return Update(ImplicitOrExplicitKeyword, operatorKeyword, Type, Parameters);
        }

        public ConversionOperatorMemberCrefSyntax WithType(TypeSyntax type)
        {
            return Update(ImplicitOrExplicitKeyword, OperatorKeyword, type, Parameters);
        }

        public ConversionOperatorMemberCrefSyntax WithParameters(CrefParameterListSyntax? parameters)
        {
            return Update(ImplicitOrExplicitKeyword, OperatorKeyword, Type, parameters);
        }

        public ConversionOperatorMemberCrefSyntax AddParametersParameters(params CrefParameterSyntax[] items)
        {
            CrefParameterListSyntax crefParameterListSyntax = Parameters ?? SyntaxFactory.CrefParameterList();
            return WithParameters(crefParameterListSyntax.WithParameters(crefParameterListSyntax.Parameters.AddRange(items)));
        }
    }
}
