using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class NamespaceOrTypeSymbol : Symbol, INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        public bool IsNamespace => Kind == SymbolKind.Namespace;

        public bool IsType => !IsNamespace;

        public sealed override bool IsVirtual => false;

        public sealed override bool IsOverride => false;

        public sealed override bool IsExtern => false;

        internal NamespaceOrTypeSymbol()
        {
        }

        public abstract ImmutableArray<Symbol> GetMembers();

        internal virtual ImmutableArray<Symbol> GetMembersUnordered()
        {
            return GetMembers().ConditionallyDeOrder();
        }

        public abstract ImmutableArray<Symbol> GetMembers(string name);

        internal virtual ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
        {
            return GetTypeMembers().ConditionallyDeOrder();
        }

        public abstract ImmutableArray<NamedTypeSymbol> GetTypeMembers();

        public abstract ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name);

        public virtual ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol t, int arity) => t.Arity == arity, arity);
        }

        internal SourceNamedTypeSymbol? GetSourceTypeMember(TypeDeclarationSyntax syntax)
        {
            return GetSourceTypeMember(syntax.Identifier.ValueText, syntax.Arity, syntax.Kind(), syntax);
        }

        internal SourceNamedTypeSymbol? GetSourceTypeMember(DelegateDeclarationSyntax syntax)
        {
            return GetSourceTypeMember(syntax.Identifier.ValueText, syntax.Arity, syntax.Kind(), syntax);
        }

        internal SourceNamedTypeSymbol? GetSourceTypeMember(string name, int arity, SyntaxKind kind, CSharpSyntaxNode syntax)
        {
            TypeKind typeKind = kind.ToDeclarationKind().ToTypeKind();
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = GetTypeMembers(name, arity).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is SourceNamedTypeSymbol sourceNamedTypeSymbol) || sourceNamedTypeSymbol.TypeKind != typeKind)
                {
                    continue;
                }
                if (syntax != null)
                {
                    ImmutableArray<Location>.Enumerator enumerator2 = sourceNamedTypeSymbol.Locations.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Location current = enumerator2.Current;
                        if (current.IsInSource && current.SourceTree == syntax.SyntaxTree && syntax.Span.Contains(current.SourceSpan))
                        {
                            return sourceNamedTypeSymbol;
                        }
                    }
                    continue;
                }
                return sourceNamedTypeSymbol;
            }
            return null;
        }

        /// <summary>
        /// Lookup an immediately nested type referenced from metadata, names should be
        /// compared case-sensitively.
        /// </summary>
        /// <param name="emittedTypeName">
        /// Simple type name, possibly with generic name mangling.
        /// </param>
        /// <returns>
        /// Symbol for the type, or MissingMetadataSymbol if the type isn't found.
        /// </returns>
        internal virtual NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
        {
            NamespaceOrTypeSymbol scope = this;

            if (scope.Kind == SymbolKind.ErrorType)
            {
                return new MissingMetadataTypeSymbol.Nested((NamedTypeSymbol)scope, ref emittedTypeName);
            }

            NamedTypeSymbol? namedType = null;

            ImmutableArray<NamedTypeSymbol> namespaceOrTypeMembers;
            bool isTopLevel = scope.IsNamespace;

            if (emittedTypeName.IsMangled)
            {
                if (emittedTypeName.ForcedArity == -1 || emittedTypeName.ForcedArity == emittedTypeName.InferredArity)
                {
                    // Let's handle mangling case first.
                    namespaceOrTypeMembers = scope.GetTypeMembers(emittedTypeName.UnmangledTypeName);

                    foreach (var named in namespaceOrTypeMembers)
                    {
                        if (emittedTypeName.InferredArity == named.Arity && named.MangleName)
                        {
                            if ((object?)namedType != null)
                            {
                                namedType = null;
                                break;
                            }

                            namedType = named;
                        }
                    }
                }
            }

            // Now try lookup without removing generic arity mangling.
            int forcedArity = emittedTypeName.ForcedArity;

            if (emittedTypeName.UseCLSCompliantNameArityEncoding)
            {
                // Only types with arity 0 are acceptable, we already examined types with mangled names.
                if (emittedTypeName.InferredArity > 0)
                {
                    goto Done;
                }
                else if (forcedArity == -1)
                {
                    forcedArity = 0;
                }
                else if (forcedArity != 0)
                {
                    goto Done;
                }
            }

            namespaceOrTypeMembers = scope.GetTypeMembers(emittedTypeName.TypeName);

            foreach (var named in namespaceOrTypeMembers)
            {
                if (!named.MangleName && (forcedArity == -1 || forcedArity == named.Arity))
                {
                    if ((object?)namedType != null)
                    {
                        namedType = null;
                        break;
                    }

                    namedType = named;
                }
            }

        Done:
            if ((object?)namedType == null)
            {
                if (isTopLevel)
                {
                    return new MissingMetadataTypeSymbol.TopLevel(scope.ContainingModule, ref emittedTypeName);
                }
                else
                {
                    return new MissingMetadataTypeSymbol.Nested((NamedTypeSymbol)scope, ref emittedTypeName);
                }
            }

            return namedType;
        }

        internal IEnumerable<NamespaceOrTypeSymbol>? GetNamespaceOrTypeByQualifiedName(IEnumerable<string> qualifiedName)
        {
            NamespaceOrTypeSymbol namespaceOrTypeSymbol = this;
            IEnumerable<NamespaceOrTypeSymbol> enumerable = null;
            foreach (string item in qualifiedName)
            {
                if (enumerable != null)
                {
                    namespaceOrTypeSymbol = enumerable.OfMinimalArity();
                    if ((object)namespaceOrTypeSymbol == null)
                    {
                        return SpecializedCollections.EmptyEnumerable<NamespaceOrTypeSymbol>();
                    }
                }
                enumerable = namespaceOrTypeSymbol.GetMembers(item).OfType<NamespaceOrTypeSymbol>();
            }
            return enumerable;
        }
    }
}
