using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class OverrideHidingHelper
	{
		public static void CheckHidingAndOverridingForType(SourceMemberContainerTypeSymbol container, BindingDiagnosticBag diagnostics)
		{
			TypeKind typeKind = container.TypeKind;
			if (typeKind == TypeKind.Class || typeKind == TypeKind.Interface || typeKind == TypeKind.Struct)
			{
				CheckMembersAgainstBaseType(container, diagnostics);
				CheckAllAbstractsAreOverriddenAndNotHidden(container, diagnostics);
			}
		}

		public static bool SignaturesMatch(Symbol sym1, Symbol sym2, out bool exactMatch, out bool exactMatchIgnoringCustomModifiers)
		{
			SymbolComparisonResults symbolComparisonResults = DetailedSignatureCompare(sym1, sym2, (SymbolComparisonResults)115557);
			if (((uint)symbolComparisonResults & 0xFFFEFDDFu) != 0)
			{
				exactMatch = false;
				exactMatchIgnoringCustomModifiers = false;
				return false;
			}
			exactMatch = (symbolComparisonResults & (SymbolComparisonResults)66080) == 0;
			exactMatchIgnoringCustomModifiers = (symbolComparisonResults & (SymbolComparisonResults)66048) == 0;
			return true;
		}

		internal static SymbolComparisonResults DetailedSignatureCompare(Symbol sym1, Symbol sym2, SymbolComparisonResults comparisons, SymbolComparisonResults stopIfAny = (SymbolComparisonResults)0)
		{
			if (sym1.Kind == SymbolKind.Property)
			{
				return PropertySignatureComparer.DetailedCompare((PropertySymbol)sym1, (PropertySymbol)sym2, comparisons, stopIfAny);
			}
			return MethodSignatureComparer.DetailedCompare((MethodSymbol)sym1, (MethodSymbol)sym2, comparisons, stopIfAny);
		}

		private static void CheckMembersAgainstBaseType(SourceMemberContainerTypeSymbol container, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (!CanOverrideOrHide(current))
				{
					continue;
				}
				switch (current.Kind)
				{
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol = (MethodSymbol)current;
					if (!SymbolExtensions.IsAccessor(methodSymbol))
					{
						if (methodSymbol.IsOverrides)
						{
							OverrideHidingHelper<MethodSymbol>.CheckOverrideMember(methodSymbol, methodSymbol.OverriddenMembers, diagnostics);
						}
						else if (methodSymbol.IsNotOverridable)
						{
							diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NotOverridableRequiresOverrides), methodSymbol.Locations[0]));
						}
					}
					break;
				}
				case SymbolKind.Property:
				{
					PropertySymbol propertySymbol = (PropertySymbol)current;
					if (propertySymbol.IsOverrides)
					{
						OverrideHidingHelper<PropertySymbol>.CheckOverrideMember(propertySymbol, propertySymbol.OverriddenMembers, diagnostics);
					}
					else if (propertySymbol.IsNotOverridable)
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_NotOverridableRequiresOverrides), propertySymbol.Locations[0]));
					}
					break;
				}
				}
				CheckShadowing(container, current, diagnostics);
			}
		}

		private static void CheckAllAbstractsAreOverriddenAndNotHidden(NamedTypeSymbol container, BindingDiagnosticBag diagnostics)
		{
			if (!container.IsMustInherit && !container.IsNotInheritable)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsMustOverride)
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_MustOverridesInClass1, container.Name), container.Locations[0]));
						break;
					}
				}
			}
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = container.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics == null || !baseTypeNoUseSiteDiagnostics.IsMustInherit)
			{
				return;
			}
			HashSet<Symbol> hashSet = new HashSet<Symbol>();
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			NamedTypeSymbol namedTypeSymbol = container;
			while ((object)namedTypeSymbol != null)
			{
				ImmutableArray<Symbol>.Enumerator enumerator2 = namedTypeSymbol.GetMembers().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current = enumerator2.Current;
					if (CanOverrideOrHide(current) && !SymbolExtensions.IsAccessor(current))
					{
						if (current.IsOverrides && (object)GetOverriddenMember(current) != null)
						{
							hashSet.Add(GetOverriddenMember(current));
						}
						if (current.IsMustOverride && (object)namedTypeSymbol != container && !hashSet.Contains(current))
						{
							instance.Add(current);
						}
					}
				}
				namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			if (instance.Any())
			{
				if (container.IsMustInherit)
				{
					HashSet<Symbol> hashSet2 = new HashSet<Symbol>();
					ArrayBuilder<Symbol>.Enumerator enumerator3 = instance.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						Symbol current2 = enumerator3.Current;
						ImmutableArray<Symbol>.Enumerator enumerator4 = container.GetMembers(current2.Name).GetEnumerator();
						while (enumerator4.MoveNext())
						{
							Symbol current3 = enumerator4.Current;
							if (DoesHide(current3, current2) && !hashSet2.Contains(current3))
							{
								ReportShadowingMustOverrideError(current3, current2, diagnostics);
								hashSet2.Add(current3);
							}
						}
					}
				}
				else
				{
					ArrayBuilder<DiagnosticInfo> instance2 = ArrayBuilder<DiagnosticInfo>.GetInstance(instance.Count);
					ArrayBuilder<Symbol>.Enumerator enumerator5 = instance.GetEnumerator();
					while (enumerator5.MoveNext())
					{
						Symbol current4 = enumerator5.Current;
						if (!SymbolExtensions.IsAccessor(current4))
						{
							if (current4.Kind == SymbolKind.Event)
							{
								diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_MustInheritEventNotOverridden, current4, CustomSymbolDisplayFormatter.QualifiedName(current4.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(container)), container.Locations[0]));
							}
							else
							{
								instance2.Add(ErrorFactory.ErrorInfo(ERRID.ERR_UnimplementedMustOverride, current4.ContainingType, current4));
							}
						}
					}
					if (instance2.Count > 0)
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_BaseOnlyClassesMustBeExplicit2, CustomSymbolDisplayFormatter.ShortErrorName(container), new CompoundDiagnosticInfo(instance2.ToArrayAndFree())), container.Locations[0]));
					}
					else
					{
						instance2.Free();
					}
				}
			}
			instance.Free();
		}

		private static bool DoesHide(Symbol hidingMember, Symbol hiddenMember)
		{
			switch (hidingMember.Kind)
			{
			case SymbolKind.Method:
				if (SymbolExtensions.IsOverloads(hidingMember) && hiddenMember.Kind == SymbolKind.Method)
				{
					MethodSymbol methodSymbol = (MethodSymbol)hidingMember;
					if (methodSymbol.IsOverrides)
					{
						return false;
					}
					bool exactMatchIgnoringCustomModifiers2 = false;
					MethodSymbol sym2 = (MethodSymbol)hiddenMember;
					bool exactMatch = false;
					return SignaturesMatch(methodSymbol, sym2, out exactMatch, out exactMatchIgnoringCustomModifiers2) && exactMatchIgnoringCustomModifiers2;
				}
				return true;
			case SymbolKind.Property:
				if (SymbolExtensions.IsOverloads(hidingMember) && hiddenMember.Kind == SymbolKind.Property)
				{
					PropertySymbol propertySymbol = (PropertySymbol)hidingMember;
					if (propertySymbol.IsOverrides)
					{
						return false;
					}
					bool exactMatchIgnoringCustomModifiers = false;
					PropertySymbol sym = (PropertySymbol)hiddenMember;
					bool exactMatch = false;
					return SignaturesMatch(propertySymbol, sym, out exactMatch, out exactMatchIgnoringCustomModifiers) && exactMatchIgnoringCustomModifiers;
				}
				return true;
			default:
				return true;
			}
		}

		protected static void CheckShadowing(SourceMemberContainerTypeSymbol container, Symbol member, BindingDiagnosticBag diagnostics)
		{
			bool memberIsOverloads = SymbolExtensions.IsOverloads(member);
			bool warnForHiddenMember = !member.ShadowsExplicitly;
			if (!warnForHiddenMember)
			{
				return;
			}
			if (TypeSymbolExtensions.IsInterfaceType(container))
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = container.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					CheckShadowingInBaseType(container, member, memberIsOverloads, current, diagnostics, ref warnForHiddenMember);
				}
			}
			else
			{
				NamedTypeSymbol baseTypeNoUseSiteDiagnostics = container.BaseTypeNoUseSiteDiagnostics;
				while ((object)baseTypeNoUseSiteDiagnostics != null)
				{
					CheckShadowingInBaseType(container, member, memberIsOverloads, baseTypeNoUseSiteDiagnostics, diagnostics, ref warnForHiddenMember);
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
				}
			}
		}

		private static void CheckShadowingInBaseType(SourceMemberContainerTypeSymbol container, Symbol member, bool memberIsOverloads, NamedTypeSymbol baseType, BindingDiagnosticBag diagnostics, ref bool warnForHiddenMember)
		{
			if (!warnForHiddenMember)
			{
				return;
			}
			ImmutableArray<Symbol>.Enumerator enumerator = baseType.GetMembers(member.Name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				if (AccessCheck.IsSymbolAccessible(current, container, null, ref useSiteInfo) && (!memberIsOverloads || current.Kind != member.Kind || SymbolExtensions.IsWithEventsProperty(current) || (member.Kind == SymbolKind.Method && MethodSymbolExtensions.IsUserDefinedOperator((MethodSymbol)member) != MethodSymbolExtensions.IsUserDefinedOperator((MethodSymbol)current)) || SymbolExtensions.IsAccessor(member) != SymbolExtensions.IsAccessor(current)) && (!SymbolExtensions.IsAccessor(member) || !SymbolExtensions.IsAccessor(current)) && (member.Kind != SymbolKind.NamedType || current.Kind != SymbolKind.NamedType || SymbolExtensions.GetArity(member) == SymbolExtensions.GetArity(current)))
				{
					ReportShadowingDiagnostic(member, current, diagnostics);
					warnForHiddenMember = false;
					break;
				}
			}
		}

		private static void ReportShadowingDiagnostic(Symbol hidingMember, Symbol hiddenMember, BindingDiagnosticBag diagnostics)
		{
			Symbol symbol = hiddenMember.get_ImplicitlyDefinedBy((Dictionary<string, ArrayBuilder<Symbol>>)null);
			if ((object)symbol == null && SymbolExtensions.IsUserDefinedOperator(hiddenMember) && !SymbolExtensions.IsUserDefinedOperator(hidingMember))
			{
				symbol = hiddenMember;
			}
			Symbol symbol2 = hidingMember.get_ImplicitlyDefinedBy((Dictionary<string, ArrayBuilder<Symbol>>)null);
			if ((object)symbol2 == null && SymbolExtensions.IsUserDefinedOperator(hidingMember) && !SymbolExtensions.IsUserDefinedOperator(hiddenMember))
			{
				symbol2 = hidingMember;
			}
			if ((object)symbol != null)
			{
				if ((object)symbol2 != null)
				{
					if (!CaseInsensitiveComparison.Equals(symbol.Name, symbol2.Name))
					{
						diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_SynthMemberShadowsSynthMember7, SymbolExtensions.GetKindText(symbol2), AssociatedSymbolName(symbol2), hidingMember.Name, SymbolExtensions.GetKindText(symbol), AssociatedSymbolName(symbol), SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), hidingMember.Locations[0]));
					}
				}
				else
				{
					diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_MemberShadowsSynthMember6, SymbolExtensions.GetKindText(hidingMember), hidingMember.Name, SymbolExtensions.GetKindText(symbol), AssociatedSymbolName(symbol), SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), hidingMember.Locations[0]));
				}
			}
			else if ((object)symbol2 != null)
			{
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_SynthMemberShadowsMember5, SymbolExtensions.GetKindText(symbol2), AssociatedSymbolName(symbol2), hidingMember.Name, SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), symbol2.Locations[0]));
			}
			else if (hidingMember.Kind == hiddenMember.Kind && (hidingMember.Kind == SymbolKind.Property || hidingMember.Kind == SymbolKind.Method) && !SymbolExtensions.IsWithEventsProperty(hiddenMember) && !SymbolExtensions.IsWithEventsProperty(hidingMember))
			{
				ERRID id = ((!hiddenMember.IsOverridable && !hiddenMember.IsOverrides && (!hiddenMember.IsMustOverride || hiddenMember.ContainingType.IsInterface)) ? ERRID.WRN_MustOverloadBase4 : ERRID.WRN_MustOverride2);
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(id, SymbolExtensions.GetKindText(hidingMember), hidingMember.Name, SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), hidingMember.Locations[0]));
			}
			else
			{
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_OverrideType5, SymbolExtensions.GetKindText(hidingMember), hidingMember.Name, SymbolExtensions.GetKindText(hiddenMember), SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), hidingMember.Locations[0]));
			}
		}

		public static string AssociatedSymbolName(Symbol associatedSymbol)
		{
			if (!SymbolExtensions.IsUserDefinedOperator(associatedSymbol))
			{
				return associatedSymbol.Name;
			}
			return SyntaxFacts.GetText(OverloadResolution.GetOperatorTokenKind(associatedSymbol.Name));
		}

		private static void ReportShadowingMustOverrideError(Symbol hidingMember, Symbol hiddenMember, BindingDiagnosticBag diagnostics)
		{
			if (SymbolExtensions.IsAccessor(hidingMember))
			{
				Symbol associatedSymbol = ((MethodSymbol)hidingMember).AssociatedSymbol;
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_SynthMemberShadowsMustOverride5, hidingMember, SymbolExtensions.GetKindText(associatedSymbol), associatedSymbol.Name, SymbolExtensions.GetKindText(hiddenMember.ContainingType), CustomSymbolDisplayFormatter.ShortErrorName(hiddenMember.ContainingType)), hidingMember.Locations[0]));
			}
			else
			{
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_CantShadowAMustOverride1, hidingMember), hidingMember.Locations[0]));
			}
		}

		internal static bool CanOverrideOrHide(Symbol sym)
		{
			if (sym.Kind != SymbolKind.Method)
			{
				return true;
			}
			switch (((MethodSymbol)sym).MethodKind)
			{
			case MethodKind.AnonymousFunction:
			case MethodKind.Constructor:
			case MethodKind.StaticConstructor:
				return false;
			case MethodKind.Conversion:
			case MethodKind.DelegateInvoke:
			case MethodKind.EventAdd:
			case MethodKind.EventRaise:
			case MethodKind.EventRemove:
			case MethodKind.UserDefinedOperator:
			case MethodKind.Ordinary:
			case MethodKind.PropertyGet:
			case MethodKind.PropertySet:
			case MethodKind.DeclareMethod:
				return true;
			default:
				return false;
			}
		}

		protected static Symbol GetOverriddenMember(Symbol sym)
		{
			return sym.Kind switch
			{
				SymbolKind.Method => ((MethodSymbol)sym).OverriddenMethod, 
				SymbolKind.Property => ((PropertySymbol)sym).OverriddenProperty, 
				SymbolKind.Event => ((EventSymbol)sym).OverriddenEvent, 
				_ => null, 
			};
		}

		public static bool RequiresExplicitOverride(MethodSymbol method)
		{
			if (SymbolExtensions.IsAccessor(method))
			{
				if (method.AssociatedSymbol is EventSymbol)
				{
					return false;
				}
				return RequiresExplicitOverride((PropertySymbol)method.AssociatedSymbol);
			}
			if ((object)method.OverriddenMethod != null)
			{
				ImmutableArray<MethodSymbol>.Enumerator enumerator = method.OverriddenMembers.InaccessibleMembers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol current = enumerator.Current;
					if (current.IsOverridable || current.IsMustOverride || current.IsOverrides)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool RequiresExplicitOverride(PropertySymbol prop)
		{
			if ((object)prop.OverriddenProperty != null)
			{
				ImmutableArray<PropertySymbol>.Enumerator enumerator = prop.OverriddenMembers.InaccessibleMembers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					PropertySymbol current = enumerator.Current;
					if (current.IsOverridable || current.IsMustOverride || current.IsOverrides)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool RequiresExplicitOverride(EventSymbol @event)
		{
			if ((object)@event.OverriddenEvent != null)
			{
				ImmutableArray<EventSymbol>.Enumerator enumerator = @event.OverriddenOrHiddenMembers.InaccessibleMembers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					EventSymbol current = enumerator.Current;
					if (current.IsOverridable || current.IsMustOverride || current.IsOverrides)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
	internal class OverrideHidingHelper<TSymbol> : OverrideHidingHelper where TSymbol : Symbol
	{
		private static IEqualityComparer<TSymbol> s_runtimeSignatureComparer;

		static OverrideHidingHelper()
		{
			OverrideHidingHelper<MethodSymbol>.s_runtimeSignatureComparer = MethodSignatureComparer.RuntimeMethodSignatureComparer;
			OverrideHidingHelper<PropertySymbol>.s_runtimeSignatureComparer = PropertySignatureComparer.RuntimePropertySignatureComparer;
			OverrideHidingHelper<EventSymbol>.s_runtimeSignatureComparer = EventSignatureComparer.RuntimeEventSignatureComparer;
		}

		internal static OverriddenMembersResult<TSymbol> MakeOverriddenMembers(TSymbol overridingSym)
		{
			if (!overridingSym.IsOverrides || !OverrideHidingHelper.CanOverrideOrHide(overridingSym))
			{
				return OverriddenMembersResult<TSymbol>.Empty;
			}
			bool dangerous_IsFromSomeCompilationIncludingRetargeting = overridingSym.Dangerous_IsFromSomeCompilationIncludingRetargeting;
			NamedTypeSymbol containingType = overridingSym.ContainingType;
			ArrayBuilder<TSymbol> instance = ArrayBuilder<TSymbol>.GetInstance();
			ArrayBuilder<TSymbol> instance2 = ArrayBuilder<TSymbol>.GetInstance();
			ArrayBuilder<TSymbol> instance3 = ArrayBuilder<TSymbol>.GetInstance();
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
			while ((object)baseTypeNoUseSiteDiagnostics != null && !FindOverriddenMembersInType(overridingSym, dangerous_IsFromSomeCompilationIncludingRetargeting, containingType, baseTypeNoUseSiteDiagnostics, instance, instance2, instance3))
			{
				baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
			}
			return OverriddenMembersResult<TSymbol>.Create(instance.ToImmutableAndFree(), instance2.ToImmutableAndFree(), instance3.ToImmutableAndFree());
		}

		private static bool FindOverriddenMembersInType(TSymbol overridingSym, bool overridingIsFromSomeCompilation, NamedTypeSymbol overridingContainingType, NamedTypeSymbol currType, ArrayBuilder<TSymbol> overriddenBuilder, ArrayBuilder<TSymbol> inexactOverriddenMembers, ArrayBuilder<TSymbol> inaccessibleBuilder)
		{
			bool stopLookup = false;
			bool haveExactMatch = false;
			ArrayBuilder<TSymbol> instance = ArrayBuilder<TSymbol>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = currType.GetMembers(overridingSym.Name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				ProcessMemberWithMatchingName(enumerator.Current, overridingSym, overridingIsFromSomeCompilation, overridingContainingType, inexactOverriddenMembers, inaccessibleBuilder, instance, ref stopLookup, ref haveExactMatch);
			}
			if (overridingSym.Kind == SymbolKind.Property)
			{
				PropertySymbol propertySymbol = (PropertySymbol)(object)overridingSym;
				if (propertySymbol.IsImplicitlyDeclared && propertySymbol.IsWithEvents)
				{
					foreach (PropertySymbol synthesizedWithEventsOverride in currType.GetSynthesizedWithEventsOverrides())
					{
						if (synthesizedWithEventsOverride.Name.Equals(propertySymbol.Name))
						{
							ProcessMemberWithMatchingName(synthesizedWithEventsOverride, overridingSym, overridingIsFromSomeCompilation, overridingContainingType, inexactOverriddenMembers, inaccessibleBuilder, instance, ref stopLookup, ref haveExactMatch);
						}
					}
				}
			}
			if (instance.Count > 1)
			{
				RemoveMembersWithConflictingAccessibility(instance);
			}
			if (instance.Count > 0)
			{
				if (haveExactMatch)
				{
					overriddenBuilder.Clear();
				}
				if (overriddenBuilder.Count == 0)
				{
					overriddenBuilder.AddRange(instance);
				}
			}
			instance.Free();
			return stopLookup;
		}

		private static void ProcessMemberWithMatchingName(Symbol sym, TSymbol overridingSym, bool overridingIsFromSomeCompilation, NamedTypeSymbol overridingContainingType, ArrayBuilder<TSymbol> inexactOverriddenMembers, ArrayBuilder<TSymbol> inaccessibleBuilder, ArrayBuilder<TSymbol> overriddenInThisType, ref bool stopLookup, ref bool haveExactMatch)
		{
			Symbol originalDefinition = sym.OriginalDefinition;
			NamedTypeSymbol originalDefinition2 = overridingContainingType.OriginalDefinition;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			bool flag = AccessCheck.IsSymbolAccessible(originalDefinition, originalDefinition2, null, ref useSiteInfo);
			TSymbol val;
			if (sym.Kind == overridingSym.Kind && OverrideHidingHelper.CanOverrideOrHide(sym))
			{
				val = (TSymbol)sym;
				bool exactMatch = true;
				bool exactMatchIgnoringCustomModifiers = true;
				bool num;
				if (!overridingIsFromSomeCompilation)
				{
					num = s_runtimeSignatureComparer.Equals(overridingSym, val);
				}
				else
				{
					if (SymbolExtensions.IsWithEventsProperty(sym) != SymbolExtensions.IsWithEventsProperty(overridingSym))
					{
						goto IL_00ed;
					}
					num = OverrideHidingHelper.SignaturesMatch(overridingSym, val, out exactMatch, out exactMatchIgnoringCustomModifiers);
				}
				if (num)
				{
					if (flag)
					{
						if (exactMatchIgnoringCustomModifiers)
						{
							if (exactMatch)
							{
								if (!haveExactMatch)
								{
									haveExactMatch = true;
									stopLookup = true;
									overriddenInThisType.Clear();
								}
								overriddenInThisType.Add(val);
							}
							else if (!haveExactMatch)
							{
								overriddenInThisType.Add(val);
							}
						}
						else
						{
							AddMemberToABuilder(val, inexactOverriddenMembers);
						}
					}
					else if (exactMatchIgnoringCustomModifiers)
					{
						inaccessibleBuilder.Add(val);
					}
					return;
				}
				goto IL_00ed;
			}
			if (flag)
			{
				stopLookup = true;
			}
			return;
			IL_00ed:
			if (!SymbolExtensions.IsOverloads(val) && flag)
			{
				stopLookup = true;
			}
		}

		private static void AddMemberToABuilder(TSymbol member, ArrayBuilder<TSymbol> builder)
		{
			NamedTypeSymbol containingType = member.ContainingType;
			int num = builder.Count - 1;
			int num2 = 0;
			while (true)
			{
				if (num2 <= num)
				{
					bool exactMatchIgnoringCustomModifiers = false;
					if (!TypeSymbol.Equals(builder[num2].ContainingType, containingType, TypeCompareKind.ConsiderEverything))
					{
						Symbol sym = builder[num2];
						Symbol sym2 = member;
						bool exactMatch = false;
						if (OverrideHidingHelper.SignaturesMatch(sym, sym2, out exactMatch, out exactMatchIgnoringCustomModifiers) && exactMatchIgnoringCustomModifiers)
						{
							break;
						}
					}
					num2++;
					continue;
				}
				builder.Add(member);
				break;
			}
		}

		internal static void CheckOverrideMember(TSymbol member, OverriddenMembersResult<TSymbol> overriddenMembersResult, BindingDiagnosticBag diagnostics)
		{
			_ = member.ShadowsExplicitly;
			SymbolExtensions.IsOverloads(member);
			ImmutableArray<TSymbol> immutableArray = overriddenMembersResult.OverriddenMembers;
			if (immutableArray.IsEmpty)
			{
				immutableArray = overriddenMembersResult.InexactOverriddenMembers;
			}
			if (immutableArray.Length == 0)
			{
				if (overriddenMembersResult.InaccessibleMembers.Length > 0)
				{
					ReportBadOverriding(ERRID.ERR_CannotOverrideInAccessibleMember, member, overriddenMembersResult.InaccessibleMembers[0], diagnostics);
					return;
				}
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_OverrideNotNeeded3, SymbolExtensions.GetKindText(member), member.Name), member.Locations[0]));
				return;
			}
			if (immutableArray.Length > 1)
			{
				ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance(immutableArray.Length);
				ImmutableArray<TSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TSymbol current = enumerator.Current;
					instance.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OverriddenCandidate1, current.OriginalDefinition));
				}
				diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_AmbiguousOverrides3, immutableArray[0], CustomSymbolDisplayFormatter.ShortErrorName(immutableArray[0].ContainingType), new CompoundDiagnosticInfo(instance.ToArrayAndFree())), member.Locations[0]));
				return;
			}
			TSymbol val = immutableArray[0];
			SymbolComparisonResults symbolComparisonResults = OverrideHidingHelper.DetailedSignatureCompare(member, val, SymbolComparisonResults.AllMismatches);
			if (val.IsNotOverridable)
			{
				ReportBadOverriding(ERRID.ERR_CantOverrideNotOverridable2, member, val, diagnostics);
				return;
			}
			if (!(val.IsOverridable | val.IsMustOverride | val.IsOverrides))
			{
				ReportBadOverriding(ERRID.ERR_CantOverride4, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.ParameterByrefMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithByref2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.OptionalParameterMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithOptional2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.ReturnTypeMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_InvalidOverrideDueToReturn2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.PropertyAccessorMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverridingPropertyKind2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.PropertyInitOnlyMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverridingInitOnlyProperty, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.ParamArrayMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithArrayVsParamArray2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.OptionalParameterTypeMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithOptionalTypes2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.OptionalParameterValueMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithDefault2, member, val, diagnostics);
				return;
			}
			if ((symbolComparisonResults & SymbolComparisonResults.ConstraintMismatch) != 0)
			{
				ReportBadOverriding(ERRID.ERR_OverrideWithConstraintMismatch2, member, val, diagnostics);
				return;
			}
			ERRID errorId = default(ERRID);
			if (!ConsistentAccessibility(member, val, ref errorId))
			{
				ReportBadOverriding(errorId, member, val, diagnostics);
				return;
			}
			if (SymbolExtensions.ContainsTupleNames(member) && (symbolComparisonResults & SymbolComparisonResults.TupleNamesMismatch) != 0)
			{
				ReportBadOverriding(ERRID.WRN_InvalidOverrideDueToTupleNames2, member, val, diagnostics);
				return;
			}
			ImmutableArray<TSymbol>.Enumerator enumerator2 = overriddenMembersResult.InaccessibleMembers.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				TSymbol current2 = enumerator2.Current;
				if (current2.DeclaredAccessibility == Accessibility.Internal && SymbolExtensions.OverriddenMember(current2) == val)
				{
					diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_InAccessibleOverridingMethod5, member, member.ContainingType, val, val.ContainingType, current2.ContainingType), member.Locations[0]));
				}
			}
			UseSiteInfo<AssemblySymbol> useSiteInfo = val.GetUseSiteInfo();
			if (!diagnostics.Add(useSiteInfo, member.Locations[0]) && member.Kind == SymbolKind.Property)
			{
				PropertySymbol obj = (PropertySymbol)(Symbol)member;
				PropertySymbol propertySymbol = (PropertySymbol)(Symbol)val;
				CheckOverridePropertyAccessor(obj.GetMethod, propertySymbol.GetMethod, diagnostics);
				CheckOverridePropertyAccessor(obj.SetMethod, propertySymbol.SetMethod, diagnostics);
			}
		}

		private static void RemoveMembersWithConflictingAccessibility(ArrayBuilder<TSymbol> members)
		{
			if (members.Count < 2)
			{
				return;
			}
			ArrayBuilder<TSymbol> instance = ArrayBuilder<TSymbol>.GetInstance();
			ArrayBuilder<TSymbol>.Enumerator enumerator = members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TSymbol current = enumerator.Current;
				bool flag = false;
				ArrayBuilder<TSymbol>.Enumerator enumerator2 = members.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TSymbol current2 = enumerator2.Current;
					if (current != current2)
					{
						Symbol originalDefinition = current.OriginalDefinition;
						Symbol originalDefinition2 = current2.OriginalDefinition;
						if (TypeSymbol.Equals(originalDefinition.ContainingType, originalDefinition2.ContainingType, TypeCompareKind.ConsiderEverything) && OverrideHidingHelper.DetailedSignatureCompare(originalDefinition, originalDefinition2, (SymbolComparisonResults)115525) == (SymbolComparisonResults)0 && LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(originalDefinition, originalDefinition2) < 0)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					instance.Add(current);
				}
			}
			if (instance.Count != members.Count)
			{
				members.Clear();
				members.AddRange(instance);
			}
			instance.Free();
		}

		internal static void CheckOverridePropertyAccessor(MethodSymbol overridingAccessor, MethodSymbol overriddenAccessor, BindingDiagnosticBag diagnostics)
		{
			if ((object)overridingAccessor != null && (object)overriddenAccessor != null)
			{
				MethodSymbol originalDefinition = overriddenAccessor.OriginalDefinition;
				NamedTypeSymbol containingType = overridingAccessor.ContainingType;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				ERRID errorId = default(ERRID);
				if (!AccessCheck.IsSymbolAccessible(originalDefinition, containingType, null, ref useSiteInfo))
				{
					ReportBadOverriding(ERRID.ERR_CannotOverrideInAccessibleMember, overridingAccessor, overriddenAccessor, diagnostics);
				}
				else if (!ConsistentAccessibility(overridingAccessor, overriddenAccessor, ref errorId))
				{
					ReportBadOverriding(errorId, overridingAccessor, overriddenAccessor, diagnostics);
				}
				diagnostics.Add(overriddenAccessor.GetUseSiteInfo(), overridingAccessor.Locations[0]);
			}
		}

		private static void ReportBadOverriding(ERRID id, Symbol overridingMember, Symbol overriddenMember, BindingDiagnosticBag diagnostics)
		{
			diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(id, overridingMember, overriddenMember), overridingMember.Locations[0]));
		}

		private static bool ConsistentAccessibility(Symbol overriding, Symbol overridden, ref ERRID errorId)
		{
			if ((overridden.DeclaredAccessibility == Accessibility.ProtectedOrInternal) & !(overriding.ContainingAssembly == overridden.ContainingAssembly))
			{
				errorId = ERRID.ERR_FriendAssemblyBadAccessOverride2;
				return overriding.DeclaredAccessibility == Accessibility.Protected;
			}
			errorId = ERRID.ERR_BadOverrideAccess2;
			return overridden.DeclaredAccessibility == overriding.DeclaredAccessibility;
		}
	}
}
