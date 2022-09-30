#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class ExpressionOrPatternSyntax : CSharpSyntaxNode
    {
        public ExpressionOrPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }
    }
}
