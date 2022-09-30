using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class NamespaceSymbol : NamespaceOrTypeSymbol, INamespace, INamedEntity, INamespaceSymbolInternal, INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        private ImmutableArray<NamedTypeSymbol> _lazyTypesMightContainExtensionMethods;

        private string _lazyQualifiedName;

        INamespace INamespace.ContainingNamespace => AdaptedNamespaceSymbol.ContainingNamespace?.GetCciAdapter();

        string INamedEntity.Name => AdaptedNamespaceSymbol.MetadataName;

        internal NamespaceSymbol AdaptedNamespaceSymbol => this;

        public virtual bool IsGlobalNamespace => (object)ContainingNamespace == null;

        internal abstract NamespaceExtent Extent { get; }

        public NamespaceKind NamespaceKind => Extent.Kind;

        public CSharpCompilation ContainingCompilation
        {
            get
            {
                if (NamespaceKind != NamespaceKind.Compilation)
                {
                    return null;
                }
                return Extent.Compilation;
            }
        }

        public virtual ImmutableArray<NamespaceSymbol> ConstituentNamespaces => ImmutableArray.Create(this);

        public sealed override NamedTypeSymbol ContainingType => null;

        public abstract override AssemblySymbol ContainingAssembly { get; }

        internal override ModuleSymbol ContainingModule
        {
            get
            {
                NamespaceExtent extent = Extent;
                if (extent.Kind == NamespaceKind.Module)
                {
                    return extent.Module;
                }
                return null;
            }
        }

        public sealed override SymbolKind Kind => SymbolKind.Namespace;

        public sealed override bool IsImplicitlyDeclared => IsGlobalNamespace;

        public sealed override Accessibility DeclaredAccessibility => Accessibility.Public;

        public sealed override bool IsStatic => true;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsSealed => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal NamedTypeSymbol ImplicitType
        {
            get
            {
                ImmutableArray<NamedTypeSymbol> typeMembers = GetTypeMembers("<invalid-global-code>");
                if (typeMembers.Length == 0)
                {
                    return null;
                }
                return typeMembers[0];
            }
        }

        private ImmutableArray<NamedTypeSymbol> TypesMightContainExtensionMethods
        {
            get
            {
                ImmutableArray<NamedTypeSymbol> lazyTypesMightContainExtensionMethods = _lazyTypesMightContainExtensionMethods;
                if (lazyTypesMightContainExtensionMethods.IsDefault)
                {
                    _lazyTypesMightContainExtensionMethods = GetTypeMembersUnordered().WhereAsArray((NamedTypeSymbol t) => t.MightContainExtensionMethods);
                    lazyTypesMightContainExtensionMethods = _lazyTypesMightContainExtensionMethods;
                }
                return lazyTypesMightContainExtensionMethods;
            }
        }

        internal string QualifiedName => _lazyQualifiedName ?? (_lazyQualifiedName = ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat));

        bool INamespaceSymbolInternal.IsGlobalNamespace => IsGlobalNamespace;

        INamespaceSymbolInternal INamespace.GetInternalSymbol()
        {
            return AdaptedNamespaceSymbol;
        }

        internal new NamespaceSymbol GetCciAdapter()
        {
            return this;
        }

        public IEnumerable<NamespaceSymbol> GetNamespaceMembers()
        {
            return GetMembers().OfType<NamespaceSymbol>();
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitNamespace(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitNamespace(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitNamespace(this);
        }

        internal NamespaceSymbol()
        {
        }

        internal NamespaceSymbol LookupNestedNamespace(ImmutableArray<string> names)
        {
            NamespaceSymbol namespaceSymbol = this;
            ImmutableArray<string>.Enumerator enumerator = names.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                NamespaceSymbol namespaceSymbol2 = null;
                ImmutableArray<Symbol>.Enumerator enumerator2 = namespaceSymbol.GetMembers(current).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if ((NamespaceOrTypeSymbol)enumerator2.Current is NamespaceSymbol namespaceSymbol3)
                    {
                        if ((object)namespaceSymbol2 != null)
                        {
                            namespaceSymbol2 = null;
                            break;
                        }
                        namespaceSymbol2 = namespaceSymbol3;
                    }
                }
                namespaceSymbol = namespaceSymbol2;
                if ((object)namespaceSymbol == null)
                {
                    break;
                }
            }
            return namespaceSymbol;
        }

        internal NamespaceSymbol GetNestedNamespace(string name)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Namespace)
                {
                    return (NamespaceSymbol)current;
                }
            }
            return null;
        }

        internal NamespaceSymbol GetNestedNamespace(NameSyntax name)
        {
            switch (name.Kind())
            {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                    return GetNestedNamespace(((SimpleNameSyntax)name).Identifier.ValueText);
                case SyntaxKind.QualifiedName:
                    {
                        QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)name;
                        NamespaceSymbol nestedNamespace = GetNestedNamespace(qualifiedNameSyntax.Left);
                        if ((object)nestedNamespace != null)
                        {
                            return nestedNamespace.GetNestedNamespace(qualifiedNameSyntax.Right);
                        }
                        break;
                    }
                case SyntaxKind.AliasQualifiedName:
                    return GetNestedNamespace(name.GetUnqualifiedName().Identifier.ValueText);
            }
            return null;
        }

        internal virtual void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string nameOpt, int arity, LookupOptions options)
        {
            if (ContainingAssembly.MightContainExtensionMethods)
            {
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = TypesMightContainExtensionMethods.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.DoGetExtensionMethods(methods, nameOpt, arity, options);
                }
            }
        }

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.NamespaceSymbol(this);
        }
    }
}
