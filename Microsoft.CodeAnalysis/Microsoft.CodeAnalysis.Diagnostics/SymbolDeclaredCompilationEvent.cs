using System;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class SymbolDeclaredCompilationEvent : CompilationEvent
    {
        private readonly Lazy<ImmutableArray<SyntaxReference>> _lazyCachedDeclaringReferences;

        public ISymbol Symbol { get; }

        public SemanticModel? SemanticModelWithCachedBoundNodes { get; }

        public ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _lazyCachedDeclaringReferences.Value;

        public SymbolDeclaredCompilationEvent(Compilation compilation, ISymbol symbol, SemanticModel? semanticModelWithCachedBoundNodes = null) : base(compilation)
        {
            ISymbol symbol2 = symbol;
            //base._002Ector(compilation);
            Symbol = symbol2;
            SemanticModelWithCachedBoundNodes = semanticModelWithCachedBoundNodes;
            _lazyCachedDeclaringReferences = new Lazy<ImmutableArray<SyntaxReference>>(() => symbol2.DeclaringSyntaxReferences);
        }

        public override string ToString()
        {
            string text = Symbol.Name;
            if (text == "")
            {
                text = "<empty>";
            }
            string text2 = ((DeclaringSyntaxReferences.Length != 0) ? (" @ " + string.Join(", ", Enumerable.Select(DeclaringSyntaxReferences, (SyntaxReference r) => r.GetLocation().GetLineSpan()))) : null);
            return "SymbolDeclaredCompilationEvent(" + text + " " + Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + text2 + ")";
        }
    }
}
