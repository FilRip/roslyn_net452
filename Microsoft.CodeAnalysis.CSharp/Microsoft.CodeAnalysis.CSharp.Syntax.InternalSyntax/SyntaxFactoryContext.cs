namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public class SyntaxFactoryContext
    {
        public bool IsInAsync;

        public int QueryDepth;

        public bool IsInQuery => QueryDepth > 0;
    }
}
