#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FunctionPointerUnmanagedCallingConventionSyntax : CSharpSyntaxNode
    {
        public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerUnmanagedCallingConventionSyntax)base.Green).name, base.Position, 0);

        internal FunctionPointerUnmanagedCallingConventionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerUnmanagedCallingConvention(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerUnmanagedCallingConvention(this);

        public FunctionPointerUnmanagedCallingConventionSyntax Update(SyntaxToken name)
        {
            if (name != Name)
            {
                FunctionPointerUnmanagedCallingConventionSyntax functionPointerUnmanagedCallingConventionSyntax = SyntaxFactory.FunctionPointerUnmanagedCallingConvention(name);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerUnmanagedCallingConventionSyntax;
                }
                return functionPointerUnmanagedCallingConventionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FunctionPointerUnmanagedCallingConventionSyntax WithName(SyntaxToken name)
        {
            return Update(name);
        }
    }
}
