#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FunctionPointerTypeSyntax : TypeSyntax
    {
        private FunctionPointerCallingConventionSyntax? callingConvention;

        private FunctionPointerParameterListSyntax? parameterList;

        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerTypeSyntax)base.Green).delegateKeyword, base.Position, 0);

        public SyntaxToken AsteriskToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerTypeSyntax)base.Green).asteriskToken, GetChildPosition(1), GetChildIndex(1));

        public FunctionPointerCallingConventionSyntax? CallingConvention => GetRed(ref callingConvention, 2);

        public FunctionPointerParameterListSyntax ParameterList => GetRed(ref parameterList, 3);

        internal FunctionPointerTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                2 => GetRed(ref callingConvention, 2),
                3 => GetRed(ref parameterList, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                2 => callingConvention,
                3 => parameterList,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerType(this);

        public FunctionPointerTypeSyntax Update(SyntaxToken delegateKeyword, SyntaxToken asteriskToken, FunctionPointerCallingConventionSyntax? callingConvention, FunctionPointerParameterListSyntax parameterList)
        {
            if (delegateKeyword != DelegateKeyword || asteriskToken != AsteriskToken || callingConvention != CallingConvention || parameterList != ParameterList)
            {
                FunctionPointerTypeSyntax functionPointerTypeSyntax = SyntaxFactory.FunctionPointerType(delegateKeyword, asteriskToken, callingConvention, parameterList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerTypeSyntax;
                }
                return functionPointerTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FunctionPointerTypeSyntax WithDelegateKeyword(SyntaxToken delegateKeyword)
        {
            return Update(delegateKeyword, AsteriskToken, CallingConvention, ParameterList);
        }

        public FunctionPointerTypeSyntax WithAsteriskToken(SyntaxToken asteriskToken)
        {
            return Update(DelegateKeyword, asteriskToken, CallingConvention, ParameterList);
        }

        public FunctionPointerTypeSyntax WithCallingConvention(FunctionPointerCallingConventionSyntax? callingConvention)
        {
            return Update(DelegateKeyword, AsteriskToken, callingConvention, ParameterList);
        }

        public FunctionPointerTypeSyntax WithParameterList(FunctionPointerParameterListSyntax parameterList)
        {
            return Update(DelegateKeyword, AsteriskToken, CallingConvention, parameterList);
        }

        public FunctionPointerTypeSyntax AddParameterListParameters(params FunctionPointerParameterSyntax[] items)
        {
            return WithParameterList(ParameterList.WithParameters(ParameterList.Parameters.AddRange(items)));
        }
    }
}
