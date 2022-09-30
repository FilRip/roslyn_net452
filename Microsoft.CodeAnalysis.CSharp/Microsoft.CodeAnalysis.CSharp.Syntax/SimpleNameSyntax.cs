#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        public abstract SyntaxToken Identifier { get; }

        internal sealed override SimpleNameSyntax GetUnqualifiedName()
        {
            return this;
        }

        internal SimpleNameSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public SimpleNameSyntax WithIdentifier(SyntaxToken identifier)
        {
            return WithIdentifierCore(identifier);
        }

        internal abstract SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier);
    }
}
