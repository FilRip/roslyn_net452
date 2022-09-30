using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundModuleVersionId : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundModuleVersionId(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ModuleVersionId, syntax, type, hasErrors)
        {
        }

        public BoundModuleVersionId(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ModuleVersionId, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitModuleVersionId(this);
        }

        public BoundModuleVersionId Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundModuleVersionId boundModuleVersionId = new BoundModuleVersionId(Syntax, type, base.HasErrors);
                boundModuleVersionId.CopyAttributes(this);
                return boundModuleVersionId;
            }
            return this;
        }
    }
}
