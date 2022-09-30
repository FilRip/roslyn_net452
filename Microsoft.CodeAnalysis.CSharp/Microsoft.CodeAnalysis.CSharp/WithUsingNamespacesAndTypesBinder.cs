using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class WithUsingNamespacesAndTypesBinder : Binder
    {
        private sealed class FromSyntax : WithUsingNamespacesAndTypesBinder
        {
            private readonly SourceNamespaceSymbol _declaringSymbol;

            private readonly CSharpSyntaxNode _declarationSyntax;

            private ImmutableArray<NamespaceOrTypeAndUsingDirective> _lazyUsings;

            internal FromSyntax(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next, bool withImportChainEntry)
                : base(next, withImportChainEntry)
            {
                _declaringSymbol = declaringSymbol;
                _declarationSyntax = declarationSyntax;
            }

            internal override ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsings(ConsList<TypeSymbol>? basesBeingResolved)
            {
                if (_lazyUsings.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyUsings, _declaringSymbol.GetUsingNamespacesOrTypes(_declarationSyntax, basesBeingResolved));
                }
                return _lazyUsings;
            }

            protected override Imports GetImports()
            {
                return _declaringSymbol.GetImports(_declarationSyntax, null);
            }
        }

        private sealed class FromSyntaxWithPreviousSubmissionImports : WithUsingNamespacesAndTypesBinder
        {
            private readonly SourceNamespaceSymbol _declaringSymbol;

            private readonly CSharpSyntaxNode _declarationSyntax;

            private Imports? _lazyFullImports;

            internal FromSyntaxWithPreviousSubmissionImports(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next, bool withImportChainEntry)
                : base(next, withImportChainEntry)
            {
                _declaringSymbol = declaringSymbol;
                _declarationSyntax = declarationSyntax;
            }

            internal override ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsings(ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetImports(basesBeingResolved).Usings;
            }

            private Imports GetImports(ConsList<TypeSymbol>? basesBeingResolved)
            {
                if (_lazyFullImports == null)
                {
                    Interlocked.CompareExchange(ref _lazyFullImports, _declaringSymbol.DeclaringCompilation.GetPreviousSubmissionImports().Concat(_declaringSymbol.GetImports(_declarationSyntax, basesBeingResolved)), null);
                }
                return _lazyFullImports;
            }

            protected override Imports GetImports()
            {
                return GetImports(null);
            }
        }

        private sealed class FromNamespacesOrTypes : WithUsingNamespacesAndTypesBinder
        {
            private readonly ImmutableArray<NamespaceOrTypeAndUsingDirective> _usings;

            internal FromNamespacesOrTypes(ImmutableArray<NamespaceOrTypeAndUsingDirective> namespacesOrTypes, Binder next, bool withImportChainEntry)
                : base(next, withImportChainEntry)
            {
                _usings = namespacesOrTypes;
            }

            internal override ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsings(ConsList<TypeSymbol>? basesBeingResolved)
            {
                return _usings;
            }

            protected override Imports GetImports()
            {
                return Imports.Create(ImmutableDictionary<string, AliasAndUsingDirective>.Empty, _usings, ImmutableArray<AliasAndExternAliasDirective>.Empty);
            }
        }

        private readonly bool _withImportChainEntry;

        private ImportChain? _lazyImportChain;

        internal override bool SupportsExtensionMethods => true;

        internal override uint LocalScopeDepth => 0u;

        internal override ImportChain? ImportChain
        {
            get
            {
                if (_lazyImportChain == null)
                {
                    ImportChain importChain = base.Next!.ImportChain;
                    if (_withImportChainEntry)
                    {
                        importChain = new ImportChain(GetImports(), importChain);
                    }
                    Interlocked.CompareExchange(ref _lazyImportChain, importChain, null);
                }
                return _lazyImportChain;
            }
        }

        protected WithUsingNamespacesAndTypesBinder(Binder next, bool withImportChainEntry)
            : base(next)
        {
            _withImportChainEntry = withImportChainEntry;
        }

        internal abstract ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsings(ConsList<TypeSymbol>? basesBeingResolved);

        protected override AssemblySymbol? GetForwardedToAssemblyInUsingNamespaces(string name, ref NamespaceOrTypeSymbol qualifierOpt, BindingDiagnosticBag diagnostics, Location location)
        {
            ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = GetUsings(null).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                string fullName = current.NamespaceOrType?.ToString() + "." + name;
                AssemblySymbol forwardedToAssembly = GetForwardedToAssembly(fullName, diagnostics, location);
                if (forwardedToAssembly != null)
                {
                    qualifierOpt = current.NamespaceOrType;
                    return forwardedToAssembly;
                }
            }
            return base.GetForwardedToAssemblyInUsingNamespaces(name, ref qualifierOpt, diagnostics, location);
        }

        internal override void GetCandidateExtensionMethods(ArrayBuilder<MethodSymbol> methods, string name, int arity, LookupOptions options, Binder originalBinder)
        {
            bool isSemanticModelBinder = originalBinder.IsSemanticModelBinder;
            bool flag = false;
            bool flag2 = false;
            ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = GetUsings(null).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                switch (current.NamespaceOrType.Kind)
                {
                    case SymbolKind.Namespace:
                        {
                            int count2 = methods.Count;
                            ((NamespaceSymbol)current.NamespaceOrType).GetExtensionMethods(methods, name, arity, options);
                            if (methods.Count != count2)
                            {
                                MarkImportDirective(current.UsingDirectiveReference, isSemanticModelBinder);
                                flag = true;
                            }
                            break;
                        }
                    case SymbolKind.NamedType:
                        {
                            int count = methods.Count;
                            ((NamedTypeSymbol)current.NamespaceOrType).GetExtensionMethods(methods, name, arity, options);
                            if (methods.Count != count)
                            {
                                MarkImportDirective(current.UsingDirectiveReference, isSemanticModelBinder);
                                flag2 = true;
                            }
                            break;
                        }
                }
            }
            if (flag && flag2)
            {
                methods.RemoveDuplicates();
            }
        }

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol>? basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            bool isSemanticModelBinder = originalBinder.IsSemanticModelBinder;
            ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = GetUsings(basesBeingResolved).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                ImmutableArray<Symbol>.Enumerator enumerator2 = Binder.GetCandidateMembers(current.NamespaceOrType, name, options, originalBinder).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (IsValidLookupCandidateInUsings(current2))
                    {
                        SingleLookupResult result2 = originalBinder.CheckViability(current2, arity, options, null, diagnose, ref useSiteInfo, basesBeingResolved);
                        if (result2.Kind == LookupResultKind.Viable)
                        {
                            MarkImportDirective(current.UsingDirectiveReference, isSemanticModelBinder);
                        }
                        result.MergeEqual(result2);
                    }
                }
            }
        }

        private static bool IsValidLookupCandidateInUsings(Symbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Namespace:
                    return false;
                case SymbolKind.Method:
                    if (!symbol.IsStatic || ((MethodSymbol)symbol).IsExtensionMethod)
                    {
                        return false;
                    }
                    break;
                default:
                    if (!symbol.IsStatic)
                    {
                        return false;
                    }
                    break;
                case SymbolKind.NamedType:
                    break;
            }
            return true;
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if ((options & LookupOptions.LabelsOnly) != 0)
            {
                return;
            }
            options = (options & ~(LookupOptions.NamespaceAliasesOnly | LookupOptions.NamespacesOrTypesOnly)) | LookupOptions.MustNotBeNamespace;
            ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = GetUsings(null).GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = enumerator.Current.NamespaceOrType.GetMembersUnordered().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    if (IsValidLookupCandidateInUsings(current) && originalBinder.CanAddLookupSymbolInfo(current, options, result, null))
                    {
                        result.AddSymbol(current, current.Name, current.GetArity());
                    }
                }
            }
        }

        protected override SourceLocalSymbol? LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected override LocalFunctionSymbol? LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }

        protected abstract Imports GetImports();

        internal static WithUsingNamespacesAndTypesBinder Create(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next, bool withPreviousSubmissionImports = false, bool withImportChainEntry = false)
        {
            if (withPreviousSubmissionImports)
            {
                return new FromSyntaxWithPreviousSubmissionImports(declaringSymbol, declarationSyntax, next, withImportChainEntry);
            }
            return new FromSyntax(declaringSymbol, declarationSyntax, next, withImportChainEntry);
        }

        internal static WithUsingNamespacesAndTypesBinder Create(ImmutableArray<NamespaceOrTypeAndUsingDirective> namespacesOrTypes, Binder next, bool withImportChainEntry = false)
        {
            return new FromNamespacesOrTypes(namespacesOrTypes, next, withImportChainEntry);
        }
    }
}
