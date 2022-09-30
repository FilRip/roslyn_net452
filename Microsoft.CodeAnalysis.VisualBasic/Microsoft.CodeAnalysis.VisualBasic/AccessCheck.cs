using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AccessCheck
	{
		private struct AccessExposure
		{
			public TypeSymbol ExposedType;

			public NamespaceOrTypeSymbol ExposedTo;
		}

		private static readonly Accessibility[] s_mapAccessToAccessOutsideAssembly;

		private AccessCheck()
		{
		}

		public static bool IsSymbolAccessible(Symbol symbol, AssemblySymbol within, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			return CheckSymbolAccessibilityCore(symbol, within, null, basesBeingResolved, ref useSiteInfo) == AccessCheckResult.Accessible;
		}

		public static AccessCheckResult CheckSymbolAccessibility(Symbol symbol, AssemblySymbol within, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			return CheckSymbolAccessibilityCore(symbol, within, null, basesBeingResolved, ref useSiteInfo);
		}

		public static bool IsSymbolAccessible(Symbol symbol, NamedTypeSymbol within, TypeSymbol throughTypeOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			return CheckSymbolAccessibilityCore(symbol, within, throughTypeOpt, basesBeingResolved, ref useSiteInfo) == AccessCheckResult.Accessible;
		}

		public static AccessCheckResult CheckSymbolAccessibility(Symbol symbol, NamedTypeSymbol within, TypeSymbol throughTypeOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			return CheckSymbolAccessibilityCore(symbol, within, throughTypeOpt, basesBeingResolved, ref useSiteInfo);
		}

		private static AccessCheckResult CheckSymbolAccessibilityCore(Symbol symbol, Symbol within, TypeSymbol throughTypeOpt, BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!(within is AssemblySymbol))
			{
				_ = ((NamedTypeSymbol)within).ContainingAssembly;
			}
			switch (symbol.Kind)
			{
			case SymbolKind.ArrayType:
				return CheckSymbolAccessibilityCore(((ArrayTypeSymbol)symbol).ElementType, within, null, basesBeingResolved, ref useSiteInfo);
			case SymbolKind.NamedType:
				return CheckNamedTypeAccessibility((NamedTypeSymbol)symbol, within, basesBeingResolved, ref useSiteInfo);
			case SymbolKind.Alias:
				return CheckSymbolAccessibilityCore(((AliasSymbol)symbol).Target, within, null, basesBeingResolved, ref useSiteInfo);
			case SymbolKind.ErrorType:
				return AccessCheckResult.Accessible;
			case SymbolKind.Assembly:
			case SymbolKind.Label:
			case SymbolKind.Local:
			case SymbolKind.NetModule:
			case SymbolKind.Namespace:
			case SymbolKind.Parameter:
			case SymbolKind.RangeVariable:
			case SymbolKind.TypeParameter:
				return AccessCheckResult.Accessible;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
			case SymbolKind.Event:
			case SymbolKind.Field:
			case SymbolKind.Method:
			case SymbolKind.Property:
				if (symbol.IsShared)
				{
					throughTypeOpt = null;
				}
				return CheckMemberAccessibility(symbol.ContainingType, symbol.DeclaredAccessibility, within, throughTypeOpt, basesBeingResolved, ref useSiteInfo);
			}
		}

		private static AccessCheckResult CheckNamedTypeAccessibility(NamedTypeSymbol typeSym, Symbol within, BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!typeSym.IsDefinition)
			{
				ImmutableArray<TypeSymbol> immutableArray = typeSym.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
				int num = immutableArray.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (immutableArray[i].Kind != SymbolKind.TypeParameter)
					{
						AccessCheckResult accessCheckResult = CheckSymbolAccessibilityCore(immutableArray[i], within, null, basesBeingResolved, ref useSiteInfo);
						if (accessCheckResult != 0)
						{
							return accessCheckResult;
						}
					}
				}
			}
			if ((object)typeSym.ContainingType == null)
			{
				return CheckNonNestedTypeAccessibility(typeSym.ContainingAssembly, typeSym.DeclaredAccessibility, within);
			}
			return CheckMemberAccessibility(typeSym.ContainingType, typeSym.DeclaredAccessibility, within, null, basesBeingResolved, ref useSiteInfo);
		}

		private static AccessCheckResult CheckNonNestedTypeAccessibility(AssemblySymbol assembly, Accessibility declaredAccessibility, Symbol within)
		{
			AssemblySymbol fromAssembly = (within as AssemblySymbol) ?? ((NamedTypeSymbol)within).ContainingAssembly;
			switch (declaredAccessibility)
			{
			case Accessibility.NotApplicable:
			case Accessibility.Public:
				return AccessCheckResult.Accessible;
			case Accessibility.Private:
			case Accessibility.ProtectedAndInternal:
			case Accessibility.Protected:
				return AccessCheckResult.Accessible;
			case Accessibility.Internal:
			case Accessibility.ProtectedOrInternal:
				return (!HasFriendAccessTo(fromAssembly, assembly)) ? AccessCheckResult.Inaccessible : AccessCheckResult.Accessible;
			default:
				throw ExceptionUtilities.UnexpectedValue(declaredAccessibility);
			}
		}

		private static AccessCheckResult CheckMemberAccessibility(NamedTypeSymbol containingType, Accessibility declaredAccessibility, Symbol within, TypeSymbol throughTypeOpt, BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol originalDefinition = containingType.OriginalDefinition;
			NamedTypeSymbol namedTypeSymbol = within as NamedTypeSymbol;
			AssemblySymbol fromAssembly = (within as AssemblySymbol) ?? namedTypeSymbol.ContainingAssembly;
			AccessCheckResult accessCheckResult = CheckNamedTypeAccessibility(containingType, within, basesBeingResolved, ref useSiteInfo);
			if (accessCheckResult != 0)
			{
				return accessCheckResult;
			}
			switch (declaredAccessibility)
			{
			case Accessibility.NotApplicable:
				return AccessCheckResult.Accessible;
			case Accessibility.Public:
				return AccessCheckResult.Accessible;
			case Accessibility.Private:
				if (containingType.TypeKind == TypeKind.Submission)
				{
					return AccessCheckResult.Accessible;
				}
				return ((object)namedTypeSymbol == null) ? AccessCheckResult.Inaccessible : CheckPrivateSymbolAccessibility(namedTypeSymbol, originalDefinition);
			case Accessibility.Internal:
				return (!HasFriendAccessTo(fromAssembly, containingType.ContainingAssembly)) ? AccessCheckResult.Inaccessible : AccessCheckResult.Accessible;
			case Accessibility.ProtectedAndInternal:
				if (!HasFriendAccessTo(fromAssembly, containingType.ContainingAssembly))
				{
					return AccessCheckResult.Inaccessible;
				}
				return CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, ref useSiteInfo);
			case Accessibility.ProtectedOrInternal:
				if (HasFriendAccessTo(fromAssembly, containingType.ContainingAssembly))
				{
					return AccessCheckResult.Accessible;
				}
				return CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, ref useSiteInfo);
			case Accessibility.Protected:
				return CheckProtectedSymbolAccessibility(within, throughTypeOpt, originalDefinition, basesBeingResolved, ref useSiteInfo);
			default:
				throw ExceptionUtilities.UnexpectedValue(declaredAccessibility);
			}
		}

		private static AccessCheckResult CheckProtectedSymbolAccessibility(Symbol within, TypeSymbol throughTypeOpt, NamedTypeSymbol originalContainingType, BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (originalContainingType.TypeKind == TypeKind.Submission)
			{
				return AccessCheckResult.Accessible;
			}
			if (!(within is NamedTypeSymbol namedTypeSymbol))
			{
				return AccessCheckResult.Inaccessible;
			}
			if (IsNestedWithinOriginalContainingType(namedTypeSymbol, originalContainingType))
			{
				return AccessCheckResult.Accessible;
			}
			NamedTypeSymbol originalDefinition = namedTypeSymbol.OriginalDefinition;
			AccessCheckResult result = AccessCheckResult.Inaccessible;
			TypeSymbol typeSymbol = throughTypeOpt?.OriginalDefinition;
			NamedTypeSymbol namedTypeSymbol2 = originalDefinition;
			while ((object)namedTypeSymbol2 != null)
			{
				if (InheritsFromOrImplementsIgnoringConstruction(namedTypeSymbol2, originalContainingType, basesBeingResolved, ref useSiteInfo))
				{
					if ((object)typeSymbol == null || InheritsFromOrImplementsIgnoringConstruction(typeSymbol, namedTypeSymbol2, basesBeingResolved, ref useSiteInfo))
					{
						return AccessCheckResult.Accessible;
					}
					result = AccessCheckResult.InaccessibleViaThroughType;
				}
				namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
			}
			return result;
		}

		private static AccessCheckResult CheckPrivateSymbolAccessibility(Symbol within, NamedTypeSymbol originalContainingType)
		{
			if (!(within is NamedTypeSymbol withinType))
			{
				return AccessCheckResult.Inaccessible;
			}
			return (!IsNestedWithinOriginalContainingType(withinType, originalContainingType)) ? AccessCheckResult.Inaccessible : AccessCheckResult.Accessible;
		}

		private static bool IsNestedWithinOriginalContainingType(NamedTypeSymbol withinType, NamedTypeSymbol originalContainingType)
		{
			NamedTypeSymbol namedTypeSymbol = withinType.OriginalDefinition;
			while ((object)namedTypeSymbol != null)
			{
				if ((object)namedTypeSymbol == originalContainingType)
				{
					return true;
				}
				namedTypeSymbol = namedTypeSymbol.ContainingType;
			}
			return false;
		}

		private static bool InheritsFromOrImplementsIgnoringConstruction(TypeSymbol derivedType, TypeSymbol baseType, BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			PooledHashSet<NamedTypeSymbol> pooledHashSet = null;
			ArrayBuilder<NamedTypeSymbol> arrayBuilder = null;
			bool flag = TypeSymbolExtensions.IsInterfaceType(baseType);
			if (flag)
			{
				pooledHashSet = PooledHashSet<NamedTypeSymbol>.GetInstance();
				arrayBuilder = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			}
			TypeSymbol typeSymbol = derivedType;
			bool flag2 = false;
			while ((object)typeSymbol != null)
			{
				if (flag == TypeSymbolExtensions.IsInterfaceType(typeSymbol) && typeSymbol.Equals(baseType))
				{
					flag2 = true;
					break;
				}
				if (flag)
				{
					AddBaseInterfaces(typeSymbol, arrayBuilder, pooledHashSet, basesBeingResolved);
				}
				ConsList<TypeSymbol> inheritsBeingResolvedOpt = basesBeingResolved.InheritsBeingResolvedOpt;
				if (inheritsBeingResolvedOpt != null && inheritsBeingResolvedOpt.Contains(typeSymbol))
				{
					typeSymbol = null;
					continue;
				}
				typeSymbol = typeSymbol.TypeKind switch
				{
					TypeKind.Interface => null, 
					TypeKind.TypeParameter => ConstraintsHelper.GetClassConstraint((TypeParameterSymbol)typeSymbol, ref useSiteInfo), 
					_ => typeSymbol.GetDirectBaseTypeWithDefinitionUseSiteDiagnostics(basesBeingResolved, ref useSiteInfo), 
				};
				if ((object)typeSymbol != null)
				{
					typeSymbol = typeSymbol.OriginalDefinition;
				}
			}
			if (!flag2 && flag)
			{
				while (arrayBuilder.Count != 0)
				{
					NamedTypeSymbol namedTypeSymbol = arrayBuilder.Pop();
					if (namedTypeSymbol.Equals(baseType))
					{
						flag2 = true;
						break;
					}
					AddBaseInterfaces(namedTypeSymbol, arrayBuilder, pooledHashSet, basesBeingResolved);
				}
				if (!flag2)
				{
					foreach (NamedTypeSymbol item in pooledHashSet)
					{
						TypeSymbolExtensions.AddUseSiteInfo(item, ref useSiteInfo);
					}
				}
			}
			pooledHashSet?.Free();
			arrayBuilder?.Free();
			return flag2;
		}

		private static void AddBaseInterfaces(TypeSymbol derived, ArrayBuilder<NamedTypeSymbol> baseInterfaces, PooledHashSet<NamedTypeSymbol> interfacesLookedAt, BasesBeingResolved basesBeingResolved)
		{
			ConsList<TypeSymbol> inheritsBeingResolvedOpt = basesBeingResolved.InheritsBeingResolvedOpt;
			if (inheritsBeingResolvedOpt != null && inheritsBeingResolvedOpt.Contains(derived))
			{
				return;
			}
			ConsList<TypeSymbol> implementsBeingResolvedOpt = basesBeingResolved.ImplementsBeingResolvedOpt;
			if (implementsBeingResolvedOpt != null && implementsBeingResolvedOpt.Contains(derived))
			{
				return;
			}
			ImmutableArray<TypeSymbol> immutableArray;
			switch (derived.Kind)
			{
			case SymbolKind.TypeParameter:
				immutableArray = ((TypeParameterSymbol)derived).ConstraintTypesNoUseSiteDiagnostics;
				break;
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
				immutableArray = ImmutableArray<TypeSymbol>.CastUp(((NamedTypeSymbol)derived).GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved));
				break;
			default:
				immutableArray = ImmutableArray<TypeSymbol>.CastUp(derived.InterfacesNoUseSiteDiagnostics);
				break;
			}
			ImmutableArray<TypeSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				switch (current.TypeKind)
				{
				case TypeKind.Interface:
				{
					NamedTypeSymbol item = (NamedTypeSymbol)current.OriginalDefinition;
					if (interfacesLookedAt.Add(item))
					{
						baseInterfaces.Add(item);
					}
					break;
				}
				case TypeKind.TypeParameter:
					AddBaseInterfaces(current, baseInterfaces, interfacesLookedAt, basesBeingResolved);
					break;
				}
			}
		}

		public static bool HasFriendAccessTo(AssemblySymbol fromAssembly, AssemblySymbol toAssembly)
		{
			if (!IsSameAssembly(fromAssembly, toAssembly))
			{
				return InternalsAccessibleTo(toAssembly, fromAssembly);
			}
			return true;
		}

		private static bool InternalsAccessibleTo(AssemblySymbol toAssembly, AssemblySymbol assemblyWantingAccess)
		{
			if (assemblyWantingAccess.AreInternalsVisibleToThisAssembly(toAssembly))
			{
				return true;
			}
			if (assemblyWantingAccess.IsInteractive && toAssembly.IsInteractive)
			{
				return true;
			}
			return false;
		}

		private static bool IsSameAssembly(AssemblySymbol fromAssembly, AssemblySymbol toAssembly)
		{
			return object.Equals(fromAssembly, toAssembly);
		}

		public static string GetAccessibilityForErrorMessage(Symbol sym, AssemblySymbol fromAssembly)
		{
			return ErrorMessageHelpers.ToDisplay(sym.DeclaredAccessibility);
		}

		private static bool VerifyAccessExposure(Symbol exposedThrough, TypeSymbol exposedType, ref ArrayBuilder<AccessExposure> illegalExposure, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool result = true;
			while (true)
			{
				switch (exposedType.Kind)
				{
				case SymbolKind.ErrorType:
				case SymbolKind.TypeParameter:
					return true;
				case SymbolKind.ArrayType:
					goto IL_002e;
				case SymbolKind.NamedType:
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)exposedType;
					if (namedTypeSymbol.IsTupleType)
					{
						namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType;
					}
					NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol;
					do
					{
						if (namedTypeSymbol2.Arity > 0)
						{
							ImmutableArray<TypeSymbol>.Enumerator enumerator = namedTypeSymbol2.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
							while (enumerator.MoveNext())
							{
								TypeSymbol current = enumerator.Current;
								if (!VerifyAccessExposure(exposedThrough, current, ref illegalExposure, ref useSiteInfo))
								{
									result = false;
								}
							}
						}
						namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
					}
					while ((object)namedTypeSymbol2 != null);
					NamespaceOrTypeSymbol containerWithAccessError = null;
					if (VerifyAccessExposure(exposedThrough, namedTypeSymbol.OriginalDefinition, ref containerWithAccessError, ref useSiteInfo))
					{
						return result;
					}
					if (illegalExposure == null)
					{
						illegalExposure = ArrayBuilder<AccessExposure>.GetInstance();
					}
					illegalExposure.Add(new AccessExposure
					{
						ExposedType = namedTypeSymbol,
						ExposedTo = containerWithAccessError
					});
					return false;
				}
				}
				continue;
				IL_002e:
				exposedType = ((ArrayTypeSymbol)exposedType).ElementType;
			}
		}

		private static bool VerifyAccessExposure(Symbol exposedThrough, NamedTypeSymbol exposedType, ref NamespaceOrTypeSymbol containerWithAccessError, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			containerWithAccessError = null;
			if (exposedType.DeclaredAccessibility == Accessibility.Public)
			{
				Symbol containingSymbol = exposedType.ContainingSymbol;
				if ((object)containingSymbol != null && containingSymbol.Kind == SymbolKind.Namespace)
				{
					return true;
				}
			}
			if (MemberIsOrNestedInType(exposedThrough, exposedType))
			{
				return true;
			}
			if (!VerifyAccessExposureWithinAssembly(exposedThrough, exposedType, ref containerWithAccessError, ref useSiteInfo))
			{
				return false;
			}
			return VerifyAccessExposureOutsideAssembly(exposedThrough, exposedType, ref useSiteInfo);
		}

		private static bool MemberIsOrNestedInType(Symbol member, NamedTypeSymbol type)
		{
			type = type.OriginalDefinition;
			if (member.Equals(type))
			{
				return true;
			}
			NamedTypeSymbol containingType = member.ContainingType;
			while ((object)containingType != null)
			{
				if (containingType.Equals(type))
				{
					return true;
				}
				containingType = containingType.ContainingType;
			}
			return false;
		}

		private static bool VerifyAccessExposureWithinAssembly(Symbol exposedThrough, NamedTypeSymbol exposedType, ref NamespaceOrTypeSymbol containerWithAccessError, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool seenThroughInheritance = false;
			return VerifyAccessExposureHelper(exposedThrough, exposedType, ref containerWithAccessError, ref seenThroughInheritance, isOutsideAssembly: false, ref useSiteInfo);
		}

		private static bool VerifyAccessExposureOutsideAssembly(Symbol exposedThrough, NamedTypeSymbol exposedType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			Accessibility effectiveAccessOutsideAssembly = GetEffectiveAccessOutsideAssembly(exposedThrough);
			if (effectiveAccessOutsideAssembly == Accessibility.Private)
			{
				return true;
			}
			switch (GetEffectiveAccessOutsideAssembly(exposedType))
			{
			case Accessibility.Private:
				return false;
			case Accessibility.Public:
				return true;
			default:
			{
				if (effectiveAccessOutsideAssembly == Accessibility.Public)
				{
					return false;
				}
				bool seenThroughInheritance = false;
				NamespaceOrTypeSymbol containerWithAccessError = null;
				VerifyAccessExposureHelper(exposedThrough, exposedType, ref containerWithAccessError, ref seenThroughInheritance, isOutsideAssembly: true, ref useSiteInfo);
				return seenThroughInheritance;
			}
			}
		}

		static AccessCheck()
		{
			s_mapAccessToAccessOutsideAssembly = new Accessibility[7];
			s_mapAccessToAccessOutsideAssembly[0] = Accessibility.NotApplicable;
			s_mapAccessToAccessOutsideAssembly[1] = Accessibility.Private;
			s_mapAccessToAccessOutsideAssembly[2] = Accessibility.Private;
			s_mapAccessToAccessOutsideAssembly[3] = Accessibility.Protected;
			s_mapAccessToAccessOutsideAssembly[4] = Accessibility.Private;
			s_mapAccessToAccessOutsideAssembly[5] = Accessibility.Protected;
			s_mapAccessToAccessOutsideAssembly[6] = Accessibility.Public;
		}

		private static Accessibility GetEffectiveAccessOutsideAssembly(Symbol symbol)
		{
			Accessibility accessibility = s_mapAccessToAccessOutsideAssembly[(int)symbol.DeclaredAccessibility];
			if (accessibility == Accessibility.Private)
			{
				return accessibility;
			}
			NamedTypeSymbol containingType = symbol.ContainingType;
			while ((object)containingType != null)
			{
				Accessibility accessibility2 = s_mapAccessToAccessOutsideAssembly[(int)containingType.DeclaredAccessibility];
				if (accessibility2 < accessibility)
				{
					accessibility = accessibility2;
				}
				if (accessibility == Accessibility.Private)
				{
					return accessibility;
				}
				containingType = containingType.ContainingType;
			}
			return accessibility;
		}

		private static Accessibility GetAccessInAssemblyContext(Symbol symbol, bool isOutsideAssembly)
		{
			Accessibility accessibility = symbol.DeclaredAccessibility;
			if (isOutsideAssembly)
			{
				accessibility = s_mapAccessToAccessOutsideAssembly[(int)accessibility];
			}
			return accessibility;
		}

		private static bool IsTypeNestedIn(NamedTypeSymbol probablyNestedType, NamedTypeSymbol probablyEnclosingType)
		{
			probablyNestedType = probablyNestedType.OriginalDefinition;
			NamedTypeSymbol containingType = probablyNestedType.ContainingType;
			while ((object)containingType != null)
			{
				if (containingType.Equals(probablyEnclosingType))
				{
					return true;
				}
				containingType = containingType.ContainingType;
			}
			return false;
		}

		private static bool VerifyAccessExposureHelper(Symbol exposingMember, NamedTypeSymbol exposedType, ref NamespaceOrTypeSymbol containerWithAccessError, ref bool seenThroughInheritance, bool isOutsideAssembly, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			seenThroughInheritance = false;
			NamedTypeSymbol namedTypeSymbol = null;
			Accessibility accessInAssemblyContext = GetAccessInAssemblyContext(exposingMember, isOutsideAssembly);
			if (accessInAssemblyContext == Accessibility.Private)
			{
				if (exposingMember.Kind != SymbolKind.NamedType || !IsTypeNestedIn(exposedType, (NamedTypeSymbol)exposingMember))
				{
					return true;
				}
				namedTypeSymbol = (NamedTypeSymbol)exposingMember;
			}
			else
			{
				Accessibility stopAtAccess = Accessibility.Protected;
				namedTypeSymbol = FindEnclosingTypeWithGivenAccess(exposingMember, stopAtAccess, isOutsideAssembly);
			}
			Accessibility accessInAssemblyContext2 = GetAccessInAssemblyContext(namedTypeSymbol, isOutsideAssembly);
			if (accessInAssemblyContext <= Accessibility.Protected && CanBeAccessedThroughInheritance(exposedType, exposingMember.ContainingType, isOutsideAssembly, ref useSiteInfo))
			{
				seenThroughInheritance = true;
				return true;
			}
			NamespaceOrTypeSymbol containingNamespaceOrType = namedTypeSymbol.ContainingNamespaceOrType;
			if (CheckNamedTypeAccessibility(exposedType, containingNamespaceOrType.IsNamespace ? ((Symbol)containingNamespaceOrType.ContainingAssembly) : ((Symbol)containingNamespaceOrType), default(BasesBeingResolved), ref useSiteInfo) != 0)
			{
				containerWithAccessError = containingNamespaceOrType;
				return false;
			}
			if (accessInAssemblyContext2 != Accessibility.Protected)
			{
				return true;
			}
			return VerifyAccessExposureHelper(namedTypeSymbol, exposedType, ref containerWithAccessError, ref seenThroughInheritance, isOutsideAssembly, ref useSiteInfo);
		}

		private static bool CanBeAccessedThroughInheritance(NamedTypeSymbol type, NamedTypeSymbol container, bool isOutsideAssembly, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (GetAccessInAssemblyContext(type, isOutsideAssembly) == Accessibility.Private)
			{
				return false;
			}
			NamedTypeSymbol containingType = type.ContainingType;
			if ((object)containingType == null)
			{
				return false;
			}
			NamedTypeSymbol originalDefinition = containingType.OriginalDefinition;
			if (container.OriginalDefinition.Equals(originalDefinition))
			{
				return true;
			}
			if (TypeSymbolExtensions.IsInterfaceType(containingType))
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = container.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OriginalDefinition.Equals(originalDefinition))
					{
						return true;
					}
				}
			}
			else
			{
				NamedTypeSymbol namedTypeSymbol = container.BaseTypeOriginalDefinition(ref useSiteInfo);
				while ((object)namedTypeSymbol != null)
				{
					if (namedTypeSymbol.Equals(originalDefinition))
					{
						return true;
					}
					namedTypeSymbol = namedTypeSymbol.BaseTypeOriginalDefinition(ref useSiteInfo);
				}
			}
			if (GetAccessInAssemblyContext(type, isOutsideAssembly) != Accessibility.Protected)
			{
				return CanBeAccessedThroughInheritance(containingType, container, isOutsideAssembly, ref useSiteInfo);
			}
			return false;
		}

		private static NamedTypeSymbol FindEnclosingTypeWithGivenAccess(Symbol member, Accessibility stopAtAccess, bool isOutsideAssembly)
		{
			NamedTypeSymbol namedTypeSymbol = member.ContainingType;
			if (member.Kind == SymbolKind.NamedType && (object)namedTypeSymbol == null)
			{
				namedTypeSymbol = (NamedTypeSymbol)member;
			}
			while (true)
			{
				NamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
				if ((object)containingType == null || GetAccessInAssemblyContext(namedTypeSymbol, isOutsideAssembly) <= stopAtAccess)
				{
					break;
				}
				namedTypeSymbol = containingType;
			}
			return namedTypeSymbol;
		}

		public static bool VerifyAccessExposureOfBaseClassOrInterface(NamedTypeSymbol classOrInterface, TypeSyntax baseClassSyntax, TypeSymbol @base, BindingDiagnosticBag diagBag)
		{
			ArrayBuilder<AccessExposure> illegalExposure = null;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagBag, classOrInterface.ContainingAssembly);
			VerifyAccessExposure(classOrInterface, @base, ref illegalExposure, ref useSiteInfo);
			((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)baseClassSyntax, useSiteInfo);
			if (illegalExposure != null)
			{
				ArrayBuilder<AccessExposure>.Enumerator enumerator = illegalExposure.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AccessExposure current = enumerator.Current;
					NamespaceOrTypeSymbol exposedTo = current.ExposedTo;
					TypeSymbol typeSymbol = TypeSymbolExtensions.DigThroughArrayType(current.ExposedType);
					if ((object)exposedTo != null)
					{
						if (typeSymbol.Equals(@base))
						{
							Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritanceAccessMismatch5, classOrInterface.Name, SymbolExtensions.GetKindText(@base), SymbolExtensions.ToErrorMessageArgument(@base), SymbolExtensions.GetKindText(exposedTo), SymbolExtensions.ToErrorMessageArgument(exposedTo));
						}
						else
						{
							Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritsTypeArgAccessMismatch7, classOrInterface.Name, SymbolExtensions.GetKindText(@base), SymbolExtensions.ToErrorMessageArgument(@base), typeSymbol, SymbolExtensions.GetKindText(exposedTo), SymbolExtensions.ToErrorMessageArgument(exposedTo));
						}
					}
					else if (typeSymbol.Equals(@base))
					{
						Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritanceAccessMismatchOutside3, classOrInterface.Name, SymbolExtensions.GetKindText(@base), SymbolExtensions.ToErrorMessageArgument(@base));
					}
					else
					{
						Binder.ReportDiagnostic(diagBag, baseClassSyntax, ERRID.ERR_InheritsTypeArgAccessMismatchOutside5, classOrInterface.Name, SymbolExtensions.GetKindText(@base), SymbolExtensions.ToErrorMessageArgument(@base), typeSymbol);
					}
				}
				illegalExposure.Free();
				return false;
			}
			return true;
		}

		public static void VerifyAccessExposureForParameterType(Symbol member, string paramName, VisualBasicSyntaxNode errorLocation, TypeSymbol TypeBehindParam, BindingDiagnosticBag diagBag)
		{
			ArrayBuilder<AccessExposure> illegalExposure = null;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagBag, member.ContainingAssembly);
			VerifyAccessExposure(member, TypeBehindParam, ref illegalExposure, ref useSiteInfo);
			((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)errorLocation, useSiteInfo);
			if (illegalExposure == null)
			{
				return;
			}
			ArrayBuilder<AccessExposure>.Enumerator enumerator = illegalExposure.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AccessExposure current = enumerator.Current;
				NamespaceOrTypeSymbol exposedTo = current.ExposedTo;
				TypeSymbol typeSymbol = TypeSymbolExtensions.DigThroughArrayType(current.ExposedType);
				NamedTypeSymbol containingType = member.ContainingType;
				if ((object)exposedTo != null)
				{
					Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatch6, paramName, typeSymbol, SymbolExtensions.GetKindText(exposedTo), SymbolExtensions.ToErrorMessageArgument(exposedTo), SymbolExtensions.GetKindText(containingType), containingType.Name);
				}
				else
				{
					Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchOutsideAssembly4, paramName, typeSymbol, SymbolExtensions.GetKindText(containingType), containingType.Name);
				}
			}
			illegalExposure.Free();
		}

		public static void VerifyAccessExposureForMemberType(Symbol member, SyntaxNodeOrToken errorLocation, TypeSymbol typeBehindMember, BindingDiagnosticBag diagBag, bool isDelegateFromImplements = false)
		{
			ArrayBuilder<AccessExposure> illegalExposure = null;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagBag, member.ContainingAssembly);
			VerifyAccessExposure(member, typeBehindMember, ref illegalExposure, ref useSiteInfo);
			diagBag.Add(errorLocation, useSiteInfo);
			if (illegalExposure == null)
			{
				return;
			}
			NamedTypeSymbol namedTypeSymbol = ((member.Kind != SymbolKind.NamedType) ? member.ContainingType : ((NamedTypeSymbol)member));
			string text = (TypeSymbolExtensions.IsDelegateType(namedTypeSymbol) ? namedTypeSymbol.Name : member.Name);
			ArrayBuilder<AccessExposure>.Enumerator enumerator = illegalExposure.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AccessExposure current = enumerator.Current;
				NamespaceOrTypeSymbol exposedTo = current.ExposedTo;
				TypeSymbol typeSymbol = TypeSymbolExtensions.DigThroughArrayType(current.ExposedType);
				if ((object)exposedTo != null)
				{
					if (isDelegateFromImplements)
					{
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchImplementedEvent6, text, typeSymbol, SymbolExtensions.GetKindText(exposedTo), SymbolExtensions.ToErrorMessageArgument(exposedTo), SymbolExtensions.GetKindText(namedTypeSymbol), namedTypeSymbol.Name);
					}
					else
					{
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatch6, text, typeSymbol, SymbolExtensions.GetKindText(exposedTo), SymbolExtensions.ToErrorMessageArgument(exposedTo), SymbolExtensions.GetKindText(namedTypeSymbol), namedTypeSymbol.Name);
					}
				}
				else if (isDelegateFromImplements)
				{
					Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchImplementedEvent4, text, typeSymbol, SymbolExtensions.GetKindText(namedTypeSymbol), namedTypeSymbol.Name);
				}
				else
				{
					Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_AccessMismatchOutsideAssembly4, text, typeSymbol, SymbolExtensions.GetKindText(namedTypeSymbol), namedTypeSymbol.Name);
				}
			}
			illegalExposure.Free();
		}
	}
}
