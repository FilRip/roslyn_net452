using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConvertedStackAllocExpression : BoundStackAllocArrayCreationBase
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(BoundStackAllocArrayCreationBase.GetChildInitializers(base.InitializerOpt).Insert(0, base.Count));

        public new TypeSymbol Type => base.Type;

        public BoundConvertedStackAllocExpression(SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ConvertedStackAllocExpression, syntax, elementType, count, initializerOpt, type, hasErrors || count.HasErrors() || initializerOpt.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConvertedStackAllocExpression(this);
        }

        public BoundConvertedStackAllocExpression Update(TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(elementType, base.ElementType, TypeCompareKind.ConsiderEverything) || count != base.Count || initializerOpt != base.InitializerOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundConvertedStackAllocExpression boundConvertedStackAllocExpression = new BoundConvertedStackAllocExpression(Syntax, elementType, count, initializerOpt, type, base.HasErrors);
                boundConvertedStackAllocExpression.CopyAttributes(this);
                return boundConvertedStackAllocExpression;
            }
            return this;
        }
    }
}
