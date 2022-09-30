#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class SelectOrGroupClauseSyntax : CSharpSyntaxNode
    {
        internal SelectOrGroupClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }
    }
}
