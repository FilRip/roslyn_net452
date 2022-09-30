using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class WithExternAndUsingAliasesBinder : WithExternAliasesBinder
    {
        private sealed class FromSyntax : WithExternAndUsingAliasesBinder
        {
            private readonly SourceNamespaceSymbol _declaringSymbol;

            private readonly CSharpSyntaxNode _declarationSyntax;

            private ImmutableArray<AliasAndExternAliasDirective> _lazyExternAliases;

            private ImmutableArray<AliasAndUsingDirective> _lazyUsingAliases;

            private ImmutableDictionary<string, AliasAndUsingDirective>? _lazyUsingAliasesMap;

            private QuickAttributeChecker? _lazyQuickAttributeChecker;

            internal sealed override ImmutableArray<AliasAndExternAliasDirective> ExternAliases
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

            internal override ImmutableArray<AliasAndUsingDirective> UsingAliases
            {
                get
                {
                    if (_lazyUsingAliases.IsDefault)
                    {
                        ImmutableInterlocked.InterlockedInitialize(ref _lazyUsingAliases, _declaringSymbol.GetUsingAliases(_declarationSyntax, null));
                    }
                    return _lazyUsingAliases;
                }
            }

            internal override QuickAttributeChecker QuickAttributeChecker
            {
                get
                {
                    if (_lazyQuickAttributeChecker == null)
                    {
                        QuickAttributeChecker quickAttributeChecker = base.Next!.QuickAttributeChecker;
                        CSharpSyntaxNode declarationSyntax = _declarationSyntax;
                        SyntaxList<UsingDirectiveSyntax> usings;
                        if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
                        {
                            if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                            {
                                throw ExceptionUtilities.UnexpectedValue(_declarationSyntax);
                            }
                            usings = namespaceDeclarationSyntax.Usings;
                        }
                        else
                        {
                            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = ((SourceNamespaceSymbol)base.Compilation.SourceModule.GlobalNamespace).MergedDeclaration.Declarations.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                SingleNamespaceDeclaration current = enumerator.Current;
                                if (current.HasGlobalUsings && compilationUnitSyntax.SyntaxTree != current.SyntaxReference.SyntaxTree)
                                {
                                    quickAttributeChecker = quickAttributeChecker.AddAliasesIfAny(((CompilationUnitSyntax)current.SyntaxReference.GetSyntax()).Usings, onlyGlobalAliases: true);
                                }
                            }
                            usings = compilationUnitSyntax.Usings;
                        }
                        quickAttributeChecker = (_lazyQuickAttributeChecker = quickAttributeChecker.AddAliasesIfAny(usings));
                    }
                    return _lazyQuickAttributeChecker;
                }
            }

            internal FromSyntax(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, WithUsingNamespacesAndTypesBinder next)
                : base(next)
            {
                _declaringSymbol = declaringSymbol;
                _declarationSyntax = declarationSyntax;
            }

            protected override ImmutableDictionary<string, AliasAndUsingDirective> GetUsingAliasesMap(ConsList<TypeSymbol>? basesBeingResolved)
            {
                if (_lazyUsingAliasesMap == null)
                {
                    Interlocked.CompareExchange(ref _lazyUsingAliasesMap, _declaringSymbol.GetUsingAliasesMap(_declarationSyntax, basesBeingResolved), null);
                }
                return _lazyUsingAliasesMap;
            }

            protected override ImportChain BuildImportChain()
            {
                ImportChain parentOpt = base.Next!.ImportChain;
                if (_declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                {
                    for (NameSyntax nameSyntax = namespaceDeclarationSyntax.Name; nameSyntax is QualifiedNameSyntax qualifiedNameSyntax; nameSyntax = qualifiedNameSyntax.Left)
                    {
                        parentOpt = new ImportChain(Imports.Empty, parentOpt);
                    }
                }
                return new ImportChain(_declaringSymbol.GetImports(_declarationSyntax, null), parentOpt);
            }
        }

        private sealed class FromSymbols : WithExternAndUsingAliasesBinder
        {
            private readonly ImmutableArray<AliasAndExternAliasDirective> _externAliases;

            private readonly ImmutableDictionary<string, AliasAndUsingDirective> _usingAliases;

            internal override ImmutableArray<AliasAndExternAliasDirective> ExternAliases => _externAliases;

            internal override ImmutableArray<AliasAndUsingDirective> UsingAliases => _usingAliases.SelectAsArray<KeyValuePair<string, AliasAndUsingDirective>, AliasAndUsingDirective>((KeyValuePair<string, AliasAndUsingDirective> pair) => pair.Value);

            internal FromSymbols(ImmutableArray<AliasAndExternAliasDirective> externAliases, ImmutableDictionary<string, AliasAndUsingDirective> usingAliases, WithUsingNamespacesAndTypesBinder next)
                : base(next)
            {
                _externAliases = externAliases;
                _usingAliases = usingAliases;
            }

            protected override ImmutableDictionary<string, AliasAndUsingDirective> GetUsingAliasesMap(ConsList<TypeSymbol>? basesBeingResolved)
            {
                return _usingAliases;
            }

            protected override ImportChain BuildImportChain()
            {
                return new ImportChain(Imports.Create(_usingAliases, ((WithUsingNamespacesAndTypesBinder)base.Next).GetUsings(null), _externAliases), base.Next!.ImportChain);
            }
        }

        private ImportChain? _lazyImportChain;

        internal abstract override ImmutableArray<AliasAndUsingDirective> UsingAliases { get; }

        internal override ImportChain ImportChain
        {
            get
            {
                if (_lazyImportChain == null)
                {
                    Interlocked.CompareExchange(ref _lazyImportChain, BuildImportChain(), null);
                }
                return _lazyImportChain;
            }
        }

        protected WithExternAndUsingAliasesBinder(WithUsingNamespacesAndTypesBinder next)
            : base(next)
        {
        }

        protected abstract ImmutableDictionary<string, AliasAndUsingDirective> GetUsingAliasesMap(ConsList<TypeSymbol>? basesBeingResolved);

        internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, ConsList<TypeSymbol> basesBeingResolved, LookupOptions options, Binder originalBinder, bool diagnose, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            LookupSymbolInAliases(GetUsingAliasesMap(basesBeingResolved), ExternAliases, originalBinder, result, name, arity, basesBeingResolved, options, diagnose, ref useSiteInfo);
        }

        protected override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo result, LookupOptions options, Binder originalBinder)
        {
            if ((options & LookupOptions.LabelsOnly) == 0)
            {
                AddLookupSymbolsInfoInAliases(GetUsingAliasesMap(null), ExternAliases, result, options, originalBinder);
            }
        }

        protected abstract ImportChain BuildImportChain();

        internal bool IsUsingAlias(string name, bool callerIsSemanticModel, ConsList<TypeSymbol>? basesBeingResolved)
        {
            return IsUsingAlias(GetUsingAliasesMap(basesBeingResolved), name, callerIsSemanticModel);
        }

        [Obsolete("Use other overloads", true)]
        internal new static WithExternAndUsingAliasesBinder Create(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal static WithExternAndUsingAliasesBinder Create(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, WithUsingNamespacesAndTypesBinder next)
        {
            return new FromSyntax(declaringSymbol, declarationSyntax, next);
        }

        internal static WithExternAndUsingAliasesBinder Create(ImmutableArray<AliasAndExternAliasDirective> externAliases, ImmutableDictionary<string, AliasAndUsingDirective> usingAliases, WithUsingNamespacesAndTypesBinder next)
        {
            return new FromSymbols(externAliases, usingAliases, next);
        }
    }
}
