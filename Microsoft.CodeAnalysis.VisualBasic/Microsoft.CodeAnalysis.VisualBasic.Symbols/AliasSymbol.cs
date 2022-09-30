using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class AliasSymbol : Symbol, IAliasSymbol
	{
		private readonly NamespaceOrTypeSymbol _aliasTarget;

		private readonly string _aliasName;

		private readonly ImmutableArray<Location> _aliasLocations;

		private readonly Symbol _aliasContainer;

		public override string Name => _aliasName;

		public override SymbolKind Kind => SymbolKind.Alias;

		public NamespaceOrTypeSymbol Target => _aliasTarget;

		private INamespaceOrTypeSymbol IAliasSymbol_Target => Target;

		public override ImmutableArray<Location> Locations => _aliasLocations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<SimpleImportsClauseSyntax>(Locations);

		public override bool IsNotOverridable => false;

		public override bool IsMustOverride => false;

		public override bool IsOverrides => false;

		public override bool IsOverridable => false;

		public override bool IsShared => false;

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		public override Symbol ContainingSymbol => _aliasContainer;

		internal AliasSymbol(VisualBasicCompilation compilation, Symbol aliasContainer, string aliasName, NamespaceOrTypeSymbol aliasTarget, Location aliasLocation)
		{
			MergedNamespaceSymbol mergedNamespaceSymbol = aliasContainer as MergedNamespaceSymbol;
			NamespaceSymbol namespaceSymbol = null;
			if ((object)mergedNamespaceSymbol != null)
			{
				namespaceSymbol = mergedNamespaceSymbol.GetConstituentForCompilation(compilation);
			}
			_aliasContainer = namespaceSymbol ?? aliasContainer;
			_aliasTarget = aliasTarget;
			_aliasName = aliasName;
			_aliasLocations = ImmutableArray.Create(aliasLocation);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			return obj is AliasSymbol aliasSymbol && object.Equals(Locations.FirstOrDefault(), aliasSymbol.Locations.FirstOrDefault()) && (object)ContainingAssembly == aliasSymbol.ContainingAssembly;
		}

		public override int GetHashCode()
		{
			if (Locations.Length <= 0)
			{
				return Name.GetHashCode();
			}
			return Locations[0].GetHashCode();
		}

		internal override TResult Accept<TArg, TResult>(VisualBasicSymbolVisitor<TArg, TResult> visitor, TArg a)
		{
			return visitor.VisitAlias(this, a);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitAlias(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitAlias(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitAlias(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitAlias(this);
		}
	}
}
