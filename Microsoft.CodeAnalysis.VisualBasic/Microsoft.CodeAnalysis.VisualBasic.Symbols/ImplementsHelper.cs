using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class ImplementsHelper
	{
		public static ImmutableArray<Symbol> GetExplicitInterfaceImplementations(Symbol member)
		{
			return member.Kind switch
			{
				SymbolKind.Method => StaticCast<Symbol>.From(((MethodSymbol)member).ExplicitInterfaceImplementations), 
				SymbolKind.Property => StaticCast<Symbol>.From(((PropertySymbol)member).ExplicitInterfaceImplementations), 
				SymbolKind.Event => StaticCast<Symbol>.From(((EventSymbol)member).ExplicitInterfaceImplementations), 
				_ => ImmutableArray<Symbol>.Empty, 
			};
		}

		public static Location GetImplementingLocation(Symbol sourceSym, Symbol implementedSym)
		{
			if (sourceSym is SourceMethodSymbol sourceMethodSymbol)
			{
				return sourceMethodSymbol.GetImplementingLocation((MethodSymbol)implementedSym);
			}
			if (sourceSym is SourcePropertySymbol sourcePropertySymbol)
			{
				return sourcePropertySymbol.GetImplementingLocation((PropertySymbol)implementedSym);
			}
			if (sourceSym is SourceEventSymbol sourceEventSymbol)
			{
				return sourceEventSymbol.GetImplementingLocation((EventSymbol)implementedSym);
			}
			throw ExceptionUtilities.Unreachable;
		}

		public static Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax FindImplementingSyntax<TSymbol>(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax implementsClause, TSymbol implementingSym, TSymbol implementedSym, SourceMemberContainerTypeSymbol container, Binder binder) where TSymbol : Symbol
		{
			SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax>.Enumerator enumerator = implementsClause.InterfaceMembers.GetEnumerator();
			LookupResultKind resultKind = default(LookupResultKind);
			while (enumerator.MoveNext())
			{
				Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax current = enumerator.Current;
				if ((Symbol)FindExplicitlyImplementedMember(implementingSym, container, current, binder, BindingDiagnosticBag.Discarded, null, ref resultKind) == (Symbol)implementedSym)
				{
					return current;
				}
			}
			return null;
		}

		public static ImmutableArray<TSymbol> ProcessImplementsClause<TSymbol>(Microsoft.CodeAnalysis.VisualBasic.Syntax.ImplementsClauseSyntax implementsClause, TSymbol implementingSym, SourceMemberContainerTypeSymbol container, Binder binder, BindingDiagnosticBag diagBag) where TSymbol : Symbol
		{
			if (!container.IsInterface)
			{
				if (TypeSymbolExtensions.IsModuleType(container))
				{
					Binder.ReportDiagnostic(diagBag, implementsClause.ImplementsKeyword, ERRID.ERR_ModuleMemberCantImplement);
					return ImmutableArray<TSymbol>.Empty;
				}
				ArrayBuilder<TSymbol> instance = ArrayBuilder<TSymbol>.GetInstance();
				ThreeState value = ThreeState.Unknown;
				bool flag = implementingSym.Kind == SymbolKind.Event;
				SeparatedSyntaxList<Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax>.Enumerator enumerator = implementsClause.InterfaceMembers.GetEnumerator();
				LookupResultKind resultKind = default(LookupResultKind);
				while (enumerator.MoveNext())
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax current = enumerator.Current;
					TSymbol val = FindExplicitlyImplementedMember(implementingSym, container, current, binder, diagBag, null, ref resultKind);
					if (val == null)
					{
						continue;
					}
					instance.Add(val);
					Binder.ReportDiagnosticsIfObsolete(diagBag, implementingSym, val, implementsClause);
					if (!flag)
					{
						continue;
					}
					if (!value.HasValue())
					{
						value = (val as EventSymbol).IsWindowsRuntimeEvent.ToThreeState();
						continue;
					}
					bool isWindowsRuntimeEvent = (val as EventSymbol).IsWindowsRuntimeEvent;
					bool flag2 = value.Value();
					if (isWindowsRuntimeEvent != flag2)
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_MixingWinRTAndNETEvents, CustomSymbolDisplayFormatter.ShortErrorName(implementingSym), CustomSymbolDisplayFormatter.QualifiedName(flag2 ? instance[0] : val), CustomSymbolDisplayFormatter.QualifiedName(flag2 ? val : instance[0]));
					}
				}
				return instance.ToImmutableAndFree();
			}
			Binder.ReportDiagnostic(id: (implementingSym.Kind == SymbolKind.Method) ? ERRID.ERR_BadInterfaceMethodFlags1 : ((implementingSym.Kind != SymbolKind.Property) ? ERRID.ERR_InterfaceCantUseEventSpecifier1 : ERRID.ERR_BadInterfacePropertyFlags1), diagBag: diagBag, syntax: implementsClause, args: new object[1] { implementsClause.ImplementsKeyword.ToString() });
			return ImmutableArray<TSymbol>.Empty;
		}

		public static TSymbol FindExplicitlyImplementedMember<TSymbol>(TSymbol implementingSym, NamedTypeSymbol containingType, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax implementedMemberSyntax, Binder binder, BindingDiagnosticBag diagBag, ArrayBuilder<Symbol> candidateSymbols, ref LookupResultKind resultKind) where TSymbol : Symbol
		{
			resultKind = LookupResultKind.Good;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.NameSyntax left = implementedMemberSyntax.Left;
			string valueText = implementedMemberSyntax.Right.Identifier.ValueText;
			TypeSymbol typeSymbol = binder.BindTypeSyntax(left, diagBag);
			if (TypeSymbolExtensions.IsInterfaceType(typeSymbol))
			{
				bool errorReported = false;
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)typeSymbol;
				if (!containingType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics[namedTypeSymbol].Contains(namedTypeSymbol))
				{
					Binder.ReportDiagnostic(diagBag, left, ERRID.ERR_InterfaceNotImplemented1, typeSymbol);
					resultKind = LookupResultKind.NotReferencable;
					errorReported = true;
				}
				LookupResult instance = LookupResult.GetInstance();
				TSymbol val = null;
				LookupOptions lookupOptions = LookupOptions.IgnoreAccessibility | LookupOptions.AllMethodsOfAnyArity | LookupOptions.IgnoreExtensionMethods;
				if (implementingSym.Kind == SymbolKind.Event)
				{
					lookupOptions |= LookupOptions.EventsOnly;
				}
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = binder.GetNewCompoundUseSiteInfo(diagBag);
				binder.LookupMember(instance, typeSymbol, valueText, -1, lookupOptions, ref useSiteInfo);
				if (instance.IsAmbiguous)
				{
					Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplementsMember3, valueText, valueText);
					candidateSymbols?.AddRange(((AmbiguousSymbolDiagnostic)instance.Diagnostic).AmbiguousSymbols);
					resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.Ambiguous);
					errorReported = true;
				}
				else if (instance.IsGood)
				{
					ArrayBuilder<TSymbol> arrayBuilder = null;
					ArrayBuilder<Symbol>.Enumerator enumerator = instance.Symbols.GetEnumerator();
					while (enumerator.MoveNext())
					{
						TSymbol val2 = enumerator.Current as TSymbol;
						if (val2 != null && val2.ContainingType.IsInterface && MembersAreMatchingForPurposesOfInterfaceImplementation(implementingSym, val2))
						{
							if (arrayBuilder == null)
							{
								arrayBuilder = ArrayBuilder<TSymbol>.GetInstance();
							}
							arrayBuilder.Add(val2);
						}
					}
					int num = arrayBuilder?.Count ?? 0;
					if (num > 1)
					{
						int num2 = arrayBuilder.Count - 2;
						for (int i = 0; i <= num2; i++)
						{
							TSymbol val3 = arrayBuilder[i];
							if (val3 == null)
							{
								continue;
							}
							int num3 = i + 1;
							int num4 = arrayBuilder.Count - 1;
							for (int j = num3; j <= num4; j++)
							{
								TSymbol val4 = arrayBuilder[j];
								if (val4 != null)
								{
									NamedTypeSymbol containingType2 = val4.ContainingType;
									NamedTypeSymbol containingType3 = val3.ContainingType;
									CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
									if (TypeSymbolExtensions.ImplementsInterface(containingType2, containingType3, null, ref useSiteInfo2))
									{
										arrayBuilder[i] = null;
										num--;
										break;
									}
									NamedTypeSymbol containingType4 = val3.ContainingType;
									NamedTypeSymbol containingType5 = val4.ContainingType;
									useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
									if (TypeSymbolExtensions.ImplementsInterface(containingType4, containingType5, null, ref useSiteInfo2))
									{
										arrayBuilder[j] = null;
										num--;
									}
								}
							}
						}
					}
					if (num > 1)
					{
						int num5 = arrayBuilder.Count - 2;
						int num6 = 0;
						while (true)
						{
							TSymbol val5;
							TSymbol val6;
							if (num6 <= num5)
							{
								val5 = arrayBuilder[num6];
								if (val5 != null)
								{
									if (val == null)
									{
										val = val5;
									}
									int num7 = num6 + 1;
									int num8 = arrayBuilder.Count - 1;
									int num9 = num7;
									while (num9 <= num8)
									{
										val6 = arrayBuilder[num9];
										if (val6 == null || !TypeSymbol.Equals(val5.ContainingType, val6.ContainingType, TypeCompareKind.ConsiderEverything))
										{
											num9++;
											continue;
										}
										goto IL_0357;
									}
								}
								num6++;
								continue;
							}
							Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplementsMember3, valueText, valueText);
							resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.Ambiguous);
							errorReported = true;
							break;
							IL_0357:
							Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_AmbiguousImplements3, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(val5.ContainingType), valueText, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(val5.ContainingType), val5, val6);
							errorReported = true;
							resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.OverloadResolutionFailure);
							break;
						}
						candidateSymbols?.AddRange(instance.Symbols);
					}
					else if (num == 1)
					{
						int num10 = arrayBuilder.Count - 1;
						for (int k = 0; k <= num10; k++)
						{
							TSymbol val7 = arrayBuilder[k];
							if (val7 != null)
							{
								val = val7;
								break;
							}
						}
					}
					else
					{
						candidateSymbols?.AddRange(instance.Symbols);
						resultKind = LookupResult.WorseResultKind(instance.Kind, LookupResultKind.OverloadResolutionFailure);
					}
					arrayBuilder?.Free();
					if (val != null)
					{
						if ((object)namedTypeSymbol.CoClassType != null && implementingSym.Kind == SymbolKind.Event != (val.Kind == SymbolKind.Event))
						{
							val = null;
						}
						if (!errorReported)
						{
							val = ValidateImplementedMember(implementingSym, val, implementedMemberSyntax, binder, diagBag, typeSymbol, valueText, ref errorReported);
						}
						if (val != null)
						{
							candidateSymbols?.Add(val);
							resultKind = LookupResult.WorseResultKind(resultKind, instance.Kind);
							if (!binder.IsAccessible(val, ref useSiteInfo))
							{
								resultKind = LookupResult.WorseResultKind(resultKind, LookupResultKind.Inaccessible);
								Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, binder.GetInaccessibleErrorInfo(val));
							}
							else if (val.Kind == SymbolKind.Property)
							{
								PropertySymbol propertySymbol = (PropertySymbol)(Symbol)val;
								MethodSymbol methodSymbol = propertySymbol.GetMethod;
								if ((object)methodSymbol == null || methodSymbol.DeclaredAccessibility == propertySymbol.DeclaredAccessibility || !SymbolExtensions.RequiresImplementation(methodSymbol))
								{
									methodSymbol = propertySymbol.SetMethod;
								}
								if ((object)methodSymbol != null && methodSymbol.DeclaredAccessibility != propertySymbol.DeclaredAccessibility && SymbolExtensions.RequiresImplementation(methodSymbol) && !binder.IsAccessible(methodSymbol, ref useSiteInfo))
								{
									Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, binder.GetInaccessibleErrorInfo(methodSymbol));
								}
							}
						}
					}
				}
				((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)left, useSiteInfo);
				instance.Free();
				if (val == null && !errorReported)
				{
					Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_IdentNotMemberOfInterface4, CustomSymbolDisplayFormatter.ShortErrorName(implementingSym), valueText, SymbolExtensions.GetKindText(implementingSym), CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(typeSymbol));
				}
				return val;
			}
			TSymbol result;
			if (typeSymbol.TypeKind == TypeKind.Error)
			{
				result = null;
			}
			else
			{
				Binder.ReportDiagnostic(diagBag, left, ERRID.ERR_BadImplementsType);
				result = null;
			}
			return result;
		}

		private static bool MembersAreMatchingForPurposesOfInterfaceImplementation(Symbol implementingSym, Symbol implementedSym)
		{
			return MembersAreMatching(implementingSym, implementedSym, (SymbolComparisonResults)(-139306), EventSignatureComparer.ExplicitEventImplementationComparer);
		}

		private static bool MembersHaveMatchingTupleNames(Symbol implementingSym, Symbol implementedSym)
		{
			return MembersAreMatching(implementingSym, implementedSym, SymbolComparisonResults.TupleNamesMismatch, EventSignatureComparer.ExplicitEventImplementationWithTupleNamesComparer);
		}

		private static bool MembersAreMatching(Symbol implementingSym, Symbol implementedSym, SymbolComparisonResults comparisons, EventSignatureComparer eventComparer)
		{
			return implementingSym.Kind switch
			{
				SymbolKind.Method => MethodSignatureComparer.DetailedCompare((MethodSymbol)implementedSym, (MethodSymbol)implementingSym, comparisons, comparisons) == (SymbolComparisonResults)0, 
				SymbolKind.Property => PropertySignatureComparer.DetailedCompare((PropertySymbol)implementedSym, (PropertySymbol)implementingSym, comparisons, comparisons) == (SymbolComparisonResults)0, 
				SymbolKind.Event => eventComparer.Equals((EventSymbol)implementedSym, (EventSymbol)implementingSym), 
				_ => throw ExceptionUtilities.UnexpectedValue(implementingSym.Kind), 
			};
		}

		private static TSymbol ValidateImplementedMember<TSymbol>(TSymbol implementingSym, TSymbol implementedSym, Microsoft.CodeAnalysis.VisualBasic.Syntax.QualifiedNameSyntax implementedMemberSyntax, Binder binder, BindingDiagnosticBag diagBag, TypeSymbol interfaceType, string implementedMethodName, ref bool errorReported) where TSymbol : Symbol
		{
			if (!SymbolExtensions.RequiresImplementation(implementedSym))
			{
				return null;
			}
			if (implementedSym.Kind == SymbolKind.Property)
			{
				PropertySymbol propertySymbol = implementedSym as PropertySymbol;
				MethodSymbol methodSymbol = propertySymbol.GetMethod;
				bool? flag = (((object)methodSymbol != null) ? new bool?(SymbolExtensions.RequiresImplementation(methodSymbol)) : null);
				if (((!flag) ?? flag).GetValueOrDefault())
				{
					methodSymbol = null;
				}
				MethodSymbol methodSymbol2 = propertySymbol.SetMethod;
				flag = (((object)methodSymbol2 != null) ? new bool?(SymbolExtensions.RequiresImplementation(methodSymbol2)) : null);
				if (((!flag) ?? flag).GetValueOrDefault())
				{
					methodSymbol2 = null;
				}
				PropertySymbol propertySymbol2 = implementingSym as PropertySymbol;
				if (((object)methodSymbol != null && (object)propertySymbol2.GetMethod == null) || ((object)methodSymbol2 != null && (object)propertySymbol2.SetMethod == null))
				{
					Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_PropertyDoesntImplementAllAccessors, propertySymbol, SymbolExtensions.GetPropertyKindText(propertySymbol2));
					errorReported = true;
				}
				else if ((((object)methodSymbol == null) ^ ((object)methodSymbol2 == null)) && (object)propertySymbol2.GetMethod != null && (object)propertySymbol2.SetMethod != null)
				{
					errorReported |= !Parser.CheckFeatureAvailability(diagBag, implementedMemberSyntax.GetLocation(), ((VisualBasicSyntaxTree)implementedMemberSyntax.SyntaxTree).Options.LanguageVersion, Feature.ImplementingReadonlyOrWriteonlyPropertyWithReadwrite);
				}
				flag = methodSymbol2?.IsInitOnly;
				bool? flag2 = propertySymbol2.SetMethod?.IsInitOnly;
				if (((flag.HasValue & flag2.HasValue) ? new bool?(flag.GetValueOrDefault() != flag2.GetValueOrDefault()) : null).GetValueOrDefault())
				{
					Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_PropertyDoesntImplementInitOnly, propertySymbol);
					errorReported = true;
				}
			}
			if (implementedSym != null && SymbolExtensions.ContainsTupleNames(implementingSym) && !MembersHaveMatchingTupleNames(implementingSym, implementedSym))
			{
				Binder.ReportDiagnostic(diagBag, implementedMemberSyntax, ERRID.ERR_ImplementingInterfaceWithDifferentTupleNames5, CustomSymbolDisplayFormatter.ShortErrorName(implementingSym), SymbolExtensions.GetKindText(implementingSym), implementedMethodName, CustomSymbolDisplayFormatter.ShortNameWithTypeArgs(interfaceType), implementingSym, implementedSym);
				errorReported = true;
			}
			return implementedSym;
		}

		public static void ValidateImplementedMethodConstraints(SourceMethodSymbol implementingMethod, MethodSymbol implementedMethod, BindingDiagnosticBag diagBag)
		{
			if (!MethodSignatureComparer.HaveSameConstraints(implementedMethod, implementingMethod))
			{
				Location implementingLocation = implementingMethod.GetImplementingLocation(implementedMethod);
				diagBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ImplementsWithConstraintMismatch3, implementingMethod, implementedMethod.ContainingType, implementedMethod), implementingLocation);
			}
		}

		public static TSymbol ComputeImplementationForInterfaceMember<TSymbol>(TSymbol interfaceMember, TypeSymbol implementingType, IEqualityComparer<TSymbol> comparer) where TSymbol : Symbol
		{
			NamedTypeSymbol containingType = interfaceMember.ContainingType;
			bool flag = false;
			TypeSymbol typeSymbol = implementingType;
			TSymbol result;
			while (true)
			{
				if ((object)typeSymbol != null)
				{
					MultiDictionary<Symbol, Symbol>.ValueSet valueSet = typeSymbol.ExplicitInterfaceImplementationMap[interfaceMember];
					if (valueSet.Count == 1)
					{
						return (TSymbol)valueSet.Single();
					}
					if (valueSet.Count > 1)
					{
						result = null;
						break;
					}
					if (!typeSymbol.Dangerous_IsFromSomeCompilationIncludingRetargeting && ((IReadOnlyList<NamedTypeSymbol>)typeSymbol.InterfacesNoUseSiteDiagnostics).Contains(containingType, (IEqualityComparer<NamedTypeSymbol>?)EqualsIgnoringComparer.InstanceCLRSignatureCompare))
					{
						flag = true;
					}
					if (flag)
					{
						TSymbol val = FindImplicitImplementationDeclaredInType(interfaceMember, typeSymbol, comparer);
						if (val != null)
						{
							return val;
						}
					}
					typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
					continue;
				}
				result = null;
				break;
			}
			return result;
		}

		private static TSymbol FindImplicitImplementationDeclaredInType<TSymbol>(TSymbol interfaceMember, TypeSymbol currType, IEqualityComparer<TSymbol> comparer) where TSymbol : Symbol
		{
			ImmutableArray<Symbol>.Enumerator enumerator = currType.GetMembers(interfaceMember.Name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.DeclaredAccessibility == Accessibility.Public && !current.IsShared && current is TSymbol && comparer.Equals(interfaceMember, (TSymbol)current))
				{
					return (TSymbol)current;
				}
			}
			return null;
		}

		public static ImmutableArray<TSymbol> SubstituteExplicitInterfaceImplementations<TSymbol>(ImmutableArray<TSymbol> unsubstitutedImplementations, TypeSubstitution substitution) where TSymbol : Symbol
		{
			if (unsubstitutedImplementations.Length == 0)
			{
				return ImmutableArray<TSymbol>.Empty;
			}
			TSymbol[] array = new TSymbol[unsubstitutedImplementations.Length - 1 + 1];
			int num = unsubstitutedImplementations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TSymbol val = unsubstitutedImplementations[i];
				NamedTypeSymbol containingType = val.ContainingType;
				array[i] = unsubstitutedImplementations[i];
				if (containingType.IsGenericType && containingType.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly() is SubstitutedNamedType substitutedNamedType)
				{
					array[i] = (TSymbol)substitutedNamedType.GetMemberForDefinition(val.OriginalDefinition);
				}
			}
			return ImmutableArray.Create(array);
		}
	}
}
