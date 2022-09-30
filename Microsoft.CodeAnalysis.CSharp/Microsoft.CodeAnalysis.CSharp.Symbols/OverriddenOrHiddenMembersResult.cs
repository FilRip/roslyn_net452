using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class OverriddenOrHiddenMembersResult
    {
        public static readonly OverriddenOrHiddenMembersResult Empty = new OverriddenOrHiddenMembersResult(ImmutableArray<Symbol>.Empty, ImmutableArray<Symbol>.Empty);

        private readonly ImmutableArray<Symbol> _overriddenMembers;

        private readonly ImmutableArray<Symbol> _hiddenMembers;

        public ImmutableArray<Symbol> OverriddenMembers => _overriddenMembers;

        public ImmutableArray<Symbol> HiddenMembers => _hiddenMembers;

        private OverriddenOrHiddenMembersResult(ImmutableArray<Symbol> overriddenMembers, ImmutableArray<Symbol> hiddenMembers)
        {
            _overriddenMembers = overriddenMembers;
            _hiddenMembers = hiddenMembers;
        }

        public static OverriddenOrHiddenMembersResult Create(ImmutableArray<Symbol> overriddenMembers, ImmutableArray<Symbol> hiddenMembers)
        {
            if (overriddenMembers.IsEmpty && hiddenMembers.IsEmpty)
            {
                return Empty;
            }
            return new OverriddenOrHiddenMembersResult(overriddenMembers, hiddenMembers);
        }

        internal static Symbol GetOverriddenMember(Symbol substitutedOverridingMember, Symbol overriddenByDefinitionMember)
        {
            if ((object)overriddenByDefinitionMember != null)
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
                        return overriddenByDefinitionMember.OriginalDefinition.SymbolAsMember(baseTypeNoUseSiteDiagnostics);
                    }
                    baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
                }
                throw ExceptionUtilities.Unreachable;
            }
            return null;
        }

        internal Symbol GetOverriddenMember()
        {
            ImmutableArray<Symbol>.Enumerator enumerator = _overriddenMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.IsAbstract || current.IsVirtual || current.IsOverride)
                {
                    return current;
                }
            }
            return null;
        }
    }
}
