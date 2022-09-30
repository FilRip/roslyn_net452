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
    public sealed class FunctionPointerCallingConventionSyntax : CSharpSyntaxNode
    {
        private FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList;

        public SyntaxToken ManagedOrUnmanagedKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FunctionPointerCallingConventionSyntax)base.Green).managedOrUnmanagedKeyword, base.Position, 0);

        public FunctionPointerUnmanagedCallingConventionListSyntax? UnmanagedCallingConventionList => GetRed(ref unmanagedCallingConventionList, 1);

        internal FunctionPointerCallingConventionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref unmanagedCallingConventionList, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return unmanagedCallingConventionList;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerCallingConvention(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerCallingConvention(this);

        public FunctionPointerCallingConventionSyntax Update(SyntaxToken managedOrUnmanagedKeyword, FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
        {
            if (managedOrUnmanagedKeyword != ManagedOrUnmanagedKeyword || unmanagedCallingConventionList != UnmanagedCallingConventionList)
            {
                FunctionPointerCallingConventionSyntax functionPointerCallingConventionSyntax = SyntaxFactory.FunctionPointerCallingConvention(managedOrUnmanagedKeyword, unmanagedCallingConventionList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerCallingConventionSyntax;
                }
                return functionPointerCallingConventionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public FunctionPointerCallingConventionSyntax WithManagedOrUnmanagedKeyword(SyntaxToken managedOrUnmanagedKeyword)
        {
            return Update(managedOrUnmanagedKeyword, UnmanagedCallingConventionList);
        }

        public FunctionPointerCallingConventionSyntax WithUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax? unmanagedCallingConventionList)
        {
            return Update(ManagedOrUnmanagedKeyword, unmanagedCallingConventionList);
        }

        public FunctionPointerCallingConventionSyntax AddUnmanagedCallingConventionListCallingConventions(params FunctionPointerUnmanagedCallingConventionSyntax[] items)
        {
            FunctionPointerUnmanagedCallingConventionListSyntax functionPointerUnmanagedCallingConventionListSyntax = UnmanagedCallingConventionList ?? SyntaxFactory.FunctionPointerUnmanagedCallingConventionList();
            return WithUnmanagedCallingConventionList(functionPointerUnmanagedCallingConventionListSyntax.WithCallingConventions(functionPointerUnmanagedCallingConventionListSyntax.CallingConventions.AddRange(items)));
        }
    }
}
