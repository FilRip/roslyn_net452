#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class TypeSyntax : ExpressionSyntax
    {
        public bool IsVar => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)base.Green).IsVar;

        public bool IsUnmanaged => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)base.Green).IsUnmanaged;

        public bool IsNotNull => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)base.Green).IsNotNull;

        public bool IsNint => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)base.Green).IsNint;

        public bool IsNuint => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeSyntax)base.Green).IsNuint;

        public TypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }
    }
}
