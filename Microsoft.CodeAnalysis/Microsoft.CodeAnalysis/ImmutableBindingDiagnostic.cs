using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct ImmutableBindingDiagnostic<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        private readonly ImmutableArray<Diagnostic> _diagnostics;

        private readonly ImmutableArray<TAssemblySymbol> _dependencies;

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics.NullToEmpty();

        public ImmutableArray<TAssemblySymbol> Dependencies => _dependencies.NullToEmpty();

        public static ImmutableBindingDiagnostic<TAssemblySymbol> Empty => new ImmutableBindingDiagnostic<TAssemblySymbol>(default(ImmutableArray<Diagnostic>), default(ImmutableArray<TAssemblySymbol>));

        public ImmutableBindingDiagnostic(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<TAssemblySymbol> dependencies)
        {
            _diagnostics = diagnostics.NullToEmpty();
            _dependencies = dependencies.NullToEmpty();
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> NullToEmpty()
        {
            return new ImmutableBindingDiagnostic<TAssemblySymbol>(Diagnostics, Dependencies);
        }

        public static bool operator ==(ImmutableBindingDiagnostic<TAssemblySymbol> first, ImmutableBindingDiagnostic<TAssemblySymbol> second)
        {
            if (first.Diagnostics == second.Diagnostics)
            {
                return first.Dependencies == second.Dependencies;
            }
            return false;
        }

        public static bool operator !=(ImmutableBindingDiagnostic<TAssemblySymbol> first, ImmutableBindingDiagnostic<TAssemblySymbol> second)
        {
            return !(first == second);
        }

        public override bool Equals(object? obj)
        {
            return (obj as ImmutableBindingDiagnostic<TAssemblySymbol>?)?.Equals(this) ?? false;
        }

        public bool Equals(ImmutableBindingDiagnostic<TAssemblySymbol> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Diagnostics.GetHashCode();
        }
    }
}
