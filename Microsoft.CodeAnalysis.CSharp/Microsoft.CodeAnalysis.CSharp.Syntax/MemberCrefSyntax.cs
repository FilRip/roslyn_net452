#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class MemberCrefSyntax : CrefSyntax
    {
        internal MemberCrefSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }
    }
}
