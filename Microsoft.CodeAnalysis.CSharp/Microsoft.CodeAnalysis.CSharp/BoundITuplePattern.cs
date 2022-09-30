using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundITuplePattern : BoundPattern
    {
        public MethodSymbol GetLengthMethod { get; }

        public MethodSymbol GetItemMethod { get; }

        public ImmutableArray<BoundSubpattern> Subpatterns { get; }

        public BoundITuplePattern(SyntaxNode syntax, MethodSymbol getLengthMethod, MethodSymbol getItemMethod, ImmutableArray<BoundSubpattern> subpatterns, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors = false)
            : base(BoundKind.ITuplePattern, syntax, inputType, narrowedType, hasErrors || subpatterns.HasErrors())
        {
            GetLengthMethod = getLengthMethod;
            GetItemMethod = getItemMethod;
            Subpatterns = subpatterns;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitITuplePattern(this);
        }

        public BoundITuplePattern Update(MethodSymbol getLengthMethod, MethodSymbol getItemMethod, ImmutableArray<BoundSubpattern> subpatterns, TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getLengthMethod, GetLengthMethod) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getItemMethod, GetItemMethod) || subpatterns != Subpatterns || !TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundITuplePattern boundITuplePattern = new BoundITuplePattern(Syntax, getLengthMethod, getItemMethod, subpatterns, inputType, narrowedType, base.HasErrors);
                boundITuplePattern.CopyAttributes(this);
                return boundITuplePattern;
            }
            return this;
        }
    }
}
