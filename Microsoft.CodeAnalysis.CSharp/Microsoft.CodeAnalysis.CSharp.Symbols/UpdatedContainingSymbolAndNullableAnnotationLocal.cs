using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class UpdatedContainingSymbolAndNullableAnnotationLocal : LocalSymbol
    {
        private readonly SourceLocalSymbol _underlyingLocal;

        public override Symbol ContainingSymbol { get; }

        public override TypeWithAnnotations TypeWithAnnotations { get; }

        public override RefKind RefKind => _underlyingLocal.RefKind;

        public override ImmutableArray<Location> Locations => _underlyingLocal.Locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingLocal.DeclaringSyntaxReferences;

        public override string Name => _underlyingLocal.Name;

        public override bool IsImplicitlyDeclared => _underlyingLocal.IsImplicitlyDeclared;

        internal override LocalDeclarationKind DeclarationKind => _underlyingLocal.DeclarationKind;

        internal override SynthesizedLocalKind SynthesizedKind => _underlyingLocal.SynthesizedKind;

        internal override SyntaxNode ScopeDesignatorOpt => _underlyingLocal.ScopeDesignatorOpt;

        internal override bool IsImportedFromMetadata => _underlyingLocal.IsImportedFromMetadata;

        internal override SyntaxToken IdentifierToken => _underlyingLocal.IdentifierToken;

        internal override bool IsPinned => _underlyingLocal.IsPinned;

        internal override bool IsCompilerGenerated => _underlyingLocal.IsCompilerGenerated;

        internal override uint RefEscapeScope => _underlyingLocal.RefEscapeScope;

        internal override uint ValEscapeScope => _underlyingLocal.ValEscapeScope;

        internal static UpdatedContainingSymbolAndNullableAnnotationLocal CreateForTest(SourceLocalSymbol underlyingLocal, Symbol updatedContainingSymbol, TypeWithAnnotations updatedType)
        {
            return new UpdatedContainingSymbolAndNullableAnnotationLocal(underlyingLocal, updatedContainingSymbol, updatedType, assertContaining: false);
        }

        private UpdatedContainingSymbolAndNullableAnnotationLocal(SourceLocalSymbol underlyingLocal, Symbol updatedContainingSymbol, TypeWithAnnotations updatedType, bool assertContaining)
        {
            ContainingSymbol = updatedContainingSymbol;
            TypeWithAnnotations = updatedType;
            _underlyingLocal = underlyingLocal;
        }

        internal UpdatedContainingSymbolAndNullableAnnotationLocal(SourceLocalSymbol underlyingLocal, Symbol updatedContainingSymbol, TypeWithAnnotations updatedType)
            : this(underlyingLocal, updatedContainingSymbol, updatedType, assertContaining: true)
        {
        }

        public override bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            if ((object)other == this)
            {
                return true;
            }
            if (!(other is LocalSymbol localSymbol))
            {
                return false;
            }
            SourceLocalSymbol sourceLocalSymbol2 = ((localSymbol is UpdatedContainingSymbolAndNullableAnnotationLocal updatedContainingSymbolAndNullableAnnotationLocal) ? updatedContainingSymbolAndNullableAnnotationLocal._underlyingLocal : ((!(localSymbol is SourceLocalSymbol sourceLocalSymbol)) ? null : sourceLocalSymbol));
            SourceLocalSymbol sourceLocalSymbol3 = sourceLocalSymbol2;
            if ((object)sourceLocalSymbol3 == null || !_underlyingLocal.Equals(sourceLocalSymbol3, compareKind))
            {
                return false;
            }
            if ((compareKind & TypeCompareKind.AllNullableIgnoreOptions) == 0)
            {
                if (TypeWithAnnotations.Equals(localSymbol.TypeWithAnnotations, compareKind))
                {
                    return ContainingSymbol.Equals(localSymbol.ContainingSymbol, compareKind);
                }
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return _underlyingLocal.GetHashCode();
        }

        internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag? diagnostics = null)
        {
            return _underlyingLocal.GetConstantValue(node, inProgress, diagnostics);
        }

        internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue)
        {
            return _underlyingLocal.GetConstantValueDiagnostics(boundInitValue);
        }

        internal override SyntaxNode GetDeclaratorSyntax()
        {
            return _underlyingLocal.GetDeclaratorSyntax();
        }

        internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
