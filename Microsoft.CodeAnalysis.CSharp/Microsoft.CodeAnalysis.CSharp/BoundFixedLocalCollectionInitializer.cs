using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFixedLocalCollectionInitializer : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Expression);

        public new TypeSymbol Type => base.Type;

        public TypeSymbol ElementPointerType { get; }

        public Conversion ElementPointerTypeConversion { get; }

        public BoundExpression Expression { get; }

        public MethodSymbol? GetPinnableOpt { get; }

        public BoundFixedLocalCollectionInitializer(SyntaxNode syntax, TypeSymbol elementPointerType, Conversion elementPointerTypeConversion, BoundExpression expression, MethodSymbol? getPinnableOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FixedLocalCollectionInitializer, syntax, type, hasErrors || expression.HasErrors())
        {
            ElementPointerType = elementPointerType;
            ElementPointerTypeConversion = elementPointerTypeConversion;
            Expression = expression;
            GetPinnableOpt = getPinnableOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFixedLocalCollectionInitializer(this);
        }

        public BoundFixedLocalCollectionInitializer Update(TypeSymbol elementPointerType, Conversion elementPointerTypeConversion, BoundExpression expression, MethodSymbol? getPinnableOpt, TypeSymbol type)
        {
            if (!TypeSymbol.Equals(elementPointerType, ElementPointerType, TypeCompareKind.ConsiderEverything) || elementPointerTypeConversion != ElementPointerTypeConversion || expression != Expression || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getPinnableOpt, GetPinnableOpt) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundFixedLocalCollectionInitializer boundFixedLocalCollectionInitializer = new BoundFixedLocalCollectionInitializer(Syntax, elementPointerType, elementPointerTypeConversion, expression, getPinnableOpt, type, base.HasErrors);
                boundFixedLocalCollectionInitializer.CopyAttributes(this);
                return boundFixedLocalCollectionInitializer;
            }
            return this;
        }
    }
}
