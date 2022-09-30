using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDynamicCollectionElementInitializer : BoundDynamicInvocableBase
    {
        public new TypeSymbol Type => base.Type;

        public ImmutableArray<MethodSymbol> ApplicableMethods { get; }

        public BoundDynamicCollectionElementInitializer(SyntaxNode syntax, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicCollectionElementInitializer, syntax, expression, arguments, type, hasErrors || expression.HasErrors() || arguments.HasErrors())
        {
            ApplicableMethods = applicableMethods;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDynamicCollectionElementInitializer(this);
        }

        public BoundDynamicCollectionElementInitializer Update(ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
        {
            if (applicableMethods != ApplicableMethods || expression != base.Expression || arguments != base.Arguments || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDynamicCollectionElementInitializer boundDynamicCollectionElementInitializer = new BoundDynamicCollectionElementInitializer(Syntax, applicableMethods, expression, arguments, type, base.HasErrors);
                boundDynamicCollectionElementInitializer.CopyAttributes(this);
                return boundDynamicCollectionElementInitializer;
            }
            return this;
        }
    }
}
