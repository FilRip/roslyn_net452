using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundModuleVersionIdString : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ModuleVersionIdString, syntax, type, hasErrors)
        {
        }

        public BoundModuleVersionIdString(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.ModuleVersionIdString, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitModuleVersionIdString(this);
        }

        public BoundModuleVersionIdString Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundModuleVersionIdString boundModuleVersionIdString = new BoundModuleVersionIdString(Syntax, type, base.HasErrors);
                boundModuleVersionIdString.CopyAttributes(this);
                return boundModuleVersionIdString;
            }
            return this;
        }
    }
}
