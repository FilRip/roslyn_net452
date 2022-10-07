using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class OverriddenOrHiddenMembersHelpers
    {
        internal static OverriddenOrHiddenMembersResult MakeOverriddenOrHiddenMembers(this MethodSymbol member)
        {
            return MakeOverriddenOrHiddenMembersWorker(member);
        }

        internal static OverriddenOrHiddenMembersResult MakeOverriddenOrHiddenMembers(this PropertySymbol member)
        {
            return MakeOverriddenOrHiddenMembersWorker(member);
        }

        internal static OverriddenOrHiddenMembersResult MakeOverriddenOrHiddenMembers(this EventSymbol member)
        {
            return MakeOverriddenOrHiddenMembersWorker(member);
        }

        private static OverriddenOrHiddenMembersResult MakeOverriddenOrHiddenMembersWorker(Symbol member)
        {
            if (!CanOverrideOrHide(member))
            {
                return OverriddenOrHiddenMembersResult.Empty;
            }
            if (member.IsAccessor())
            {
                MethodSymbol methodSymbol = member as MethodSymbol;
                Symbol associatedSymbol = methodSymbol.AssociatedSymbol;
                if ((object)associatedSymbol != null)
                {
                    if (associatedSymbol.Kind == SymbolKind.Property)
                    {
                        return MakePropertyAccessorOverriddenOrHiddenMembers(methodSymbol, (PropertySymbol)associatedSymbol);
                    }
                    return MakeEventAccessorOverriddenOrHiddenMembers(methodSymbol, (EventSymbol)associatedSymbol);
                }
            }
            NamedTypeSymbol containingType = member.ContainingType;
            bool dangerous_IsFromSomeCompilation = member.Dangerous_IsFromSomeCompilation;
            if (containingType.IsInterface)
            {
                return MakeInterfaceOverriddenOrHiddenMembers(member, dangerous_IsFromSomeCompilation);
            }
            FindOverriddenOrHiddenMembers(member, containingType, dangerous_IsFromSomeCompilation, out var hiddenBuilder, out var overriddenMembers);
            ImmutableArray<Symbol> hiddenMembers = hiddenBuilder?.ToImmutableAndFree() ?? ImmutableArray<Symbol>.Empty;
            return OverriddenOrHiddenMembersResult.Create(overriddenMembers, hiddenMembers);
        }

        private static void FindOverriddenOrHiddenMembers(Symbol member, NamedTypeSymbol containingType, bool memberIsFromSomeCompilation,
            out ArrayBuilder<Symbol> hiddenBuilder,
            out ImmutableArray<Symbol> overriddenMembers)
        {
            Symbol bestMatch = null;
            hiddenBuilder = null;

            // A specific override exact match candidate, if one is known. This supports covariant returns, for which signature
            // matching is not sufficient. This member is treated as being as good as an exact match.
            Symbol knownOverriddenMember = member switch
            {
                MethodSymbol method => KnownOverriddenClassMethod(method),
                PEPropertySymbol { GetMethod: PEMethodSymbol { ExplicitlyOverriddenClassMethod: { AssociatedSymbol: PropertySymbol overriddenProperty } } } => overriddenProperty,
                RetargetingPropertySymbol { GetMethod: RetargetingMethodSymbol { ExplicitlyOverriddenClassMethod: { AssociatedSymbol: PropertySymbol overriddenProperty } } } => overriddenProperty,
                _ => null
            };

            for (NamedTypeSymbol currType = containingType.BaseTypeNoUseSiteDiagnostics;
                (object)currType != null && (object)bestMatch == null && hiddenBuilder == null;
                currType = currType.BaseTypeNoUseSiteDiagnostics)
            {
                bool unused;
                FindOverriddenOrHiddenMembersInType(
                    member,
                    memberIsFromSomeCompilation,
                    containingType,
                    knownOverriddenMember,
                    currType,
                    out bestMatch,
                    out unused,
                    out hiddenBuilder);
            }

            // Based on bestMatch, find other methods that will be overridden, hidden, or runtime overridden
            // (in bestMatch.ContainingType).
            FindRelatedMembers(member.IsOverride, memberIsFromSomeCompilation, member.Kind, bestMatch, out overriddenMembers, ref hiddenBuilder);
        }

        public static Symbol FindFirstHiddenMemberIfAny(Symbol member, bool memberIsFromSomeCompilation)
        {
            FindOverriddenOrHiddenMembers(member, member.ContainingType, memberIsFromSomeCompilation, out var hiddenBuilder, out var _);
            Symbol result = hiddenBuilder?.FirstOrDefault();
            hiddenBuilder?.Free();
            return result;
        }

        private static MethodSymbol KnownOverriddenClassMethod(MethodSymbol method)
        {
            if (!(method is PEMethodSymbol pEMethodSymbol))
            {
                if (method is RetargetingMethodSymbol retargetingMethodSymbol)
                {
                    return retargetingMethodSymbol.ExplicitlyOverriddenClassMethod;
                }
                return null;
            }
            return pEMethodSymbol.ExplicitlyOverriddenClassMethod;
        }

        private static OverriddenOrHiddenMembersResult MakePropertyAccessorOverriddenOrHiddenMembers(MethodSymbol accessor, PropertySymbol associatedProperty)
        {
            bool flag = accessor.MethodKind == MethodKind.PropertyGet;
            MethodSymbol methodSymbol = null;
            ArrayBuilder<Symbol> hiddenBuilder = null;
            OverriddenOrHiddenMembersResult overriddenOrHiddenMembers = associatedProperty.OverriddenOrHiddenMembers;
            ImmutableArray<Symbol>.Enumerator enumerator = overriddenOrHiddenMembers.HiddenMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Property)
                {
                    PropertySymbol propertySymbol = (PropertySymbol)current;
                    MethodSymbol methodSymbol2 = (flag ? propertySymbol.GetMethod : propertySymbol.SetMethod);
                    if ((object)methodSymbol2 != null)
                    {
                        AccessOrGetInstance(ref hiddenBuilder).Add(methodSymbol2);
                    }
                }
            }
            if (overriddenOrHiddenMembers.OverriddenMembers.Any())
            {
                PropertySymbol property = (PropertySymbol)overriddenOrHiddenMembers.OverriddenMembers[0];
                MethodSymbol methodSymbol3 = (flag ? property.GetOwnOrInheritedGetMethod() : property.GetOwnOrInheritedSetMethod());
                if ((object)methodSymbol3 != null)
                {
                    methodSymbol = methodSymbol3;
                }
            }
            bool accessorIsFromSomeCompilation = accessor.Dangerous_IsFromSomeCompilation;
            ImmutableArray<Symbol> overriddenMembers = ImmutableArray<Symbol>.Empty;
            if ((object)methodSymbol != null && IsOverriddenSymbolAccessible(methodSymbol, accessor.ContainingType) && isAccessorOverride(accessor, methodSymbol))
            {
                FindRelatedMembers(accessor.IsOverride, accessorIsFromSomeCompilation, accessor.Kind, methodSymbol, out overriddenMembers, ref hiddenBuilder);
            }
            ImmutableArray<Symbol> hiddenMembers = hiddenBuilder?.ToImmutableAndFree() ?? ImmutableArray<Symbol>.Empty;
            return OverriddenOrHiddenMembersResult.Create(overriddenMembers, hiddenMembers);
            bool isAccessorOverride(MethodSymbol accessor, MethodSymbol overriddenAccessor)
            {
                if (accessorIsFromSomeCompilation)
                {
                    return MemberSignatureComparer.CSharpAccessorOverrideComparer.Equals(accessor, overriddenAccessor);
                }
                if (overriddenAccessor.Equals(KnownOverriddenClassMethod(accessor), TypeCompareKind.AllIgnoreOptions))
                {
                    return true;
                }
                return MemberSignatureComparer.RuntimeSignatureComparer.Equals(accessor, overriddenAccessor);
            }
        }

        private static OverriddenOrHiddenMembersResult MakeEventAccessorOverriddenOrHiddenMembers(MethodSymbol accessor, EventSymbol associatedEvent)
        {
            bool flag = accessor.MethodKind == MethodKind.EventAdd;
            MethodSymbol methodSymbol = null;
            ArrayBuilder<Symbol> hiddenBuilder = null;
            OverriddenOrHiddenMembersResult overriddenOrHiddenMembers = associatedEvent.OverriddenOrHiddenMembers;
            ImmutableArray<Symbol>.Enumerator enumerator = overriddenOrHiddenMembers.HiddenMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Event)
                {
                    EventSymbol eventSymbol = (EventSymbol)current;
                    MethodSymbol methodSymbol2 = (flag ? eventSymbol.AddMethod : eventSymbol.RemoveMethod);
                    if ((object)methodSymbol2 != null)
                    {
                        AccessOrGetInstance(ref hiddenBuilder).Add(methodSymbol2);
                    }
                }
            }
            if (overriddenOrHiddenMembers.OverriddenMembers.Any())
            {
                MethodSymbol ownOrInheritedAccessor = ((EventSymbol)overriddenOrHiddenMembers.OverriddenMembers[0]).GetOwnOrInheritedAccessor(flag);
                if ((object)ownOrInheritedAccessor != null)
                {
                    methodSymbol = ownOrInheritedAccessor;
                }
            }
            bool dangerous_IsFromSomeCompilation = accessor.Dangerous_IsFromSomeCompilation;
            ImmutableArray<Symbol> overriddenMembers = ImmutableArray<Symbol>.Empty;
            if ((object)methodSymbol != null && IsOverriddenSymbolAccessible(methodSymbol, accessor.ContainingType) && (dangerous_IsFromSomeCompilation ? MemberSignatureComparer.CSharpAccessorOverrideComparer.Equals(accessor, methodSymbol) : MemberSignatureComparer.RuntimeSignatureComparer.Equals(accessor, methodSymbol)))
            {
                FindRelatedMembers(accessor.IsOverride, dangerous_IsFromSomeCompilation, accessor.Kind, methodSymbol, out overriddenMembers, ref hiddenBuilder);
            }
            ImmutableArray<Symbol> hiddenMembers = hiddenBuilder?.ToImmutableAndFree() ?? ImmutableArray<Symbol>.Empty;
            return OverriddenOrHiddenMembersResult.Create(overriddenMembers, hiddenMembers);
        }

        internal static OverriddenOrHiddenMembersResult MakeInterfaceOverriddenOrHiddenMembers(Symbol member, bool memberIsFromSomeCompilation)
        {
            NamedTypeSymbol containingType = member.ContainingType;
            PooledHashSet<NamedTypeSymbol> instance = PooledHashSet<NamedTypeSymbol>.GetInstance();
            PooledHashSet<NamedTypeSymbol> instance2 = PooledHashSet<NamedTypeSymbol>.GetInstance();
            ArrayBuilder<Symbol> builder = null;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = containingType.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (instance2.Contains(current))
                {
                    continue;
                }
                FindOverriddenOrHiddenMembersInType(member, memberIsFromSomeCompilation, containingType, null, current, out var currTypeBestMatch, out var currTypeHasSameKindNonMatch, out var hiddenBuilder);
                bool flag = (object)currTypeBestMatch != null;
                if (flag)
                {
                    ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NamedTypeSymbol current2 = enumerator2.Current;
                        instance2.Add(current2);
                    }
                    AccessOrGetInstance(ref builder).Add(currTypeBestMatch);
                }
                if (hiddenBuilder != null)
                {
                    if (!instance.Contains(current))
                    {
                        if (!flag)
                        {
                            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                NamedTypeSymbol current3 = enumerator2.Current;
                                instance2.Add(current3);
                            }
                        }
                        AccessOrGetInstance(ref builder).AddRange(hiddenBuilder);
                    }
                    hiddenBuilder.Free();
                }
                else if (currTypeHasSameKindNonMatch && !flag)
                {
                    ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NamedTypeSymbol current4 = enumerator2.Current;
                        instance.Add(current4);
                    }
                }
            }
            instance.Free();
            instance2.Free();
            ImmutableArray<Symbol> overriddenMembers = ImmutableArray<Symbol>.Empty;
            if (builder != null)
            {
                ArrayBuilder<Symbol> hiddenBuilder2 = null;
                ArrayBuilder<Symbol>.Enumerator enumerator3 = builder.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    Symbol current5 = enumerator3.Current;
                    FindRelatedMembers(member.IsOverride, memberIsFromSomeCompilation, member.Kind, current5, out overriddenMembers, ref hiddenBuilder2);
                }
                builder.Free();
                builder = hiddenBuilder2;
            }
            ImmutableArray<Symbol> hiddenMembers = builder?.ToImmutableAndFree() ?? ImmutableArray<Symbol>.Empty;
            return OverriddenOrHiddenMembersResult.Create(overriddenMembers, hiddenMembers);
        }

        private static void FindOverriddenOrHiddenMembersInType(Symbol member, bool memberIsFromSomeCompilation, NamedTypeSymbol memberContainingType, Symbol knownOverriddenMember, NamedTypeSymbol currType, out Symbol currTypeBestMatch, out bool currTypeHasSameKindNonMatch, out ArrayBuilder<Symbol> hiddenBuilder)
        {
            currTypeBestMatch = null;
            currTypeHasSameKindNonMatch = false;
            hiddenBuilder = null;
            bool flag = false;
            int num = int.MaxValue;
            IEqualityComparer<Symbol> equalityComparer = (memberIsFromSomeCompilation ? MemberSignatureComparer.CSharpCustomModifierOverrideComparer : MemberSignatureComparer.RuntimePlusRefOutSignatureComparer);
            IEqualityComparer<Symbol> equalityComparer2 = (memberIsFromSomeCompilation ? MemberSignatureComparer.CSharpOverrideComparer : MemberSignatureComparer.RuntimeSignatureComparer);
            SymbolKind kind = member.Kind;
            int memberArity = member.GetMemberArity();
            ImmutableArray<Symbol>.Enumerator enumerator = currType.GetMembers(member.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (!IsOverriddenSymbolAccessible(current, memberContainingType) || (current.IsAccessor() && !((MethodSymbol)current).IsIndexedPropertyAccessor()))
                {
                    continue;
                }
                if (current.Kind != kind)
                {
                    int memberArity2 = current.GetMemberArity();
                    if (memberArity2 == memberArity || (kind == SymbolKind.Method && memberArity2 == 0))
                    {
                        AddHiddenMemberIfApplicable(ref hiddenBuilder, kind, current);
                    }
                }
                else
                {
                    if (flag)
                    {
                        continue;
                    }
                    switch (kind)
                    {
                        case SymbolKind.Field:
                            flag = true;
                            currTypeBestMatch = current;
                            continue;
                        case SymbolKind.NamedType:
                            if (current.GetMemberArity() == memberArity)
                            {
                                flag = true;
                                currTypeBestMatch = current;
                            }
                            continue;
                    }
                    if (current.Equals(knownOverriddenMember, TypeCompareKind.AllIgnoreOptions))
                    {
                        flag = true;
                        currTypeBestMatch = current;
                        continue;
                    }
                    if (!(knownOverriddenMember == null))
                    {
                        continue;
                    }
                    if (equalityComparer.Equals(member, current))
                    {
                        flag = true;
                        currTypeBestMatch = current;
                    }
                    else if (equalityComparer2.Equals(member, current))
                    {
                        int num2 = CustomModifierCount(current);
                        if (num2 < num)
                        {
                            num = num2;
                            currTypeBestMatch = current;
                        }
                    }
                    else
                    {
                        currTypeHasSameKindNonMatch = true;
                    }
                }
            }
            if (kind == SymbolKind.Field || kind == SymbolKind.NamedType || !(flag && memberIsFromSomeCompilation) || !member.IsDefinition || !TypeOrReturnTypeHasCustomModifiers(currTypeBestMatch))
            {
                return;
            }
            Symbol symbol = currTypeBestMatch;
            enumerator = currType.GetMembers(member.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current2 = enumerator.Current;
                if (current2.Kind == currTypeBestMatch.Kind && (object)current2 != currTypeBestMatch && MemberSignatureComparer.CSharpOverrideComparer.Equals(current2, currTypeBestMatch))
                {
                    int num3 = CustomModifierCount(current2);
                    if (num3 < num)
                    {
                        num = num3;
                        symbol = current2;
                    }
                }
            }
            currTypeBestMatch = symbol;
        }

        private static void FindRelatedMembers(bool isOverride, bool overridingMemberIsFromSomeCompilation, SymbolKind overridingMemberKind, Symbol representativeMember, out ImmutableArray<Symbol> overriddenMembers, ref ArrayBuilder<Symbol> hiddenBuilder)
        {
            overriddenMembers = ImmutableArray<Symbol>.Empty;
            if ((object)representativeMember == null)
            {
                return;
            }
            bool flag = representativeMember.Kind != SymbolKind.Field && representativeMember.Kind != SymbolKind.NamedType && (!representativeMember.ContainingType.IsDefinition || representativeMember.IsIndexer());
            if (isOverride)
            {
                if (flag)
                {
                    ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
                    instance.Add(representativeMember);
                    FindOtherOverriddenMethodsInContainingType(representativeMember, overridingMemberIsFromSomeCompilation, instance);
                    overriddenMembers = instance.ToImmutableAndFree();
                }
                else
                {
                    overriddenMembers = ImmutableArray.Create(representativeMember);
                }
            }
            else
            {
                AddHiddenMemberIfApplicable(ref hiddenBuilder, overridingMemberKind, representativeMember);
                if (flag)
                {
                    FindOtherHiddenMembersInContainingType(overridingMemberKind, representativeMember, ref hiddenBuilder);
                }
            }
        }

        private static void AddHiddenMemberIfApplicable(ref ArrayBuilder<Symbol> hiddenBuilder, SymbolKind hidingMemberKind, Symbol hiddenMember)
        {
            if (hiddenMember.Kind != SymbolKind.Method || ((MethodSymbol)hiddenMember).CanBeHiddenByMemberKind(hidingMemberKind))
            {
                AccessOrGetInstance(ref hiddenBuilder).Add(hiddenMember);
            }
        }

        private static ArrayBuilder<T> AccessOrGetInstance<T>(ref ArrayBuilder<T> builder)
        {
            if (builder == null)
            {
                builder = ArrayBuilder<T>.GetInstance();
            }
            return builder;
        }

        private static void FindOtherOverriddenMethodsInContainingType(Symbol representativeMember, bool overridingMemberIsFromSomeCompilation, ArrayBuilder<Symbol> overriddenBuilder)
        {
            int num = -1;
            ImmutableArray<Symbol>.Enumerator enumerator = representativeMember.ContainingType.GetMembers(representativeMember.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != representativeMember.Kind || !(current != representativeMember))
                {
                    continue;
                }
                if (overridingMemberIsFromSomeCompilation)
                {
                    if (num < 0)
                    {
                        num = representativeMember.CustomModifierCount();
                    }
                    if (MemberSignatureComparer.CSharpOverrideComparer.Equals(current, representativeMember) && current.CustomModifierCount() == num)
                    {
                        overriddenBuilder.Add(current);
                    }
                }
                else if (MemberSignatureComparer.CSharpCustomModifierOverrideComparer.Equals(current, representativeMember))
                {
                    overriddenBuilder.Add(current);
                }
            }
        }

        private static void FindOtherHiddenMembersInContainingType(SymbolKind hidingMemberKind, Symbol representativeMember, ref ArrayBuilder<Symbol> hiddenBuilder)
        {
            IEqualityComparer<Symbol> cSharpCustomModifierOverrideComparer = MemberSignatureComparer.CSharpCustomModifierOverrideComparer;
            ImmutableArray<Symbol>.Enumerator enumerator = representativeMember.ContainingType.GetMembers(representativeMember.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == representativeMember.Kind && current != representativeMember && cSharpCustomModifierOverrideComparer.Equals(current, representativeMember))
                {
                    AddHiddenMemberIfApplicable(ref hiddenBuilder, hidingMemberKind, current);
                }
            }
        }

        private static bool CanOverrideOrHide(Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Property:
                    return !member.IsExplicitInterfaceImplementation();
                case SymbolKind.Method:
                    {
                        MethodSymbol methodSymbol = (MethodSymbol)member;
                        if (MethodSymbol.CanOverrideOrHide(methodSymbol.MethodKind))
                        {
                            return (object)methodSymbol == methodSymbol.ConstructedFrom;
                        }
                        return false;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        private static bool TypeOrReturnTypeHasCustomModifiers(Symbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    {
                        MethodSymbol methodSymbol = (MethodSymbol)member;
                        TypeWithAnnotations returnTypeWithAnnotations = methodSymbol.ReturnTypeWithAnnotations;
                        if (!returnTypeWithAnnotations.CustomModifiers.Any() && !methodSymbol.RefCustomModifiers.Any())
                        {
                            return returnTypeWithAnnotations.Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds: false);
                        }
                        return true;
                    }
                case SymbolKind.Property:
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)member;
                        TypeWithAnnotations typeWithAnnotations = propertySymbol.TypeWithAnnotations;
                        if (!typeWithAnnotations.CustomModifiers.Any() && !propertySymbol.RefCustomModifiers.Any())
                        {
                            return typeWithAnnotations.Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds: false);
                        }
                        return true;
                    }
                case SymbolKind.Event:
                    return ((EventSymbol)member).Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds: false);
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
            }
        }

        private static int CustomModifierCount(Symbol member)
        {
            return member.Kind switch
            {
                SymbolKind.Method => ((MethodSymbol)member).CustomModifierCount(),
                SymbolKind.Property => ((PropertySymbol)member).CustomModifierCount(),
                SymbolKind.Event => ((EventSymbol)member).Type.CustomModifierCount(),
                _ => throw ExceptionUtilities.UnexpectedValue(member.Kind),
            };
        }

        internal static bool RequiresExplicitOverride(this MethodSymbol method, out bool warnAmbiguous)
        {
            warnAmbiguous = false;
            if (!method.IsOverride)
            {
                return false;
            }
            MethodSymbol overriddenMethod = method.OverriddenMethod;
            if ((object)overriddenMethod == null)
            {
                return false;
            }
            MethodSymbol firstRuntimeOverriddenMethodIgnoringNewSlot = method.GetFirstRuntimeOverriddenMethodIgnoringNewSlot(out bool wasAmbiguous);
            if (overriddenMethod == firstRuntimeOverriddenMethodIgnoringNewSlot && !wasAmbiguous)
            {
                return false;
            }
            if (method.ContainingAssembly.RuntimeSupportsCovariantReturnsOfClasses)
            {
                return true;
            }
            if (!method.ReturnType.Equals(overriddenMethod.ReturnType, TypeCompareKind.AllIgnoreOptions))
            {
                return true;
            }
            if (!overriddenMethod.MethodHasRuntimeCollision())
            {
                return true;
            }
            bool flag = overriddenMethod.IsDefinition || overriddenMethod.OriginalDefinition.MethodHasRuntimeCollision();
            warnAmbiguous = !flag;
            if (!overriddenMethod.ContainingType.Equals(firstRuntimeOverriddenMethodIgnoringNewSlot.ContainingType, TypeCompareKind.CLRSignatureCompareOptions))
            {
                return true;
            }
            if (overriddenMethod != firstRuntimeOverriddenMethodIgnoringNewSlot)
            {
                return method.IsAccessor() != firstRuntimeOverriddenMethodIgnoringNewSlot.IsAccessor();
            }
            return false;
        }

        internal static bool MethodHasRuntimeCollision(this MethodSymbol method)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = method.ContainingType.GetMembers(method.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current != method && MemberSignatureComparer.RuntimeSignatureComparer.Equals(current, method))
                {
                    return true;
                }
            }
            return false;
        }

        internal static MethodSymbol GetFirstRuntimeOverriddenMethodIgnoringNewSlot(this MethodSymbol method, out bool wasAmbiguous)
        {
            wasAmbiguous = false;
            if (!method.IsMetadataVirtual(ignoreInterfaceImplementationChanges: true))
            {
                return null;
            }
            NamedTypeSymbol containingType = method.ContainingType;
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
            while ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                MethodSymbol methodSymbol = null;
                ImmutableArray<Symbol>.Enumerator enumerator = baseTypeNoUseSiteDiagnostics.GetMembers(method.Name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind != SymbolKind.Method || !IsOverriddenSymbolAccessible(current, containingType) || !MemberSignatureComparer.RuntimeSignatureComparer.Equals(method, current))
                    {
                        continue;
                    }
                    MethodSymbol methodSymbol2 = (MethodSymbol)current;
                    if (methodSymbol2.IsMetadataVirtual(ignoreInterfaceImplementationChanges: true))
                    {
                        if ((object)methodSymbol != null)
                        {
                            wasAmbiguous = true;
                            return methodSymbol;
                        }
                        methodSymbol = methodSymbol2;
                    }
                }
                if ((object)methodSymbol != null)
                {
                    return methodSymbol;
                }
                baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
            }
            return null;
        }

        private static bool IsOverriddenSymbolAccessible(Symbol overridden, NamedTypeSymbol overridingContainingType)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return AccessCheck.IsSymbolAccessible(overridden.OriginalDefinition, overridingContainingType.OriginalDefinition, ref useSiteInfo);
        }
    }
}
