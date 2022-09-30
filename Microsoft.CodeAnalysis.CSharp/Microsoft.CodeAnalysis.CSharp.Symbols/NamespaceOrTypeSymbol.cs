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

        internal virtual NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
        {
            if (Kind == SymbolKind.ErrorType)
            {
                return new MissingMetadataTypeSymbol.Nested((NamedTypeSymbol)this, ref emittedTypeName);
            }
            NamedTypeSymbol namedTypeSymbol = null;
            bool isNamespace = IsNamespace;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator;
            if (emittedTypeName.IsMangled && (emittedTypeName.ForcedArity == -1 || emittedTypeName.ForcedArity == emittedTypeName.InferredArity))
            {
                enumerator = GetTypeMembers(emittedTypeName.UnmangledTypeName).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamedTypeSymbol current = enumerator.Current;
                    if (emittedTypeName.InferredArity == current.Arity && current.MangleName)
                    {
                        if ((object)namedTypeSymbol != null)
                        {
                            namedTypeSymbol = null;
                            break;
                        }
                        namedTypeSymbol = current;
                    }
                }
            }
            int num = emittedTypeName.ForcedArity;
            if (emittedTypeName.UseCLSCompliantNameArityEncoding)
            {
                if (emittedTypeName.InferredArity > 0)
                {
                    goto IL_0100;
                }
                if (num == -1)
                {
                    num = 0;
                }
                else if (num != 0)
                {
                    goto IL_0100;
                }
            }
            enumerator = GetTypeMembers(emittedTypeName.TypeName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current2 = enumerator.Current;
                if (!current2.MangleName && (num == -1 || num == current2.Arity))
                {
                    if ((object)namedTypeSymbol != null)
                    {
                        namedTypeSymbol = null;
                        break;
                    }
                    namedTypeSymbol = current2;
                }
            }
            goto IL_0100;
        IL_0100:
            if ((object)namedTypeSymbol == null)
            {
                if (isNamespace)
                {
                    return new MissingMetadataTypeSymbol.TopLevel(ContainingModule, ref emittedTypeName);
                }
                return new MissingMetadataTypeSymbol.Nested((NamedTypeSymbol)this, ref emittedTypeName);
            }
            return namedTypeSymbol;
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
