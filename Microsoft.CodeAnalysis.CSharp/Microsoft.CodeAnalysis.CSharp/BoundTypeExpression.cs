using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTypeExpression : BoundExpression
    {
        public override Symbol ExpressionSymbol => (Symbol)(AliasOpt ?? ((object)Type));

        public override LookupResultKind ResultKind
        {
            get
            {
                if (Type.OriginalDefinition is ErrorTypeSymbol errorTypeSymbol)
                {
                    return errorTypeSymbol.ResultKind;
                }
                return LookupResultKind.Viable;
            }
        }

        public AliasSymbol? AliasOpt { get; }

        public BoundTypeExpression? BoundContainingTypeOpt { get; }

        public ImmutableArray<BoundExpression> BoundDimensionsOpt { get; }

        public new TypeSymbol Type => base.Type;

        public TypeWithAnnotations TypeWithAnnotations { get; }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, ImmutableArray<BoundExpression> boundDimensionsOpt, TypeWithAnnotations typeWithAnnotations, bool hasErrors = false)
            : this(syntax, aliasOpt, boundContainingTypeOpt, boundDimensionsOpt, typeWithAnnotations, typeWithAnnotations.Type, hasErrors)
        {
        }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, TypeWithAnnotations typeWithAnnotations, bool hasErrors = false)
            : this(syntax, aliasOpt, boundContainingTypeOpt, ImmutableArray<BoundExpression>.Empty, typeWithAnnotations, hasErrors)
        {
        }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, TypeWithAnnotations typeWithAnnotations, bool hasErrors = false)
            : this(syntax, aliasOpt, null, typeWithAnnotations, hasErrors)
        {
        }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, aliasOpt, null, TypeWithAnnotations.Create(type), hasErrors)
        {
        }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, ImmutableArray<BoundExpression> dimensionsOpt, TypeWithAnnotations typeWithAnnotations, bool hasErrors = false)
            : this(syntax, aliasOpt, null, dimensionsOpt, typeWithAnnotations, hasErrors)
        {
        }

        public BoundTypeExpression(SyntaxNode syntax, AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, ImmutableArray<BoundExpression> boundDimensionsOpt, TypeWithAnnotations typeWithAnnotations, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TypeExpression, syntax, type, hasErrors || boundContainingTypeOpt.HasErrors() || boundDimensionsOpt.HasErrors())
        {
            AliasOpt = aliasOpt;
            BoundContainingTypeOpt = boundContainingTypeOpt;
            BoundDimensionsOpt = boundDimensionsOpt;
            TypeWithAnnotations = typeWithAnnotations;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTypeExpression(this);
        }

        public BoundTypeExpression Update(AliasSymbol? aliasOpt, BoundTypeExpression? boundContainingTypeOpt, ImmutableArray<BoundExpression> boundDimensionsOpt, TypeWithAnnotations typeWithAnnotations, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(aliasOpt, AliasOpt) || boundContainingTypeOpt != BoundContainingTypeOpt || boundDimensionsOpt != BoundDimensionsOpt || typeWithAnnotations != TypeWithAnnotations || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTypeExpression boundTypeExpression = new BoundTypeExpression(Syntax, aliasOpt, boundContainingTypeOpt, boundDimensionsOpt, typeWithAnnotations, type, base.HasErrors);
                boundTypeExpression.CopyAttributes(this);
                return boundTypeExpression;
            }
            return this;
        }
    }
}
