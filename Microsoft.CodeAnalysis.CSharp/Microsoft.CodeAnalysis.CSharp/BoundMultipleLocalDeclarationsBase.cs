using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundMultipleLocalDeclarationsBase : BoundStatement
    {
        public ImmutableArray<BoundLocalDeclaration> LocalDeclarations { get; }

        protected BoundMultipleLocalDeclarationsBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(kind, syntax, hasErrors)
        {
            LocalDeclarations = localDeclarations;
        }
    }
}
