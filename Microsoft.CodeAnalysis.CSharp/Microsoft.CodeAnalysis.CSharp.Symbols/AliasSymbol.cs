using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class AliasSymbol : Symbol
    {
        private readonly ImmutableArray<Location> _locations;

        private readonly string _aliasName;

        private readonly bool _isExtern;

        private readonly Symbol? _containingSymbol;

        public sealed override string Name => _aliasName;

        public override SymbolKind Kind => SymbolKind.Alias;

        public abstract NamespaceOrTypeSymbol Target { get; }

        public override ImmutableArray<Location> Locations => _locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<UsingDirectiveSyntax>(_locations);

        public sealed override bool IsExtern => _isExtern;

        public override bool IsSealed => false;

        public override bool IsAbstract => false;

        public override bool IsOverride => false;

        public override bool IsVirtual => false;

        public override bool IsStatic => false;

        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData => null;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public sealed override Symbol? ContainingSymbol => _containingSymbol;

        internal abstract override bool RequiresCompletion { get; }

        protected AliasSymbol(string aliasName, Symbol? containingSymbol, ImmutableArray<Location> locations, bool isExtern)
        {
            _locations = locations;
            _aliasName = aliasName;
            _isExtern = isExtern;
            _containingSymbol = containingSymbol;
        }

        internal static AliasSymbol CreateGlobalNamespaceAlias(NamespaceSymbol globalNamespace)
        {
            return new AliasSymbolFromResolvedTarget(globalNamespace, "global", globalNamespace, ImmutableArray<Location>.Empty, isExtern: false);
        }

        internal static AliasSymbol CreateCustomDebugInfoAlias(NamespaceOrTypeSymbol targetSymbol, SyntaxToken aliasToken, Symbol? containingSymbol, bool isExtern)
        {
            return new AliasSymbolFromResolvedTarget(targetSymbol, aliasToken.ValueText, containingSymbol, ImmutableArray.Create(aliasToken.GetLocation()), isExtern);
        }

        internal AliasSymbol ToNewSubmission(CSharpCompilation compilation)
        {
            NamespaceOrTypeSymbol target = Target;
            if (target.Kind != SymbolKind.Namespace)
            {
                return this;
            }
            NamespaceSymbol globalNamespace = compilation.GlobalNamespace;
            return new AliasSymbolFromResolvedTarget(Imports.ExpandPreviousSubmissionNamespace((NamespaceSymbol)target, globalNamespace), Name, ContainingSymbol, _locations, _isExtern);
        }

        internal override TResult Accept<TArg, TResult>(CSharpSymbolVisitor<TArg, TResult> visitor, TArg a)
        {
            return visitor.VisitAlias(this, a);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitAlias(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitAlias(this);
        }

        internal abstract NamespaceOrTypeSymbol GetAliasTarget(ConsList<TypeSymbol>? basesBeingResolved);

        internal void CheckConstraints(BindingDiagnosticBag diagnostics)
        {
            if (Target is TypeSymbol type && Locations.Length > 0)
            {
                TypeConversions conversions = new(ContainingAssembly.CorLibrary);
                type.CheckAllConstraints(DeclaringCompilation, conversions, Locations[0], diagnostics);
            }
        }

        public override bool Equals(Symbol? obj, TypeCompareKind compareKind)
        {
            if ((object)this == obj)
            {
                return true;
            }
            if ((object)obj == null)
            {
                return false;
            }
            if (obj is AliasSymbol aliasSymbol && object.Equals(Locations.FirstOrDefault(), aliasSymbol.Locations.FirstOrDefault()))
            {
                return ContainingAssembly.Equals(aliasSymbol.ContainingAssembly, compareKind);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Locations.Length > 0)
            {
                return Locations.First().GetHashCode();
            }
            return Name.GetHashCode();
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.AliasSymbol(this);
        }
    }
}
