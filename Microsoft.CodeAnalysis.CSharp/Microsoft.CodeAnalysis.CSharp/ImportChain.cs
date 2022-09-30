using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal sealed class ImportChain : IImportScope
    {
        public readonly Imports Imports;

        public readonly ImportChain ParentOpt;

        private ImmutableArray<UsedNamespaceOrType> _lazyTranslatedImports;

        IImportScope IImportScope.Parent => ParentOpt;

        public ImportChain(Imports imports, ImportChain parentOpt)
        {
            Imports = imports;
            ParentOpt = parentOpt;
        }

        private string GetDebuggerDisplay()
        {
            return $"{Imports.GetDebuggerDisplay()} ^ {ParentOpt?.GetHashCode() ?? 0}";
        }

        ImmutableArray<UsedNamespaceOrType> IImportScope.GetUsedNamespaces()
        {
            return _lazyTranslatedImports;
        }

        public IImportScope Translate(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            ImportChain importChain = this;
            while (importChain != null && importChain._lazyTranslatedImports.IsDefault)
            {
                ImmutableInterlocked.InterlockedInitialize(ref importChain._lazyTranslatedImports, importChain.TranslateImports(moduleBuilder, diagnostics));
                importChain = importChain.ParentOpt;
            }
            return this;
        }

        private ImmutableArray<UsedNamespaceOrType> TranslateImports(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            ArrayBuilder<UsedNamespaceOrType> instance = ArrayBuilder<UsedNamespaceOrType>.GetInstance();
            ImmutableArray<AliasAndExternAliasDirective> externAliases = Imports.ExternAliases;
            if (!externAliases.IsDefault)
            {
                ImmutableArray<AliasAndExternAliasDirective>.Enumerator enumerator = externAliases.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    instance.Add(UsedNamespaceOrType.CreateExternAlias(enumerator.Current.Alias.Name));
                }
            }
            ImmutableArray<NamespaceOrTypeAndUsingDirective> usings = Imports.Usings;
            if (!usings.IsDefault)
            {
                ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator2 = usings.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    NamespaceOrTypeAndUsingDirective current = enumerator2.Current;
                    NamespaceOrTypeSymbol namespaceOrType = current.NamespaceOrType;
                    if (namespaceOrType.IsNamespace)
                    {
                        NamespaceSymbol namespaceSymbol = (NamespaceSymbol)namespaceOrType;
                        IAssemblyReference assemblyOpt = TryGetAssemblyScope(namespaceSymbol, moduleBuilder, diagnostics);
                        instance.Add(UsedNamespaceOrType.CreateNamespace(namespaceSymbol.GetCciAdapter(), assemblyOpt));
                    }
                    else if (!namespaceOrType.ContainingAssembly.IsLinked)
                    {
                        ITypeReference typeReference = GetTypeReference((TypeSymbol)namespaceOrType, current.UsingDirective, moduleBuilder, diagnostics);
                        instance.Add(UsedNamespaceOrType.CreateType(typeReference));
                    }
                }
            }
            ImmutableDictionary<string, AliasAndUsingDirective> usingAliases = Imports.UsingAliases;
            if (!usingAliases.IsEmpty)
            {
                ArrayBuilder<string> instance2 = ArrayBuilder<string>.GetInstance(usingAliases.Count);
                instance2.AddRange(usingAliases.Keys);
                instance2.Sort(StringComparer.Ordinal);
                ArrayBuilder<string>.Enumerator enumerator3 = instance2.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    string current2 = enumerator3.Current;
                    AliasAndUsingDirective aliasAndUsingDirective = usingAliases[current2];
                    AliasSymbol alias = aliasAndUsingDirective.Alias;
                    UsingDirectiveSyntax usingDirective = aliasAndUsingDirective.UsingDirective;
                    NamespaceOrTypeSymbol target = alias.Target;
                    if (target.Kind == SymbolKind.Namespace)
                    {
                        NamespaceSymbol namespaceSymbol2 = (NamespaceSymbol)target;
                        IAssemblyReference assemblyOpt2 = TryGetAssemblyScope(namespaceSymbol2, moduleBuilder, diagnostics);
                        instance.Add(UsedNamespaceOrType.CreateNamespace(namespaceSymbol2.GetCciAdapter(), assemblyOpt2, current2));
                    }
                    else if (!target.ContainingAssembly.IsLinked)
                    {
                        ITypeReference typeReference2 = GetTypeReference((TypeSymbol)target, usingDirective, moduleBuilder, diagnostics);
                        instance.Add(UsedNamespaceOrType.CreateType(typeReference2, current2));
                    }
                }
                instance2.Free();
            }
            return instance.ToImmutableAndFree();
        }

        private static ITypeReference GetTypeReference(TypeSymbol type, SyntaxNode syntaxNode, PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            return moduleBuilder.Translate(type, syntaxNode, diagnostics);
        }

        private static IAssemblyReference TryGetAssemblyScope(NamespaceSymbol @namespace, PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
        {
            AssemblySymbol containingAssembly = @namespace.ContainingAssembly;
            if ((object)containingAssembly != null && (object)containingAssembly != moduleBuilder.CommonCompilation.Assembly)
            {
                var referenceManager = ((CSharpCompilation)moduleBuilder.CommonCompilation).GetBoundReferenceManager();

                for (int i = 0; i < referenceManager.ReferencedAssemblies.Length; i++)
                {
                    if ((object)referenceManager.ReferencedAssemblies[i] == containingAssembly)
                    {
                        if (!referenceManager.DeclarationsAccessibleWithoutAlias(i))
                        {
                            return moduleBuilder.Translate(containingAssembly, diagnostics);
                        }
                    }
                }
            }

            return null;
        }
    }
}
