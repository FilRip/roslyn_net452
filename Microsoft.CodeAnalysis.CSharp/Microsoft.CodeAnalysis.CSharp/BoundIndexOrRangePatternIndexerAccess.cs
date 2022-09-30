using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIndexOrRangePatternIndexerAccess : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create(Receiver, (BoundNode)Argument);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public PropertySymbol LengthOrCountProperty { get; }

        public Symbol PatternSymbol { get; }

        public BoundExpression Argument { get; }

        public BoundIndexOrRangePatternIndexerAccess(SyntaxNode syntax, BoundExpression receiver, PropertySymbol lengthOrCountProperty, Symbol patternSymbol, BoundExpression argument, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.IndexOrRangePatternIndexerAccess, syntax, type, hasErrors || receiver.HasErrors() || argument.HasErrors())
        {
            Receiver = receiver;
            LengthOrCountProperty = lengthOrCountProperty;
            PatternSymbol = patternSymbol;
            Argument = argument;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIndexOrRangePatternIndexerAccess(this);
        }

        public BoundIndexOrRangePatternIndexerAccess Update(BoundExpression receiver, PropertySymbol lengthOrCountProperty, Symbol patternSymbol, BoundExpression argument, TypeSymbol type)
        {
            if (receiver != Receiver || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(lengthOrCountProperty, LengthOrCountProperty) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(patternSymbol, PatternSymbol) || argument != Argument || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundIndexOrRangePatternIndexerAccess boundIndexOrRangePatternIndexerAccess = new BoundIndexOrRangePatternIndexerAccess(Syntax, receiver, lengthOrCountProperty, patternSymbol, argument, type, base.HasErrors);
                boundIndexOrRangePatternIndexerAccess.CopyAttributes(this);
                return boundIndexOrRangePatternIndexerAccess;
            }
            return this;
        }
    }
}
