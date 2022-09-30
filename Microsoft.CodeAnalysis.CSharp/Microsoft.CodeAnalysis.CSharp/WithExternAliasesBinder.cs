using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class WithExternAliasesBinder : Binder
    {
        private sealed class FromSyntax : WithExternAliasesBinder
        {
            private readonly SourceNamespaceSymbol _declaringSymbol;

            private readonly CSharpSyntaxNode _declarationSyntax;

            private ImmutableArray<AliasAndExternAliasDirective> _lazyExternAliases;

            internal override ImmutableArray<AliasAndExternAliasDirective> ExternAliases
            {
                get
                {
                    if (_lazyExternAliases.IsDefault)
                    {
                        ImmutableInterlocked.InterlockedInitialize(ref _lazyExternAliases, _declaringSymbol.GetExternAliases(_declarationSyntax));
                    }
                    return _lazyExternAliases;
                }
            }

            internal FromSyntax(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next)
                : base(next)
            {
                _declaringSymbol = declaringSymbol;
                _declarationSyntax = declarationSyntax;
            }
        }

        private sealed class FromSymbols : WithExternAliasesBinder
        {
            private readonly ImmutableArray<AliasAndExternAliasDirective> _externAliases;

            internal override ImmutableArray<AliasAndExternAliasDirective> ExternAliases => _externAliases;

            internal FromSymbols(ImmutableArray<AliasAndExternAliasDirective> externAliases, Binder next)
                : base(next)
            {
                _externAliases = externAliases;
            }
        }

        internal abstract override ImmutableArray<AliasAndExternAliasDirective> ExternAliases { get; }

        internal sealed override uint LocalScopeDepth => 0u;

        internal WithExternAliasesBinder(Binder next)
            : base(next)
        {
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            LookupSymbolInAliases(ImmutableDictionary<string, AliasAndUsingDirective>.Empty, ExternAliases, originalBinder, result, name, arity, basesBeingResolved, options, diagnose, ref useSiteInfo);
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if ((options & LookupOptions.LabelsOnly) == 0)
            {
                AddLookupSymbolsInfoInAliases(ImmutableDictionary<string, AliasAndUsingDirective>.Empty, ExternAliases, result, options, originalBinder);
            }
        }

        protected sealed override SourceLocalSymbol? LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected sealed override LocalFunctionSymbol? LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }

        internal static WithExternAliasesBinder Create(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next)
        {
            return new FromSyntax(declaringSymbol, declarationSyntax, next);
        }

        internal static WithExternAliasesBinder Create(ImmutableArray<AliasAndExternAliasDirective> externAliases, Binder next)
        {
            return new FromSymbols(externAliases, next);
        }
    }
}
