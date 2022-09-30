using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class OverriddenMembersResult<TSymbol> where TSymbol : Symbol
	{
		public static readonly OverriddenMembersResult<TSymbol> Empty = new OverriddenMembersResult<TSymbol>(ImmutableArray<TSymbol>.Empty, ImmutableArray<TSymbol>.Empty, ImmutableArray<TSymbol>.Empty);

		private readonly ImmutableArray<TSymbol> _overriddenMembers;

		private readonly ImmutableArray<TSymbol> _inexactOverriddenMembers;

		private readonly ImmutableArray<TSymbol> _inaccessibleMembers;

		public ImmutableArray<TSymbol> OverriddenMembers => _overriddenMembers;

		public ImmutableArray<TSymbol> InexactOverriddenMembers => _inexactOverriddenMembers;

		public ImmutableArray<TSymbol> InaccessibleMembers => _inaccessibleMembers;

		public TSymbol OverriddenMember
		{
			get
			{
				ImmutableArray<TSymbol>.Enumerator enumerator = OverriddenMembers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TSymbol current = enumerator.Current;
					if (current.IsMustOverride || current.IsOverridable || current.IsOverrides)
					{
						return current;
					}
				}
				return null;
			}
		}

		private OverriddenMembersResult(ImmutableArray<TSymbol> overriddenMembers, ImmutableArray<TSymbol> inexactOverriddenMembers, ImmutableArray<TSymbol> inaccessibleMembers)
		{
			_overriddenMembers = overriddenMembers;
			_inexactOverriddenMembers = inexactOverriddenMembers;
			_inaccessibleMembers = inaccessibleMembers;
		}

		public static OverriddenMembersResult<TSymbol> Create(ImmutableArray<TSymbol> overriddenMembers, ImmutableArray<TSymbol> inexactOverriddenMembers, ImmutableArray<TSymbol> inaccessibleMembers)
		{
			if (overriddenMembers.IsEmpty && inexactOverriddenMembers.IsEmpty && inaccessibleMembers.IsEmpty)
			{
				return Empty;
			}
			return new OverriddenMembersResult<TSymbol>(overriddenMembers, inexactOverriddenMembers, inaccessibleMembers);
		}

		public static TSymbol GetOverriddenMember(TSymbol substitutedOverridingMember, TSymbol overriddenByDefinitionMember)
		{
			if (overriddenByDefinitionMember != null)
			{
				NamedTypeSymbol containingType = overriddenByDefinitionMember.ContainingType;
				NamedTypeSymbol originalDefinition = containingType.OriginalDefinition;
				NamedTypeSymbol baseTypeNoUseSiteDiagnostics = substitutedOverridingMember.ContainingType.BaseTypeNoUseSiteDiagnostics;
				while ((object)baseTypeNoUseSiteDiagnostics != null)
				{
					if (TypeSymbol.Equals(baseTypeNoUseSiteDiagnostics.OriginalDefinition, originalDefinition, TypeCompareKind.ConsiderEverything))
					{
						if (TypeSymbol.Equals(baseTypeNoUseSiteDiagnostics, containingType, TypeCompareKind.ConsiderEverything))
						{
							return overriddenByDefinitionMember;
						}
						return (TSymbol)SymbolExtensions.AsMember(overriddenByDefinitionMember.OriginalDefinition, baseTypeNoUseSiteDiagnostics);
					}
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
				}
				throw ExceptionUtilities.Unreachable;
			}
			return null;
		}
	}
}
