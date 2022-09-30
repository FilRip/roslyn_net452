using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceNamespaceSymbol : NamespaceSymbol
    {
        private class AliasesAndUsings
        {
            private class ExternAliasesAndDiagnostics
            {
                public static readonly ExternAliasesAndDiagnostics Empty = new ExternAliasesAndDiagnostics
                {
                    ExternAliases = ImmutableArray<AliasAndExternAliasDirective>.Empty,
                    Diagnostics = ImmutableArray<Diagnostic>.Empty
                };

                public ImmutableArray<AliasAndExternAliasDirective> ExternAliases { get; set; }

                public ImmutableArray<Diagnostic> Diagnostics { get; set; }
            }

            private class UsingsAndDiagnostics
            {
                public static readonly UsingsAndDiagnostics Empty = new UsingsAndDiagnostics
                {
                    UsingAliases = ImmutableArray<AliasAndUsingDirective>.Empty,
                    UsingAliasesMap = null,
                    UsingNamespacesOrTypes = ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty,
                    Diagnostics = null
                };

                public ImmutableArray<AliasAndUsingDirective> UsingAliases { get; set; }

                public ImmutableDictionary<string, AliasAndUsingDirective>? UsingAliasesMap { get; set; }

                public ImmutableArray<NamespaceOrTypeAndUsingDirective> UsingNamespacesOrTypes { get; set; }

                public DiagnosticBag? Diagnostics { get; set; }
            }

            private ExternAliasesAndDiagnostics? _lazyExternAliases;

            private UsingsAndDiagnostics? _lazyGlobalUsings;

            private UsingsAndDiagnostics? _lazyUsings;

            private Imports? _lazyImports;

            private SymbolCompletionState _state;

            internal ImmutableArray<AliasAndExternAliasDirective> GetExternAliases(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax)
            {
                return GetExternAliasesAndDiagnostics(declaringSymbol, declarationSyntax).ExternAliases;
            }

            internal ImmutableArray<AliasAndExternAliasDirective> GetExternAliases(SourceNamespaceSymbol declaringSymbol, SyntaxReference declarationSyntax)
            {
                return (_lazyExternAliases ?? GetExternAliasesAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax()))!.ExternAliases;
            }

            private ExternAliasesAndDiagnostics GetExternAliasesAndDiagnostics(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax)
            {
                if (_lazyExternAliases == null)
                {
                    SyntaxList<ExternAliasDirectiveSyntax> externs;
                    if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
                    {
                        if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                        {
                            throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                        }
                        externs = namespaceDeclarationSyntax.Externs;
                    }
                    else
                    {
                        externs = compilationUnitSyntax.Externs;
                    }
                    if (!externs.Any())
                    {
                        _lazyExternAliases = ExternAliasesAndDiagnostics.Empty;
                    }
                    else
                    {
                        DiagnosticBag instance = DiagnosticBag.GetInstance();
                        Interlocked.CompareExchange(ref _lazyExternAliases, new ExternAliasesAndDiagnostics
                        {
                            ExternAliases = buildExternAliases(externs, declaringSymbol, instance),
                            Diagnostics = instance.ToReadOnlyAndFree()
                        }, null);
                    }
                }
                return _lazyExternAliases;
                static ImmutableArray<AliasAndExternAliasDirective> buildExternAliases(SyntaxList<ExternAliasDirectiveSyntax> syntaxList, SourceNamespaceSymbol declaringSymbol, DiagnosticBag diagnostics)
                {
                    CSharpCompilation declaringCompilation = declaringSymbol.DeclaringCompilation;
                    ArrayBuilder<AliasAndExternAliasDirective> instance2 = ArrayBuilder<AliasAndExternAliasDirective>.GetInstance();
                    SyntaxList<ExternAliasDirectiveSyntax>.Enumerator enumerator = syntaxList.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ExternAliasDirectiveSyntax current = enumerator.Current;
                        declaringCompilation.RecordImport(current);
                        bool skipInLookup = false;
                        if (declaringCompilation.IsSubmission)
                        {
                            diagnostics.Add(ErrorCode.ERR_ExternAliasNotAllowed, current.Location);
                            skipInLookup = true;
                        }
                        else
                        {
                            ArrayBuilder<AliasAndExternAliasDirective>.Enumerator enumerator2 = instance2.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                AliasAndExternAliasDirective current2 = enumerator2.Current;
                                if (current2.Alias.Name == current.Identifier.ValueText)
                                {
                                    diagnostics.Add(ErrorCode.ERR_DuplicateAlias, current2.Alias.Locations[0], current2.Alias.Name);
                                    break;
                                }
                            }
                            if (current.Identifier.ContextualKind() == SyntaxKind.GlobalKeyword)
                            {
                                diagnostics.Add(ErrorCode.ERR_GlobalExternAlias, current.Identifier.GetLocation());
                            }
                        }
                        instance2.Add(new AliasAndExternAliasDirective(new AliasSymbolFromSyntax(declaringSymbol, current), current, skipInLookup));
                    }
                    return instance2.ToImmutableAndFree();
                }
            }

            internal ImmutableArray<AliasAndUsingDirective> GetUsingAliases(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetUsingsAndDiagnostics(declaringSymbol, declarationSyntax, basesBeingResolved).UsingAliases;
            }

            internal ImmutableArray<AliasAndUsingDirective> GetGlobalUsingAliases(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetGlobalUsingsAndDiagnostics(declaringSymbol, declarationSyntax, basesBeingResolved).UsingAliases;
            }

            internal ImmutableDictionary<string, AliasAndUsingDirective> GetUsingAliasesMap(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetUsingsAndDiagnostics(declaringSymbol, declarationSyntax, basesBeingResolved).UsingAliasesMap ?? ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
            }

            internal ImmutableDictionary<string, AliasAndUsingDirective> GetGlobalUsingAliasesMap(SourceNamespaceSymbol declaringSymbol, SyntaxReference declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return (_lazyGlobalUsings ?? GetGlobalUsingsAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax(), basesBeingResolved))!.UsingAliasesMap ?? ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
            }

            internal ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsingNamespacesOrTypes(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetUsingsAndDiagnostics(declaringSymbol, declarationSyntax, basesBeingResolved).UsingNamespacesOrTypes;
            }

            private UsingsAndDiagnostics GetUsingsAndDiagnostics(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetUsingsAndDiagnostics(ref _lazyUsings, declaringSymbol, declarationSyntax, basesBeingResolved, onlyGlobal: false);
            }

            internal ImmutableArray<NamespaceOrTypeAndUsingDirective> GetGlobalUsingNamespacesOrTypes(SourceNamespaceSymbol declaringSymbol, SyntaxReference declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return (_lazyGlobalUsings ?? GetGlobalUsingsAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax(), basesBeingResolved))!.UsingNamespacesOrTypes;
            }

            private UsingsAndDiagnostics GetGlobalUsingsAndDiagnostics(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                return GetUsingsAndDiagnostics(ref _lazyGlobalUsings, declaringSymbol, declarationSyntax, basesBeingResolved, onlyGlobal: true);
            }

            private UsingsAndDiagnostics GetUsingsAndDiagnostics(ref UsingsAndDiagnostics? usings, SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved, bool onlyGlobal)
            {
                if (usings == null)
                {
                    bool? flag;
                    SyntaxList<UsingDirectiveSyntax> usings2;
                    if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
                    {
                        if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                        {
                            throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                        }
                        flag = null;
                        usings2 = namespaceDeclarationSyntax.Usings;
                    }
                    else
                    {
                        flag = onlyGlobal;
                        usings2 = compilationUnitSyntax.Usings;
                    }
                    UsingsAndDiagnostics value = (usings2.Any() ? buildUsings(usings2, declaringSymbol, declarationSyntax, flag, basesBeingResolved) : ((flag == false) ? new UsingsAndDiagnostics
                    {
                        UsingAliases = GetGlobalUsingAliases(declaringSymbol, declarationSyntax, basesBeingResolved),
                        UsingAliasesMap = declaringSymbol.GetGlobalUsingAliasesMap(basesBeingResolved),
                        UsingNamespacesOrTypes = declaringSymbol.GetGlobalUsingNamespacesOrTypes(basesBeingResolved),
                        Diagnostics = null
                    } : UsingsAndDiagnostics.Empty));
                    Interlocked.CompareExchange(ref usings, value, null);
                }
                return usings;
                UsingsAndDiagnostics buildUsings(SyntaxList<UsingDirectiveSyntax> usingDirectives, SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, bool? applyIsGlobalFilter, ConsList<TypeSymbol>? basesBeingResolved)
                {
                    ImmutableArray<AliasAndExternAliasDirective> externAliases = GetExternAliases(declaringSymbol, declarationSyntax);
                    ImmutableDictionary<string, AliasAndUsingDirective> immutableDictionary = ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
                    ImmutableArray<NamespaceOrTypeAndUsingDirective> immutableArray = ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty;
                    ImmutableArray<AliasAndUsingDirective> immutableArray2 = ImmutableArray<AliasAndUsingDirective>.Empty;
                    if (applyIsGlobalFilter == false)
                    {
                        immutableDictionary = declaringSymbol.GetGlobalUsingAliasesMap(basesBeingResolved);
                        immutableArray = declaringSymbol.GetGlobalUsingNamespacesOrTypes(basesBeingResolved);
                        immutableArray2 = GetGlobalUsingAliases(declaringSymbol, declarationSyntax, basesBeingResolved);
                    }
                    DiagnosticBag diagnosticBag = new DiagnosticBag();
                    CSharpCompilation declaringCompilation = declaringSymbol.DeclaringCompilation;
                    ArrayBuilder<NamespaceOrTypeAndUsingDirective> usings3 = null;
                    ImmutableDictionary<string, AliasAndUsingDirective>.Builder builder = null;
                    ArrayBuilder<AliasAndUsingDirective> arrayBuilder = null;
                    Binder binder = null;
                    PooledHashSet<NamespaceOrTypeSymbol> uniqueUsings2 = null;
                    SyntaxList<UsingDirectiveSyntax>.Enumerator enumerator = usingDirectives.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        UsingDirectiveSyntax current = enumerator.Current;
                        if (!applyIsGlobalFilter.HasValue || current.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword) == applyIsGlobalFilter.GetValueOrDefault())
                        {
                            declaringCompilation.RecordImport(current);
                            if (current.Alias != null)
                            {
                                SyntaxToken identifier = current.Alias!.Name.Identifier;
                                Location location = current.Alias!.Name.Location;
                                if (identifier.ContextualKind() == SyntaxKind.GlobalKeyword)
                                {
                                    diagnosticBag.Add(ErrorCode.WRN_GlobalAliasDefn, location);
                                }
                                if (current.StaticKeyword != default(SyntaxToken))
                                {
                                    diagnosticBag.Add(ErrorCode.ERR_NoAliasHere, location);
                                }
                                SourceMemberContainerTypeSymbol.ReportTypeNamedRecord(identifier.Text, declaringCompilation, diagnosticBag, location);
                                string valueText = identifier.ValueText;
                                bool flag2 = false;
                                if (builder?.ContainsKey(valueText) ?? immutableDictionary.ContainsKey(valueText))
                                {
                                    flag2 = true;
                                    if (!current.Name.IsMissing)
                                    {
                                        diagnosticBag.Add(ErrorCode.ERR_DuplicateAlias, location, valueText);
                                    }
                                }
                                else
                                {
                                    ImmutableArray<AliasAndExternAliasDirective>.Enumerator enumerator2 = externAliases.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        if (enumerator2.Current.Alias.Name == valueText)
                                        {
                                            diagnosticBag.Add(ErrorCode.ERR_DuplicateAlias, current.Location, valueText);
                                            break;
                                        }
                                    }
                                }
                                AliasAndUsingDirective aliasAndUsingDirective = new AliasAndUsingDirective(new AliasSymbolFromSyntax(declaringSymbol, current), current);
                                if (arrayBuilder == null)
                                {
                                    arrayBuilder = ArrayBuilder<AliasAndUsingDirective>.GetInstance();
                                    arrayBuilder.AddRange(immutableArray2);
                                }
                                arrayBuilder.Add(aliasAndUsingDirective);
                                if (!flag2)
                                {
                                    if (builder == null)
                                    {
                                        builder = immutableDictionary.ToBuilder();
                                    }
                                    builder.Add(valueText, aliasAndUsingDirective);
                                }
                            }
                            else if (!current.Name.IsMissing)
                            {
                                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                                if (binder == null)
                                {
                                    binder = declaringCompilation.GetBinderFactory(declarationSyntax.SyntaxTree).GetBinder(current.Name).WithAdditionalFlags(BinderFlags.SuppressConstraintChecks);
                                }
                                NamespaceOrTypeSymbol namespaceOrTypeSymbol = binder.BindNamespaceOrTypeSymbol(current.Name, instance, basesBeingResolved).NamespaceOrTypeSymbol;
                                if (namespaceOrTypeSymbol.Kind == SymbolKind.Namespace)
                                {
                                    if (current.StaticKeyword != default(SyntaxToken))
                                    {
                                        diagnosticBag.Add(ErrorCode.ERR_BadUsingType, current.Name.Location, namespaceOrTypeSymbol);
                                    }
                                    else if (!getOrCreateUniqueUsings(ref uniqueUsings2, immutableArray).Add(namespaceOrTypeSymbol))
                                    {
                                        diagnosticBag.Add(ErrorCode.WRN_DuplicateUsing, current.Name.Location, namespaceOrTypeSymbol);
                                    }
                                    else
                                    {
                                        getOrCreateUsingsBuilder(ref usings3, immutableArray).Add(new NamespaceOrTypeAndUsingDirective(namespaceOrTypeSymbol, current, default(ImmutableArray<AssemblySymbol>)));
                                    }
                                }
                                else if (namespaceOrTypeSymbol.Kind == SymbolKind.NamedType)
                                {
                                    if (current.StaticKeyword == default(SyntaxToken))
                                    {
                                        diagnosticBag.Add(ErrorCode.ERR_BadUsingNamespace, current.Name.Location, namespaceOrTypeSymbol);
                                    }
                                    else
                                    {
                                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)namespaceOrTypeSymbol;
                                        if (!getOrCreateUniqueUsings(ref uniqueUsings2, immutableArray).Add(namedTypeSymbol))
                                        {
                                            diagnosticBag.Add(ErrorCode.WRN_DuplicateUsing, current.Name.Location, namedTypeSymbol);
                                        }
                                        else
                                        {
                                            binder.ReportDiagnosticsIfObsolete(diagnosticBag, namedTypeSymbol, current.Name, hasBaseReceiver: false);
                                            getOrCreateUsingsBuilder(ref usings3, immutableArray).Add(new NamespaceOrTypeAndUsingDirective(namedTypeSymbol, current, instance.DependenciesBag.ToImmutableArray()));
                                        }
                                    }
                                }
                                else if (namespaceOrTypeSymbol.Kind != SymbolKind.ErrorType)
                                {
                                    diagnosticBag.Add(ErrorCode.ERR_BadSKknown, current.Name.Location, current.Name, namespaceOrTypeSymbol.GetKindText(), MessageID.IDS_SK_TYPE_OR_NAMESPACE.Localize());
                                }
                                diagnosticBag.AddRange(instance.DiagnosticBag);
                                instance.Free();
                            }
                        }
                    }
                    uniqueUsings2?.Free();
                    if (diagnosticBag.IsEmptyWithoutResolution)
                    {
                        diagnosticBag = null;
                    }
                    return new UsingsAndDiagnostics
                    {
                        UsingAliases = (arrayBuilder?.ToImmutableAndFree() ?? immutableArray2),
                        UsingAliasesMap = (builder?.ToImmutable() ?? immutableDictionary),
                        UsingNamespacesOrTypes = (usings3?.ToImmutableAndFree() ?? immutableArray),
                        Diagnostics = diagnosticBag
                    };
                }
                static PooledHashSet<NamespaceOrTypeSymbol> getOrCreateUniqueUsings(ref PooledHashSet<NamespaceOrTypeSymbol>? uniqueUsings, ImmutableArray<NamespaceOrTypeAndUsingDirective> globalUsingNamespacesOrTypes)
                {
                    if (uniqueUsings == null)
                    {
                        uniqueUsings = SpecializedSymbolCollections.GetPooledSymbolHashSetInstance<NamespaceOrTypeSymbol>();
                        uniqueUsings.AddAll(globalUsingNamespacesOrTypes.Select((NamespaceOrTypeAndUsingDirective n) => n.NamespaceOrType));
                    }
                    return uniqueUsings;
                }
                static ArrayBuilder<NamespaceOrTypeAndUsingDirective> getOrCreateUsingsBuilder(ref ArrayBuilder<NamespaceOrTypeAndUsingDirective>? usings, ImmutableArray<NamespaceOrTypeAndUsingDirective> globalUsingNamespacesOrTypes)
                {
                    if (usings == null)
                    {
                        usings = ArrayBuilder<NamespaceOrTypeAndUsingDirective>.GetInstance();
                        usings!.AddRange(globalUsingNamespacesOrTypes);
                    }
                    return usings;
                }
            }

            internal Imports GetImports(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
            {
                if (_lazyImports == null)
                {
                    Interlocked.CompareExchange(ref _lazyImports, Imports.Create(GetUsingAliasesMap(declaringSymbol, declarationSyntax, basesBeingResolved), GetUsingNamespacesOrTypes(declaringSymbol, declarationSyntax, basesBeingResolved), GetExternAliases(declaringSymbol, declarationSyntax)), null);
                }
                return _lazyImports;
            }

            internal void Complete(SourceNamespaceSymbol declaringSymbol, SyntaxReference declarationSyntax, CancellationToken cancellationToken)
            {
                ExternAliasesAndDiagnostics externAliasesAndDiagnostics = _lazyExternAliases ?? GetExternAliasesAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax(cancellationToken));
                cancellationToken.ThrowIfCancellationRequested();
                UsingsAndDiagnostics usingsAndDiagnostics = _lazyGlobalUsings ?? (declaringSymbol.IsGlobalNamespace ? GetGlobalUsingsAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax(cancellationToken), null) : UsingsAndDiagnostics.Empty);
                cancellationToken.ThrowIfCancellationRequested();
                UsingsAndDiagnostics usingsAndDiagnostics2 = _lazyUsings ?? GetUsingsAndDiagnostics(declaringSymbol, (CSharpSyntaxNode)declarationSyntax.GetSyntax(cancellationToken), null);
                cancellationToken.ThrowIfCancellationRequested();
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CompletionPart nextIncompletePart = _state.NextIncompletePart;
                    switch (nextIncompletePart)
                    {
                        case CompletionPart.StartBaseType:
                            if (_state.NotePartComplete(CompletionPart.StartBaseType))
                            {
                                Validate(declaringSymbol, declarationSyntax, externAliasesAndDiagnostics, usingsAndDiagnostics2, usingsAndDiagnostics.Diagnostics);
                                _state.NotePartComplete(CompletionPart.FinishBaseType);
                            }
                            break;
                        case CompletionPart.FinishBaseType:
                            _state.SpinWaitComplete(CompletionPart.FinishBaseType, cancellationToken);
                            break;
                        case CompletionPart.None:
                            return;
                        default:
                            _state.NotePartComplete(CompletionPart.MethodSymbolAll | CompletionPart.StartInterfaces | CompletionPart.FinishInterfaces | CompletionPart.EnumUnderlyingType | CompletionPart.TypeArguments | CompletionPart.FinishMemberChecks | CompletionPart.MembersCompleted);
                            break;
                    }
                    _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
                }
            }

            private static void Validate(SourceNamespaceSymbol declaringSymbol, SyntaxReference declarationSyntax, ExternAliasesAndDiagnostics externAliasesAndDiagnostics, UsingsAndDiagnostics usingsAndDiagnostics, DiagnosticBag? globalUsingDiagnostics)
            {
                CSharpCompilation compilation = declaringSymbol.DeclaringCompilation;
                DiagnosticBag declarationDiagnostics = compilation.DeclarationDiagnostics;
                BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
                if (usingsAndDiagnostics.UsingAliasesMap != null)
                {
                    foreach (var (_, aliasAndUsingDirective2) in usingsAndDiagnostics.UsingAliasesMap!)
                    {
                        if (aliasAndUsingDirective2.UsingDirectiveReference!.SyntaxTree == declarationSyntax.SyntaxTree)
                        {
                            NamespaceOrTypeSymbol aliasTarget = aliasAndUsingDirective2.Alias.GetAliasTarget(null);
                            diagnostics.Clear();
                            if (aliasAndUsingDirective2.Alias is AliasSymbolFromSyntax aliasSymbolFromSyntax)
                            {
                                diagnostics.AddRange(aliasSymbolFromSyntax.AliasTargetDiagnostics);
                            }
                            aliasAndUsingDirective2.Alias.CheckConstraints(diagnostics);
                            declarationDiagnostics.AddRange(diagnostics.DiagnosticBag);
                            recordImportDependencies(aliasAndUsingDirective2.UsingDirective, aliasTarget);
                        }
                    }
                }
                TypeConversions conversions = new TypeConversions(compilation.SourceAssembly.CorLibrary);
                ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator2 = usingsAndDiagnostics.UsingNamespacesOrTypes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    NamespaceOrTypeAndUsingDirective current = enumerator2.Current;
                    if (current.UsingDirectiveReference!.SyntaxTree == declarationSyntax.SyntaxTree)
                    {
                        diagnostics.Clear();
                        diagnostics.AddDependencies(current.Dependencies);
                        NamespaceOrTypeSymbol namespaceOrType = current.NamespaceOrType;
                        UsingDirectiveSyntax usingDirective2 = current.UsingDirective;
                        if (namespaceOrType.IsType)
                        {
                            ((TypeSymbol)namespaceOrType).CheckAllConstraints(location: usingDirective2.Name.Location, compilation: compilation, conversions: conversions, diagnostics: diagnostics);
                        }
                        declarationDiagnostics.AddRange(diagnostics.DiagnosticBag);
                        recordImportDependencies(usingDirective2, namespaceOrType);
                    }
                }
                ImmutableArray<AliasAndExternAliasDirective>.Enumerator enumerator3 = externAliasesAndDiagnostics.ExternAliases.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    AliasAndExternAliasDirective current2 = enumerator3.Current;
                    if (!current2.SkipInLookup)
                    {
                        NamespaceSymbol ns = (NamespaceSymbol)current2.Alias.GetAliasTarget(null);
                        if (current2.Alias is AliasSymbolFromSyntax aliasSymbolFromSyntax2)
                        {
                            declarationDiagnostics.AddRange(aliasSymbolFromSyntax2.AliasTargetDiagnostics.DiagnosticBag);
                        }
                        if (!Compilation.ReportUnusedImportsInTree(current2.ExternAliasDirective!.SyntaxTree))
                        {
                            diagnostics.Clear();
                            diagnostics.AddAssembliesUsedByNamespaceReference(ns);
                            compilation.AddUsedAssemblies(diagnostics.DependenciesBag);
                        }
                    }
                }
                declarationDiagnostics.AddRange(externAliasesAndDiagnostics.Diagnostics);
                DiagnosticBag? diagnostics2 = usingsAndDiagnostics.Diagnostics;
                if (diagnostics2 != null && !diagnostics2!.IsEmptyWithoutResolution)
                {
                    declarationDiagnostics.AddRange(usingsAndDiagnostics.Diagnostics!.AsEnumerable());
                }
                if (globalUsingDiagnostics != null && !globalUsingDiagnostics!.IsEmptyWithoutResolution)
                {
                    declarationDiagnostics.AddRange(globalUsingDiagnostics!.AsEnumerable());
                }
                diagnostics.Free();
                void recordImportDependencies(UsingDirectiveSyntax usingDirective, NamespaceOrTypeSymbol target)
                {
                    if (Compilation.ReportUnusedImportsInTree(usingDirective.SyntaxTree))
                    {
                        compilation.RecordImportDependencies(usingDirective, diagnostics.DependenciesBag.ToImmutableArray());
                    }
                    else
                    {
                        if (target.IsNamespace)
                        {
                            diagnostics.AddAssembliesUsedByNamespaceReference((NamespaceSymbol)target);
                        }
                        compilation.AddUsedAssemblies(diagnostics.DependenciesBag);
                    }
                }
            }
        }

        private class MergedGlobalAliasesAndUsings
        {
            private Imports? _lazyImports;

            private SymbolCompletionState _state;

            public static readonly MergedGlobalAliasesAndUsings Empty = new MergedGlobalAliasesAndUsings
            {
                UsingAliasesMap = ImmutableDictionary<string, AliasAndUsingDirective>.Empty,
                UsingNamespacesOrTypes = ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty,
                Diagnostics = ImmutableArray<Diagnostic>.Empty,
                _lazyImports = Microsoft.CodeAnalysis.CSharp.Imports.Empty
            };

            public ImmutableDictionary<string, AliasAndUsingDirective>? UsingAliasesMap { get; set; }

            public ImmutableArray<NamespaceOrTypeAndUsingDirective> UsingNamespacesOrTypes { get; set; }

            public ImmutableArray<Diagnostic> Diagnostics { get; set; }

            public Imports Imports
            {
                get
                {
                    if (_lazyImports == null)
                    {
                        Interlocked.CompareExchange(ref _lazyImports, Microsoft.CodeAnalysis.CSharp.Imports.Create(UsingAliasesMap ?? ImmutableDictionary<string, AliasAndUsingDirective>.Empty, UsingNamespacesOrTypes, ImmutableArray<AliasAndExternAliasDirective>.Empty), null);
                    }
                    return _lazyImports;
                }
            }

            internal void Complete(SourceNamespaceSymbol declaringSymbol, CancellationToken cancellationToken)
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CompletionPart nextIncompletePart = _state.NextIncompletePart;
                    switch (nextIncompletePart)
                    {
                        case CompletionPart.StartBaseType:
                            if (_state.NotePartComplete(CompletionPart.StartBaseType))
                            {
                                if (!Diagnostics.IsDefaultOrEmpty)
                                {
                                    declaringSymbol.DeclaringCompilation.DeclarationDiagnostics.AddRange(Diagnostics);
                                }
                                _state.NotePartComplete(CompletionPart.FinishBaseType);
                            }
                            break;
                        case CompletionPart.FinishBaseType:
                            _state.SpinWaitComplete(CompletionPart.FinishBaseType, cancellationToken);
                            break;
                        case CompletionPart.None:
                            return;
                        default:
                            _state.NotePartComplete(CompletionPart.MethodSymbolAll | CompletionPart.StartInterfaces | CompletionPart.FinishInterfaces | CompletionPart.EnumUnderlyingType | CompletionPart.TypeArguments | CompletionPart.FinishMemberChecks | CompletionPart.MembersCompleted);
                            break;
                    }
                    _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
                }
            }
        }

        private struct NameToSymbolMapBuilder
        {
            private readonly Dictionary<string, object> _dictionary;

            public NameToSymbolMapBuilder(int capacity)
            {
                _dictionary = new Dictionary<string, object>(capacity, StringOrdinalComparer.Instance);
            }

            public void Add(NamespaceOrTypeSymbol symbol)
            {
                string name = symbol.Name;
                if (_dictionary.TryGetValue(name, out var value))
                {
                    ArrayBuilder<NamespaceOrTypeSymbol> arrayBuilder = value as ArrayBuilder<NamespaceOrTypeSymbol>;
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<NamespaceOrTypeSymbol>.GetInstance();
                        arrayBuilder.Add((NamespaceOrTypeSymbol)value);
                        _dictionary[name] = arrayBuilder;
                    }
                    arrayBuilder.Add(symbol);
                }
                else
                {
                    _dictionary[name] = symbol;
                }
            }

            public Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> CreateMap()
            {
                Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> dictionary = new Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>>(_dictionary.Count, StringOrdinalComparer.Instance);
                foreach (KeyValuePair<string, object> item in _dictionary)
                {
                    object value = item.Value;
                    ImmutableArray<NamespaceOrTypeSymbol> value2;
                    if (value is ArrayBuilder<NamespaceOrTypeSymbol> arrayBuilder)
                    {
                        bool flag = false;
                        for (int i = 0; i < arrayBuilder.Count; i++)
                        {
                            if (flag)
                            {
                                break;
                            }
                            flag |= arrayBuilder[i].Kind == SymbolKind.Namespace;
                        }
                        value2 = (flag ? arrayBuilder.ToImmutable() : StaticCast<NamespaceOrTypeSymbol>.From(arrayBuilder.ToDowncastedImmutable<NamedTypeSymbol>()));
                        arrayBuilder.Free();
                    }
                    else
                    {
                        NamespaceOrTypeSymbol namespaceOrTypeSymbol = (NamespaceOrTypeSymbol)value;
                        value2 = ((namespaceOrTypeSymbol.Kind == SymbolKind.Namespace) ? ImmutableArray.Create(namespaceOrTypeSymbol) : StaticCast<NamespaceOrTypeSymbol>.From(ImmutableArray.Create((NamedTypeSymbol)namespaceOrTypeSymbol)));
                    }
                    dictionary.Add(item.Key, value2);
                }
                return dictionary;
            }
        }

        private readonly SourceModuleSymbol _module;

        private readonly Symbol _container;

        private readonly MergedNamespaceDeclaration _mergedDeclaration;

        private SymbolCompletionState _state;

        private ImmutableArray<Location> _locations;

        private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> _nameToMembersMap;

        private Dictionary<string, ImmutableArray<NamedTypeSymbol>> _nameToTypeMembersMap;

        private ImmutableArray<Symbol> _lazyAllMembers;

        private ImmutableArray<NamedTypeSymbol> _lazyTypeMembersUnordered;

        private readonly ImmutableDictionary<SingleNamespaceDeclaration, AliasesAndUsings> _aliasesAndUsings;

        private MergedGlobalAliasesAndUsings _lazyMergedGlobalAliasesAndUsings;

        private const int LazyAllMembersIsSorted = 1;

        private int _flags;

        private LexicalSortKey _lazyLexicalSortKey = LexicalSortKey.NotInitialized;

        private static readonly Func<SingleNamespaceDeclaration, SyntaxReference> s_declaringSyntaxReferencesSelector = (SingleNamespaceDeclaration d) => new NamespaceDeclarationSyntaxReference(d.SyntaxReference);

        internal MergedNamespaceDeclaration MergedDeclaration => _mergedDeclaration;

        public override Symbol ContainingSymbol => _container;

        public override AssemblySymbol ContainingAssembly => _module.ContainingAssembly;

        public override string Name => _mergedDeclaration.Name;

        public override ImmutableArray<Location> Locations
        {
            get
            {
                if (_locations.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _locations, _mergedDeclaration.NameLocations, default(ImmutableArray<Location>));
                }
                return _locations;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ComputeDeclaringReferencesCore();

        internal override ModuleSymbol ContainingModule => _module;

        internal override NamespaceExtent Extent => new NamespaceExtent(_module);

        public Imports GetImports(CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
            {
                if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                }
                if (!namespaceDeclarationSyntax.Externs.Any() && !namespaceDeclarationSyntax.Usings.Any())
                {
                    return Imports.Empty;
                }
            }
            else if (!compilationUnitSyntax.Externs.Any() && !compilationUnitSyntax.Usings.Any())
            {
                return GetGlobalUsingImports(basesBeingResolved);
            }
            return GetAliasesAndUsings(declarationSyntax).GetImports(this, declarationSyntax, basesBeingResolved);
        }

        private AliasesAndUsings GetAliasesAndUsings(CSharpSyntaxNode declarationSyntax)
        {
            return _aliasesAndUsings[GetMatchingNamespaceDeclaration(declarationSyntax)];
        }

        private SingleNamespaceDeclaration GetMatchingNamespaceDeclaration(CSharpSyntaxNode declarationSyntax)
        {
            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _mergedDeclaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleNamespaceDeclaration current = enumerator.Current;
                SyntaxReference syntaxReference = current.SyntaxReference;
                if (syntaxReference.SyntaxTree == declarationSyntax.SyntaxTree && syntaxReference.GetSyntax() == declarationSyntax)
                {
                    return current;
                }
            }
            throw ExceptionUtilities.Unreachable;
        }

        public ImmutableArray<AliasAndExternAliasDirective> GetExternAliases(CSharpSyntaxNode declarationSyntax)
        {
            if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
            {
                if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                }
                if (!namespaceDeclarationSyntax.Externs.Any())
                {
                    return ImmutableArray<AliasAndExternAliasDirective>.Empty;
                }
            }
            else if (!compilationUnitSyntax.Externs.Any())
            {
                return ImmutableArray<AliasAndExternAliasDirective>.Empty;
            }
            return GetAliasesAndUsings(declarationSyntax).GetExternAliases(this, declarationSyntax);
        }

        public ImmutableArray<AliasAndUsingDirective> GetUsingAliases(CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
            {
                if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                }
                if (!namespaceDeclarationSyntax.Usings.Any())
                {
                    return ImmutableArray<AliasAndUsingDirective>.Empty;
                }
            }
            else if (!compilationUnitSyntax.Usings.Any())
            {
                return ImmutableArray<AliasAndUsingDirective>.Empty;
            }
            return GetAliasesAndUsings(declarationSyntax).GetUsingAliases(this, declarationSyntax, basesBeingResolved);
        }

        public ImmutableDictionary<string, AliasAndUsingDirective> GetUsingAliasesMap(CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
            {
                if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                }
                if (!namespaceDeclarationSyntax.Usings.Any())
                {
                    return ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
                }
            }
            else if (!compilationUnitSyntax.Usings.Any())
            {
                return GetGlobalUsingAliasesMap(basesBeingResolved);
            }
            return GetAliasesAndUsings(declarationSyntax).GetUsingAliasesMap(this, declarationSyntax, basesBeingResolved);
        }

        public ImmutableArray<NamespaceOrTypeAndUsingDirective> GetUsingNamespacesOrTypes(CSharpSyntaxNode declarationSyntax, ConsList<TypeSymbol>? basesBeingResolved)
        {
            if (!(declarationSyntax is CompilationUnitSyntax compilationUnitSyntax))
            {
                if (!(declarationSyntax is NamespaceDeclarationSyntax namespaceDeclarationSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(declarationSyntax);
                }
                if (!namespaceDeclarationSyntax.Usings.Any())
                {
                    return ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty;
                }
            }
            else if (!compilationUnitSyntax.Usings.Any())
            {
                return GetGlobalUsingNamespacesOrTypes(basesBeingResolved);
            }
            return GetAliasesAndUsings(declarationSyntax).GetUsingNamespacesOrTypes(this, declarationSyntax, basesBeingResolved);
        }

        private Imports GetGlobalUsingImports(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return GetMergedGlobalAliasesAndUsings(basesBeingResolved).Imports;
        }

        private ImmutableDictionary<string, AliasAndUsingDirective> GetGlobalUsingAliasesMap(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return GetMergedGlobalAliasesAndUsings(basesBeingResolved).UsingAliasesMap;
        }

        private ImmutableArray<NamespaceOrTypeAndUsingDirective> GetGlobalUsingNamespacesOrTypes(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return GetMergedGlobalAliasesAndUsings(basesBeingResolved).UsingNamespacesOrTypes;
        }

        private MergedGlobalAliasesAndUsings GetMergedGlobalAliasesAndUsings(ConsList<TypeSymbol>? basesBeingResolved, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_lazyMergedGlobalAliasesAndUsings == null)
            {
                if (!IsGlobalNamespace)
                {
                    _lazyMergedGlobalAliasesAndUsings = MergedGlobalAliasesAndUsings.Empty;
                }
                else
                {
                    ImmutableDictionary<string, AliasAndUsingDirective> immutableDictionary = null;
                    ArrayBuilder<NamespaceOrTypeAndUsingDirective> arrayBuilder = ArrayBuilder<NamespaceOrTypeAndUsingDirective>.GetInstance();
                    PooledHashSet<NamespaceOrTypeSymbol> pooledSymbolHashSetInstance = SpecializedSymbolCollections.GetPooledSymbolHashSetInstance<NamespaceOrTypeSymbol>();
                    DiagnosticBag diagnosticBag = DiagnosticBag.GetInstance();
                    try
                    {
                        bool flag = false;
                        ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _mergedDeclaration.Declarations.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            SingleNamespaceDeclaration current = enumerator.Current;
                            if (current.HasExternAliases)
                            {
                                flag = true;
                            }
                            if (!current.HasGlobalUsings)
                            {
                                continue;
                            }
                            ImmutableDictionary<string, AliasAndUsingDirective> globalUsingAliasesMap = _aliasesAndUsings[current].GetGlobalUsingAliasesMap(this, current.SyntaxReference, basesBeingResolved);
                            cancellationToken.ThrowIfCancellationRequested();
                            if (!globalUsingAliasesMap.IsEmpty)
                            {
                                if (immutableDictionary == null)
                                {
                                    immutableDictionary = globalUsingAliasesMap;
                                }
                                else
                                {
                                    ImmutableDictionary<string, AliasAndUsingDirective>.Builder builder = immutableDictionary.ToBuilder();
                                    bool flag2 = false;
                                    foreach (KeyValuePair<string, AliasAndUsingDirective> item in globalUsingAliasesMap)
                                    {
                                        if (builder.ContainsKey(item.Key))
                                        {
                                            diagnosticBag.Add(ErrorCode.ERR_DuplicateAlias, item.Value.Alias.Locations[0], item.Key);
                                        }
                                        else
                                        {
                                            builder.Add(item);
                                            flag2 = true;
                                        }
                                    }
                                    if (flag2)
                                    {
                                        immutableDictionary = builder.ToImmutable();
                                    }
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                            ImmutableArray<NamespaceOrTypeAndUsingDirective> globalUsingNamespacesOrTypes = _aliasesAndUsings[current].GetGlobalUsingNamespacesOrTypes(this, current.SyntaxReference, basesBeingResolved);
                            if (!globalUsingNamespacesOrTypes.IsEmpty)
                            {
                                if (arrayBuilder.Count == 0)
                                {
                                    arrayBuilder.AddRange(globalUsingNamespacesOrTypes);
                                    pooledSymbolHashSetInstance.AddAll(globalUsingNamespacesOrTypes.Select((NamespaceOrTypeAndUsingDirective n) => n.NamespaceOrType));
                                }
                                else
                                {
                                    ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator3 = globalUsingNamespacesOrTypes.GetEnumerator();
                                    while (enumerator3.MoveNext())
                                    {
                                        NamespaceOrTypeAndUsingDirective current3 = enumerator3.Current;
                                        if (!pooledSymbolHashSetInstance.Add(current3.NamespaceOrType))
                                        {
                                            diagnosticBag.Add(ErrorCode.WRN_DuplicateUsing, current3.UsingDirective!.Name.Location, current3.NamespaceOrType);
                                        }
                                        else
                                        {
                                            arrayBuilder.Add(current3);
                                        }
                                    }
                                }
                            }
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        if (flag && immutableDictionary != null)
                        {
                            enumerator = _mergedDeclaration.Declarations.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                SingleNamespaceDeclaration current4 = enumerator.Current;
                                if (!current4.HasExternAliases)
                                {
                                    continue;
                                }
                                ImmutableArray<AliasAndExternAliasDirective> externAliases = _aliasesAndUsings[current4].GetExternAliases(this, current4.SyntaxReference);
                                ImmutableDictionary<string, AliasAndUsingDirective> immutableDictionary2 = ImmutableDictionary<string, AliasAndUsingDirective>.Empty;
                                if (current4.HasGlobalUsings)
                                {
                                    immutableDictionary2 = _aliasesAndUsings[current4].GetGlobalUsingAliasesMap(this, current4.SyntaxReference, basesBeingResolved);
                                }
                                ImmutableArray<AliasAndExternAliasDirective>.Enumerator enumerator4 = externAliases.GetEnumerator();
                                while (enumerator4.MoveNext())
                                {
                                    AliasAndExternAliasDirective current5 = enumerator4.Current;
                                    if (!current5.SkipInLookup && !immutableDictionary2.ContainsKey(current5.Alias.Name) && immutableDictionary.ContainsKey(current5.Alias.Name))
                                    {
                                        diagnosticBag.Add(ErrorCode.ERR_DuplicateAlias, current5.Alias.Locations[0], current5.Alias.Name);
                                    }
                                }
                            }
                        }
                        Interlocked.CompareExchange(ref _lazyMergedGlobalAliasesAndUsings, new MergedGlobalAliasesAndUsings
                        {
                            UsingAliasesMap = (immutableDictionary ?? ImmutableDictionary<string, AliasAndUsingDirective>.Empty),
                            UsingNamespacesOrTypes = arrayBuilder.ToImmutableAndFree(),
                            Diagnostics = diagnosticBag.ToReadOnlyAndFree()
                        }, null);
                        arrayBuilder = null;
                        diagnosticBag = null;
                    }
                    finally
                    {
                        pooledSymbolHashSetInstance.Free();
                        arrayBuilder?.Free();
                        diagnosticBag?.Free();
                    }
                }
            }
            return _lazyMergedGlobalAliasesAndUsings;
        }

        internal SourceNamespaceSymbol(SourceModuleSymbol module, Symbol container, MergedNamespaceDeclaration mergedDeclaration, BindingDiagnosticBag diagnostics)
        {
            _module = module;
            _container = container;
            _mergedDeclaration = mergedDeclaration;
            ImmutableDictionary<SingleNamespaceDeclaration, AliasesAndUsings>.Builder builder = ImmutableDictionary.CreateBuilder<SingleNamespaceDeclaration, AliasesAndUsings>(ReferenceEqualityComparer.Instance);
            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = mergedDeclaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleNamespaceDeclaration current = enumerator.Current;
                if (current.HasExternAliases || current.HasGlobalUsings || current.HasUsings)
                {
                    builder.Add(current, new AliasesAndUsings());
                }
                diagnostics.AddRange(current.Diagnostics);
            }
            _aliasesAndUsings = builder.ToImmutable();
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            if (!_lazyLexicalSortKey.IsInitialized)
            {
                _lazyLexicalSortKey.SetFrom(_mergedDeclaration.GetLexicalSortKey(DeclaringCompilation));
            }
            return _lazyLexicalSortKey;
        }

        private ImmutableArray<SyntaxReference> ComputeDeclaringReferencesCore()
        {
            return _mergedDeclaration.Declarations.SelectAsArray(s_declaringSyntaxReferencesSelector);
        }

        internal override ImmutableArray<Symbol> GetMembersUnordered()
        {
            ImmutableArray<Symbol> lazyAllMembers = _lazyAllMembers;
            if (lazyAllMembers.IsDefault)
            {
                ImmutableArray<Symbol> value = StaticCast<Symbol>.From(GetNameToMembersMap().Flatten());
                ImmutableInterlocked.InterlockedInitialize(ref _lazyAllMembers, value);
                lazyAllMembers = _lazyAllMembers;
            }
            return lazyAllMembers.ConditionallyDeOrder();
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            if (((uint)_flags & (true ? 1u : 0u)) != 0)
            {
                return _lazyAllMembers;
            }
            ImmutableArray<Symbol> immutableArray = GetMembersUnordered();
            if (immutableArray.Length >= 2)
            {
                immutableArray = immutableArray.Sort(LexicalOrderSymbolComparer.Instance);
                ImmutableInterlocked.InterlockedExchange(ref _lazyAllMembers, immutableArray);
            }
            ThreadSafeFlagOperations.Set(ref _flags, 1);
            return immutableArray;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (!GetNameToMembersMap().TryGetValue(name, out var value))
            {
                return ImmutableArray<Symbol>.Empty;
            }
            return value.Cast<NamespaceOrTypeSymbol, Symbol>();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
        {
            if (_lazyTypeMembersUnordered.IsDefault)
            {
                ImmutableArray<NamedTypeSymbol> value = GetNameToTypeMembersMap().Flatten();
                ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeMembersUnordered, value);
            }
            return _lazyTypeMembersUnordered;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return GetNameToTypeMembersMap().Flatten(LexicalOrderSymbolComparer.Instance);
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            if (!GetNameToTypeMembersMap().TryGetValue(name, out var value))
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }
            return value;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol s, int arity) => s.Arity == arity, arity);
        }

        private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> GetNameToMembersMap()
        {
            if (_nameToMembersMap == null)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                if (Interlocked.CompareExchange(ref _nameToMembersMap, MakeNameToMembersMap(instance), null) == null)
                {
                    AddDeclarationDiagnostics(instance);
                    RegisterDeclaredCorTypes();
                    DeclaringCompilation.SymbolDeclaredEvent(this);
                    _state.NotePartComplete(CompletionPart.Members);
                }
                instance.Free();
            }
            return _nameToMembersMap;
        }

        private Dictionary<string, ImmutableArray<NamedTypeSymbol>> GetNameToTypeMembersMap()
        {
            if (_nameToTypeMembersMap == null)
            {
                Interlocked.CompareExchange(ref _nameToTypeMembersMap, GetTypesFromMemberMap(GetNameToMembersMap()), null);
            }
            return _nameToTypeMembersMap;
        }

        private static Dictionary<string, ImmutableArray<NamedTypeSymbol>> GetTypesFromMemberMap(Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> map)
        {
            Dictionary<string, ImmutableArray<NamedTypeSymbol>> dictionary = new Dictionary<string, ImmutableArray<NamedTypeSymbol>>(StringOrdinalComparer.Instance);
            foreach (KeyValuePair<string, ImmutableArray<NamespaceOrTypeSymbol>> item in map)
            {
                ImmutableArray<NamespaceOrTypeSymbol> value = item.Value;
                bool flag = false;
                bool flag2 = false;
                ImmutableArray<NamespaceOrTypeSymbol>.Enumerator enumerator2 = value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.Kind == SymbolKind.NamedType)
                    {
                        flag = true;
                        if (flag2)
                        {
                            break;
                        }
                    }
                    else
                    {
                        flag2 = true;
                        if (flag)
                        {
                            break;
                        }
                    }
                }
                if (flag)
                {
                    if (flag2)
                    {
                        dictionary.Add(item.Key, value.OfType<NamedTypeSymbol>().AsImmutable());
                    }
                    else
                    {
                        dictionary.Add(item.Key, value.As<NamedTypeSymbol>());
                    }
                }
            }
            return dictionary;
        }

        private Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> MakeNameToMembersMap(BindingDiagnosticBag diagnostics)
        {
            NameToSymbolMapBuilder nameToSymbolMapBuilder = new NameToSymbolMapBuilder(_mergedDeclaration.Children.Length);
            ImmutableArray<MergedNamespaceOrTypeDeclaration>.Enumerator enumerator = _mergedDeclaration.Children.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MergedNamespaceOrTypeDeclaration current = enumerator.Current;
                nameToSymbolMapBuilder.Add(BuildSymbol(current, diagnostics));
            }
            Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> result = nameToSymbolMapBuilder.CreateMap();
            CheckMembers(this, result, diagnostics);
            return result;
        }

        private static void CheckMembers(NamespaceSymbol @namespace, Dictionary<string, ImmutableArray<NamespaceOrTypeSymbol>> result, BindingDiagnosticBag diagnostics)
        {
            Symbol[] array = new Symbol[10];
            MergedNamespaceSymbol mergedNamespaceSymbol = null;
            if (@namespace.ContainingAssembly.Modules.Length > 1)
            {
                mergedNamespaceSymbol = @namespace.ContainingAssembly.GetAssemblyNamespace(@namespace) as MergedNamespaceSymbol;
            }
            foreach (string key in result.Keys)
            {
                Array.Clear(array, 0, array.Length);
                ImmutableArray<NamespaceOrTypeSymbol>.Enumerator enumerator2 = result[key].GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    NamespaceOrTypeSymbol current2 = enumerator2.Current;
                    NamedTypeSymbol namedTypeSymbol = current2 as NamedTypeSymbol;
                    int num = namedTypeSymbol?.Arity ?? 0;
                    if (num >= array.Length)
                    {
                        Array.Resize(ref array, num + 1);
                    }
                    Symbol symbol = array[num];
                    if ((object)symbol == null && (object)mergedNamespaceSymbol != null)
                    {
                        ImmutableArray<NamespaceSymbol>.Enumerator enumerator3 = mergedNamespaceSymbol.ConstituentNamespaces.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            NamespaceSymbol current3 = enumerator3.Current;
                            if ((object)current3 != @namespace)
                            {
                                ImmutableArray<NamedTypeSymbol> typeMembers = current3.GetTypeMembers(current2.Name, num);
                                if (typeMembers.Length > 0)
                                {
                                    symbol = typeMembers[0];
                                    break;
                                }
                            }
                        }
                    }
                    if ((object)symbol != null)
                    {
                        SourceNamedTypeSymbol obj = namedTypeSymbol as SourceNamedTypeSymbol;
                        if ((object)obj != null && obj.IsPartial)
                        {
                            SourceNamedTypeSymbol obj2 = symbol as SourceNamedTypeSymbol;
                            if ((object)obj2 != null && obj2.IsPartial)
                            {
                                diagnostics.Add(ErrorCode.ERR_PartialTypeKindConflict, current2.Locations.FirstOrNone(), current2);
                                goto IL_0178;
                            }
                        }
                        diagnostics.Add(ErrorCode.ERR_DuplicateNameInNS, current2.Locations.FirstOrNone(), key, @namespace);
                    }
                    goto IL_0178;
                IL_0178:
                    array[num] = current2;
                    if ((object)namedTypeSymbol != null)
                    {
                        Accessibility declaredAccessibility = namedTypeSymbol.DeclaredAccessibility;
                        if (declaredAccessibility != Accessibility.Public && declaredAccessibility != Accessibility.Internal)
                        {
                            diagnostics.Add(ErrorCode.ERR_NoNamespacePrivate, current2.Locations.FirstOrNone());
                        }
                    }
                }
            }
        }

        private NamespaceOrTypeSymbol BuildSymbol(MergedNamespaceOrTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
        {
            switch (declaration.Kind)
            {
                case DeclarationKind.Namespace:
                    return new SourceNamespaceSymbol(_module, this, (MergedNamespaceDeclaration)declaration, diagnostics);
                case DeclarationKind.Class:
                case DeclarationKind.Interface:
                case DeclarationKind.Struct:
                case DeclarationKind.Enum:
                case DeclarationKind.Delegate:
                case DeclarationKind.Record:
                case DeclarationKind.RecordStruct:
                    return new SourceNamedTypeSymbol(this, (MergedTypeDeclaration)declaration, diagnostics);
                case DeclarationKind.Script:
                case DeclarationKind.Submission:
                case DeclarationKind.ImplicitClass:
                    return new ImplicitNamedTypeSymbol(this, (MergedTypeDeclaration)declaration, diagnostics);
                case DeclarationKind.SimpleProgram:
                    return new SimpleProgramNamedTypeSymbol(this, (MergedTypeDeclaration)declaration, diagnostics);
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaration.Kind);
            }
        }

        private void RegisterDeclaredCorTypes()
        {
            AssemblySymbol containingAssembly = ContainingAssembly;
            if (!containingAssembly.KeepLookingForDeclaredSpecialTypes)
            {
                return;
            }
            foreach (ImmutableArray<NamespaceOrTypeSymbol> value in _nameToMembersMap.Values)
            {
                ImmutableArray<NamespaceOrTypeSymbol>.Enumerator enumerator2 = value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.SpecialType != 0)
                    {
                        containingAssembly.RegisterDeclaredSpecialType(namedTypeSymbol);
                        if (!containingAssembly.KeepLookingForDeclaredSpecialTypes)
                        {
                            return;
                        }
                    }
                }
            }
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsGlobalNamespace)
            {
                return true;
            }
            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _mergedDeclaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleNamespaceDeclaration current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                SyntaxReference syntaxReference = current.SyntaxReference;
                if (syntaxReference.SyntaxTree == tree)
                {
                    if (!definedWithinSpan.HasValue)
                    {
                        return true;
                    }
                    if (NamespaceDeclarationSyntaxReference.GetSyntax(syntaxReference, cancellationToken).FullSpan.IntersectsWith(definedWithinSpan.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
            ImmutableArray<Symbol> members;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompletionPart nextIncompletePart = _state.NextIncompletePart;
                switch (nextIncompletePart)
                {
                    case CompletionPart.Members:
                        GetNameToMembersMap();
                        break;
                    case CompletionPart.MembersCompleted:
                        {
                            SingleNamespaceDeclaration singleNamespaceDeclaration = null;
                            ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = _mergedDeclaration.Declarations.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                SingleNamespaceDeclaration current = enumerator.Current;
                                if ((locationOpt == null || locationOpt.SourceTree == current.SyntaxReference.SyntaxTree) && (current.HasGlobalUsings || current.HasUsings || current.HasExternAliases))
                                {
                                    singleNamespaceDeclaration = current;
                                    _aliasesAndUsings[current].Complete(this, current.SyntaxReference, cancellationToken);
                                }
                            }
                            if (IsGlobalNamespace && ((object)locationOpt == null || singleNamespaceDeclaration != null))
                            {
                                GetMergedGlobalAliasesAndUsings(null, cancellationToken).Complete(this, cancellationToken);
                            }
                            members = GetMembers();
                            bool flag = true;
                            if (DeclaringCompilation.Options.ConcurrentBuild)
                            {
                                RoslynParallel.For(0, members.Length, UICultureUtilities.WithCurrentUICulture(delegate (int i)
                                {
                                    Symbol.ForceCompleteMemberByLocation(locationOpt, members[i], cancellationToken);
                                }), cancellationToken);
                                ImmutableArray<Symbol>.Enumerator enumerator2 = members.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    if (!enumerator2.Current.HasComplete(CompletionPart.All))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ImmutableArray<Symbol>.Enumerator enumerator2 = members.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    Symbol current2 = enumerator2.Current;
                                    Symbol.ForceCompleteMemberByLocation(locationOpt, current2, cancellationToken);
                                    flag = flag && current2.HasComplete(CompletionPart.All);
                                }
                            }
                            if (flag)
                            {
                                _state.NotePartComplete(CompletionPart.MembersCompleted);
                                break;
                            }
                            CompletionPart part = ((locationOpt == null) ? CompletionPart.NamespaceSymbolAll : CompletionPart.Members);
                            _state.SpinWaitComplete(part, cancellationToken);
                            return;
                        }
                    case CompletionPart.None:
                        return;
                    default:
                        _state.NotePartComplete(CompletionPart.PropertySymbolAll | CompletionPart.ReturnTypeAttributes | CompletionPart.Parameters | CompletionPart.Type | CompletionPart.TypeParameters | CompletionPart.TypeMembers | CompletionPart.SynthesizedExplicitImplementations | CompletionPart.StartMemberChecks | CompletionPart.FinishMemberChecks);
                        break;
                }
                _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        internal override bool HasComplete(CompletionPart part)
        {
            return _state.HasComplete(part);
        }
    }
}
