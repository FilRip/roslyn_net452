using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SynthesizedMetadataCompiler : CSharpSymbolVisitor
    {
        private readonly PEModuleBuilder _moduleBeingBuilt;

        private readonly CancellationToken _cancellationToken;

        private SynthesizedMetadataCompiler(PEModuleBuilder moduleBeingBuilt, CancellationToken cancellationToken)
        {
            _moduleBeingBuilt = moduleBeingBuilt;
            _cancellationToken = cancellationToken;
        }

        public static void ProcessSynthesizedMembers(CSharpCompilation compilation, PEModuleBuilder moduleBeingBuilt, CancellationToken cancellationToken)
        {
            new SynthesizedMetadataCompiler(moduleBeingBuilt, cancellationToken).Visit(compilation.SourceModule.GlobalNamespace);
        }

        public override void VisitNamespace(NamespaceSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Accept(this);
            }
        }

        public override void VisitNamedType(NamedTypeSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (symbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol && _moduleBeingBuilt != null)
            {
                ImmutableArray<SynthesizedExplicitImplementationForwardingMethod>.Enumerator enumerator = sourceMemberContainerTypeSymbol.GetSynthesizedExplicitImplementations(_cancellationToken).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SynthesizedExplicitImplementationForwardingMethod current = enumerator.Current;
                    _moduleBeingBuilt.AddSynthesizedDefinition(symbol, current.GetCciAdapter());
                }
            }
            ImmutableArray<Symbol>.Enumerator enumerator2 = symbol.GetMembers().GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Symbol current2 = enumerator2.Current;
                SymbolKind kind = current2.Kind;
                if (kind == SymbolKind.NamedType || kind == SymbolKind.Property)
                {
                    current2.Accept(this);
                }
            }
        }

        public override void VisitProperty(PropertySymbol symbol)
        {
            if (symbol is SourcePropertySymbolBase sourcePropertySymbolBase && sourcePropertySymbolBase.IsSealed)
            {
                SynthesizedSealedPropertyAccessor synthesizedSealedAccessorOpt = sourcePropertySymbolBase.SynthesizedSealedAccessorOpt;
                if ((object)synthesizedSealedAccessorOpt != null)
                {
                    _moduleBeingBuilt.AddSynthesizedDefinition(sourcePropertySymbolBase.ContainingType, synthesizedSealedAccessorOpt.GetCciAdapter());
                }
            }
        }
    }
}
