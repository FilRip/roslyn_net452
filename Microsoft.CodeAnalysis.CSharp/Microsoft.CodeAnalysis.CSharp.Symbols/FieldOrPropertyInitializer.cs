namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public struct FieldOrPropertyInitializer
    {
        internal readonly FieldSymbol FieldOpt;

        internal readonly SyntaxReference Syntax;

        public FieldOrPropertyInitializer(FieldSymbol fieldOpt, SyntaxNode syntax)
        {
            FieldOpt = fieldOpt;
            Syntax = syntax.GetReference();
        }
    }
}
