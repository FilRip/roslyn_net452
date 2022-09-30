using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMultipleLocalDeclarations : BoundMultipleLocalDeclarationsBase
    {
        public BoundMultipleLocalDeclarations(SyntaxNode syntax, ImmutableArray<BoundLocalDeclaration> localDeclarations, bool hasErrors = false)
            : base(BoundKind.MultipleLocalDeclarations, syntax, localDeclarations, hasErrors || localDeclarations.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMultipleLocalDeclarations(this);
        }

        public BoundMultipleLocalDeclarations Update(ImmutableArray<BoundLocalDeclaration> localDeclarations)
        {
            if (localDeclarations != base.LocalDeclarations)
            {
                BoundMultipleLocalDeclarations boundMultipleLocalDeclarations = new BoundMultipleLocalDeclarations(Syntax, localDeclarations, base.HasErrors);
                boundMultipleLocalDeclarations.CopyAttributes(this);
                return boundMultipleLocalDeclarations;
            }
            return this;
        }
    }
}
