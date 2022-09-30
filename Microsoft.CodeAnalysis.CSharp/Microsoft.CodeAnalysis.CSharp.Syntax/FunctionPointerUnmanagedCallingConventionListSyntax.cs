#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FunctionPointerUnmanagedCallingConventionListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? callingConventions;

        public SyntaxToken OpenBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)base.Green).openBracketToken, base.Position, 0);

        public SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> CallingConventions
        {
            get
            {
                SyntaxNode red = GetRed(ref callingConventions, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax>);
                }
                return new SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseBracketToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionListSyntax)base.Green).closeBracketToken, GetChildPosition(2), GetChildIndex(2));

        internal FunctionPointerUnmanagedCallingConventionListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref callingConventions, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return callingConventions;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerUnmanagedCallingConventionList(this);

        public FunctionPointerUnmanagedCallingConventionListSyntax Update(SyntaxToken openBracketToken, SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions, SyntaxToken closeBracketToken)
        {
            if (openBracketToken != OpenBracketToken || callingConventions != CallingConventions || closeBracketToken != CloseBracketToken)
            {
                FunctionPointerUnmanagedCallingConventionListSyntax functionPointerUnmanagedCallingConventionListSyntax = SyntaxFactory.FunctionPointerUnmanagedCallingConventionList(openBracketToken, callingConventions, closeBracketToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerUnmanagedCallingConventionListSyntax;
                }
                return functionPointerUnmanagedCallingConventionListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return Update(openBracketToken, CallingConventions, CloseBracketToken);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax WithCallingConventions(SeparatedSyntaxList<FunctionPointerUnmanagedCallingConventionSyntax> callingConventions)
        {
            return Update(OpenBracketToken, callingConventions, CloseBracketToken);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return Update(OpenBracketToken, CallingConventions, closeBracketToken);
        }

        public FunctionPointerUnmanagedCallingConventionListSyntax AddCallingConventions(params FunctionPointerUnmanagedCallingConventionSyntax[] items)
        {
            return WithCallingConventions(CallingConventions.AddRange(items));
        }
    }
}
