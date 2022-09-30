using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TupleTypeSymbol : WrappedNamedTypeSymbol
	{
		private readonly ImmutableArray<Location> _locations;

		private readonly ImmutableArray<Location> _elementLocations;

		private readonly ImmutableArray<string> _providedElementNames;

		private readonly ImmutableArray<bool> _errorPositions;

		private ImmutableArray<string> _lazyActualElementNames;

		private readonly ImmutableArray<TypeSymbol> _elementTypes;

		private ImmutableArray<Symbol> _lazyMembers;

		private ImmutableArray<FieldSymbol> _lazyFields;

		private SmallDictionary<Symbol, Symbol> _lazyUnderlyingDefinitionToMemberMap;

		internal const int RestPosition = 8;

		internal const int RestIndex = 7;

		internal const string TupleTypeName = "ValueTuple";

		internal const string RestFieldName = "Rest";

		private static readonly WellKnownType[] tupleTypes = new WellKnownType[8]
		{
			WellKnownType.System_ValueTuple_T1,
			WellKnownType.System_ValueTuple_T2,
			WellKnownType.System_ValueTuple_T3,
			WellKnownType.System_ValueTuple_T4,
			WellKnownType.System_ValueTuple_T5,
			WellKnownType.System_ValueTuple_T6,
			WellKnownType.System_ValueTuple_T7,
			WellKnownType.System_ValueTuple_TRest
		};

		private static readonly WellKnownMember[] tupleCtors = new WellKnownMember[8]
		{
			WellKnownMember.System_ValueTuple_T1__ctor,
			WellKnownMember.System_ValueTuple_T2__ctor,
			WellKnownMember.System_ValueTuple_T3__ctor,
			WellKnownMember.System_ValueTuple_T4__ctor,
			WellKnownMember.System_ValueTuple_T5__ctor,
			WellKnownMember.System_ValueTuple_T6__ctor,
			WellKnownMember.System_ValueTuple_T7__ctor,
			WellKnownMember.System_ValueTuple_TRest__ctor
		};

		private static readonly WellKnownMember[][] tupleMembers = new WellKnownMember[8][]
		{
			new WellKnownMember[1] { WellKnownMember.System_ValueTuple_T1__Item1 },
			new WellKnownMember[2]
			{
				WellKnownMember.System_ValueTuple_T2__Item1,
				WellKnownMember.System_ValueTuple_T2__Item2
			},
			new WellKnownMember[3]
			{
				WellKnownMember.System_ValueTuple_T3__Item1,
				WellKnownMember.System_ValueTuple_T3__Item2,
				WellKnownMember.System_ValueTuple_T3__Item3
			},
			new WellKnownMember[4]
			{
				WellKnownMember.System_ValueTuple_T4__Item1,
				WellKnownMember.System_ValueTuple_T4__Item2,
				WellKnownMember.System_ValueTuple_T4__Item3,
				WellKnownMember.System_ValueTuple_T4__Item4
			},
			new WellKnownMember[5]
			{
				WellKnownMember.System_ValueTuple_T5__Item1,
				WellKnownMember.System_ValueTuple_T5__Item2,
				WellKnownMember.System_ValueTuple_T5__Item3,
				WellKnownMember.System_ValueTuple_T5__Item4,
				WellKnownMember.System_ValueTuple_T5__Item5
			},
			new WellKnownMember[6]
			{
				WellKnownMember.System_ValueTuple_T6__Item1,
				WellKnownMember.System_ValueTuple_T6__Item2,
				WellKnownMember.System_ValueTuple_T6__Item3,
				WellKnownMember.System_ValueTuple_T6__Item4,
				WellKnownMember.System_ValueTuple_T6__Item5,
				WellKnownMember.System_ValueTuple_T6__Item6
			},
			new WellKnownMember[7]
			{
				WellKnownMember.System_ValueTuple_T7__Item1,
				WellKnownMember.System_ValueTuple_T7__Item2,
				WellKnownMember.System_ValueTuple_T7__Item3,
				WellKnownMember.System_ValueTuple_T7__Item4,
				WellKnownMember.System_ValueTuple_T7__Item5,
				WellKnownMember.System_ValueTuple_T7__Item6,
				WellKnownMember.System_ValueTuple_T7__Item7
			},
			new WellKnownMember[8]
			{
				WellKnownMember.System_ValueTuple_TRest__Item1,
				WellKnownMember.System_ValueTuple_TRest__Item2,
				WellKnownMember.System_ValueTuple_TRest__Item3,
				WellKnownMember.System_ValueTuple_TRest__Item4,
				WellKnownMember.System_ValueTuple_TRest__Item5,
				WellKnownMember.System_ValueTuple_TRest__Item6,
				WellKnownMember.System_ValueTuple_TRest__Item7,
				WellKnownMember.System_ValueTuple_TRest__Rest
			}
		};

		private static readonly HashSet<string> ForbiddenNames = new HashSet<string>(new string[6] { "CompareTo", "Deconstruct", "Equals", "GetHashCode", "Rest", "ToString" }, CaseInsensitiveComparison.Comparer);

		public override bool IsTupleType => true;

		public override NamedTypeSymbol TupleUnderlyingType => _underlyingType;

		public override ImmutableArray<TypeSymbol> TupleElementTypes => _elementTypes;

		public override ImmutableArray<string> TupleElementNames
		{
			get
			{
				if (_providedElementNames.IsDefault)
				{
					return default(ImmutableArray<string>);
				}
				if (_lazyActualElementNames.IsDefault)
				{
					_lazyActualElementNames = TupleElements.SelectAsArray((FieldSymbol e) => (!e.IsImplicitlyDeclared) ? e.Name : null);
				}
				return _lazyActualElementNames;
			}
		}

		public override bool IsImplicitlyDeclared => false;

		public override ImmutableArray<FieldSymbol> TupleElements
		{
			get
			{
				if (_lazyFields.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyFields, CollectTupleElementFields());
				}
				return _lazyFields;
			}
		}

		internal SmallDictionary<Symbol, Symbol> UnderlyingDefinitionToMemberMap
		{
			get
			{
				if (_lazyUnderlyingDefinitionToMemberMap == null)
				{
					_lazyUnderlyingDefinitionToMemberMap = ComputeDefinitionToMemberMap();
				}
				return _lazyUnderlyingDefinitionToMemberMap;
			}
		}

		public override NamedTypeSymbol EnumUnderlyingType => _underlyingType.EnumUnderlyingType;

		public override SymbolKind Kind => SymbolKind.NamedType;

		public override TypeKind TypeKind => TypeKind.Struct;

		public override Symbol ContainingSymbol => _underlyingType.ContainingSymbol;

		public override ImmutableArray<Location> Locations => _locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<VisualBasicSyntaxNode>(_locations);

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (TypeSymbolExtensions.IsErrorType(_underlyingType))
				{
					return Accessibility.Public;
				}
				return _underlyingType.DeclaredAccessibility;
			}
		}

		public override bool IsMustInherit => false;

		public override bool IsNotInheritable => true;

		public override int Arity => 0;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		internal override bool HasTypeArgumentsCustomModifiers => false;

		internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

		public override NamedTypeSymbol ConstructedFrom => this;

		public override bool MightContainExtensionMethods => false;

		public override string Name => string.Empty;

		internal override bool MangleName => false;

		public override IEnumerable<string> MemberNames => new VB_0024StateMachine_67_get_MemberNames(-2)
		{
			_0024VB_0024Me = this
		};

		internal override bool HasSpecialName => false;

		internal override bool IsComImport => false;

		internal override bool IsWindowsRuntimeImport => false;

		internal override bool ShouldAddWinRTMembers => false;

		public override bool IsSerializable => _underlyingType.IsSerializable;

		internal override TypeLayout Layout => _underlyingType.Layout;

		internal override CharSet MarshallingCharSet => _underlyingType.MarshallingCharSet;

		internal override bool HasDeclarativeSecurity => _underlyingType.HasDeclarativeSecurity;

		internal override bool IsInterface => false;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => _underlyingType.IsExtensibleInterfaceNoUseSiteDiagnostics;

		internal override bool CanConstruct => false;

		internal override TypeSubstitution TypeSubstitution => null;

		public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetEmptyTypeArgumentCustomModifiers(ordinal);
		}

		private TupleTypeSymbol(Location locationOpt, NamedTypeSymbol underlyingType, ImmutableArray<Location> elementLocations, ImmutableArray<string> elementNames, ImmutableArray<TypeSymbol> elementTypes, ImmutableArray<bool> errorPositions)
			: this(((object)locationOpt == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(locationOpt), underlyingType, elementLocations, elementNames, elementTypes, errorPositions)
		{
		}

		private TupleTypeSymbol(ImmutableArray<Location> locations, NamedTypeSymbol underlyingType, ImmutableArray<Location> elementLocations, ImmutableArray<string> elementNames, ImmutableArray<TypeSymbol> elementTypes, ImmutableArray<bool> errorPositions)
			: base(underlyingType)
		{
			_elementLocations = elementLocations;
			_providedElementNames = elementNames;
			_elementTypes = elementTypes;
			_locations = locations;
			_errorPositions = errorPositions;
		}

		internal static TupleTypeSymbol Create(Location locationOpt, ImmutableArray<TypeSymbol> elementTypes, ImmutableArray<Location> elementLocations, ImmutableArray<string> elementNames, VisualBasicCompilation compilation, bool shouldCheckConstraints, ImmutableArray<bool> errorPositions, SyntaxNode syntax = null, BindingDiagnosticBag diagnostics = null)
		{
			if (elementTypes.Length <= 1)
			{
				throw ExceptionUtilities.Unreachable;
			}
			NamedTypeSymbol tupleUnderlyingType = GetTupleUnderlyingType(elementTypes, syntax, compilation, diagnostics);
			if (diagnostics?.DiagnosticBag != null && ((SourceModuleSymbol)compilation.SourceModule).AnyReferencedAssembliesAreLinked)
			{
				EmbeddedTypesManager.IsValidEmbeddableType(tupleUnderlyingType, syntax, diagnostics.DiagnosticBag);
			}
			TupleTypeSymbol tupleTypeSymbol = Create(locationOpt, tupleUnderlyingType, elementLocations, elementNames, errorPositions);
			if (shouldCheckConstraints)
			{
				ConstraintsHelper.CheckConstraints(tupleTypeSymbol, syntax, elementLocations, diagnostics, new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, compilation.Assembly));
			}
			return tupleTypeSymbol;
		}

		public static TupleTypeSymbol Create(NamedTypeSymbol tupleCompatibleType)
		{
			return Create(ImmutableArray<Location>.Empty, tupleCompatibleType, default(ImmutableArray<Location>), default(ImmutableArray<string>), default(ImmutableArray<bool>));
		}

		public static TupleTypeSymbol Create(NamedTypeSymbol tupleCompatibleType, ImmutableArray<string> elementNames)
		{
			return Create(ImmutableArray<Location>.Empty, tupleCompatibleType, default(ImmutableArray<Location>), elementNames, default(ImmutableArray<bool>));
		}

		public static TupleTypeSymbol Create(Location locationOpt, NamedTypeSymbol tupleCompatibleType, ImmutableArray<Location> elementLocations, ImmutableArray<string> elementNames, ImmutableArray<bool> errorPositions)
		{
			return Create(((object)locationOpt == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(locationOpt), tupleCompatibleType, elementLocations, elementNames, errorPositions);
		}

		public static TupleTypeSymbol Create(ImmutableArray<Location> locations, NamedTypeSymbol tupleCompatibleType, ImmutableArray<Location> elementLocations, ImmutableArray<string> elementNames, ImmutableArray<bool> errorPositions)
		{
			ImmutableArray<TypeSymbol> elementTypes;
			if (tupleCompatibleType.Arity == 8)
			{
				tupleCompatibleType = EnsureRestExtensionsAreTuples(tupleCompatibleType);
				ImmutableArray<TypeSymbol> tupleElementTypes = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics[7].TupleElementTypes;
				ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(7 + tupleElementTypes.Length);
				instance.AddRange(tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics, 7);
				instance.AddRange(tupleElementTypes);
				elementTypes = instance.ToImmutableAndFree();
			}
			else
			{
				elementTypes = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics;
			}
			return new TupleTypeSymbol(locations, tupleCompatibleType, elementLocations, elementNames, elementTypes, errorPositions);
		}

		private static NamedTypeSymbol EnsureRestExtensionsAreTuples(NamedTypeSymbol tupleCompatibleType)
		{
			if (!tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics[7].IsTupleType)
			{
				ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
				NamedTypeSymbol namedTypeSymbol = tupleCompatibleType;
				do
				{
					instance.Add(namedTypeSymbol);
					namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[7];
				}
				while (namedTypeSymbol.Arity == 8);
				if (!namedTypeSymbol.IsTupleType)
				{
					instance.Add(namedTypeSymbol);
				}
				tupleCompatibleType = instance.Pop();
				ArrayBuilder<TypeWithModifiers> instance2 = ArrayBuilder<TypeWithModifiers>.GetInstance(8);
				do
				{
					TupleTypeSymbol extensionTuple = Create(null, tupleCompatibleType, default(ImmutableArray<Location>), default(ImmutableArray<string>), default(ImmutableArray<bool>));
					tupleCompatibleType = instance.Pop();
					tupleCompatibleType = ReplaceRestExtensionType(tupleCompatibleType, instance2, extensionTuple);
				}
				while (instance.Count != 0);
				instance2.Free();
				instance.Free();
			}
			return tupleCompatibleType;
		}

		private static NamedTypeSymbol ReplaceRestExtensionType(NamedTypeSymbol tupleCompatibleType, ArrayBuilder<TypeWithModifiers> typeArgumentsBuilder, TupleTypeSymbol extensionTuple)
		{
			bool hasTypeArgumentsCustomModifiers = tupleCompatibleType.HasTypeArgumentsCustomModifiers;
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = tupleCompatibleType.TypeArgumentsNoUseSiteDiagnostics;
			typeArgumentsBuilder.Clear();
			int num = 0;
			do
			{
				typeArgumentsBuilder.Add(new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics[num], hasTypeArgumentsCustomModifiers ? tupleCompatibleType.GetTypeArgumentCustomModifiers(num) : default(ImmutableArray<CustomModifier>)));
				num++;
			}
			while (num <= 6);
			typeArgumentsBuilder.Add(new TypeWithModifiers(extensionTuple, hasTypeArgumentsCustomModifiers ? tupleCompatibleType.GetTypeArgumentCustomModifiers(7) : default(ImmutableArray<CustomModifier>)));
			NamedTypeSymbol constructedFrom = tupleCompatibleType.ConstructedFrom;
			TypeSubstitution substitution = TypeSubstitution.Create(constructedFrom, constructedFrom.TypeParameters, typeArgumentsBuilder.ToImmutable());
			return constructedFrom.Construct(substitution);
		}

		private static ImmutableArray<CustomModifier> GetModifiers(ImmutableArray<ImmutableArray<CustomModifier>> modifiers, int i)
		{
			if (!modifiers.IsDefaultOrEmpty)
			{
				return modifiers[i];
			}
			return default(ImmutableArray<CustomModifier>);
		}

		internal TupleTypeSymbol WithUnderlyingType(NamedTypeSymbol newUnderlyingType)
		{
			return Create(_locations, newUnderlyingType, _elementLocations, _providedElementNames, _errorPositions);
		}

		internal TupleTypeSymbol WithElementNames(ImmutableArray<string> newElementNames)
		{
			if (_providedElementNames.IsDefault)
			{
				if (newElementNames.IsDefault)
				{
					return this;
				}
			}
			else if (!newElementNames.IsDefault && _providedElementNames.SequenceEqual(newElementNames))
			{
				return this;
			}
			return new TupleTypeSymbol(null, _underlyingType, default(ImmutableArray<Location>), newElementNames, _elementTypes, default(ImmutableArray<bool>));
		}

		internal static void GetUnderlyingTypeChain(NamedTypeSymbol underlyingTupleType, ArrayBuilder<NamedTypeSymbol> underlyingTupleTypeChain)
		{
			NamedTypeSymbol namedTypeSymbol = underlyingTupleType;
			while (true)
			{
				underlyingTupleTypeChain.Add(namedTypeSymbol);
				if (namedTypeSymbol.Arity == 8)
				{
					namedTypeSymbol = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[7].TupleUnderlyingType;
					continue;
				}
				break;
			}
		}

		internal static void AddElementTypes(NamedTypeSymbol underlyingTupleType, ArrayBuilder<TypeSymbol> tupleElementTypes)
		{
			NamedTypeSymbol namedTypeSymbol = underlyingTupleType;
			while (true)
			{
				if (!namedTypeSymbol.IsTupleType)
				{
					int length = Math.Min(namedTypeSymbol.Arity, 7);
					tupleElementTypes.AddRange(namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics, length);
					if (namedTypeSymbol.Arity == 8)
					{
						namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[7];
						continue;
					}
					break;
				}
				tupleElementTypes.AddRange(namedTypeSymbol.TupleElementTypes);
				break;
			}
		}

		private static NamedTypeSymbol GetNestedTupleUnderlyingType(NamedTypeSymbol topLevelUnderlyingType, int depth)
		{
			NamedTypeSymbol namedTypeSymbol = topLevelUnderlyingType;
			int num = depth - 1;
			for (int i = 0; i <= num; i++)
			{
				namedTypeSymbol = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[7].TupleUnderlyingType;
			}
			return namedTypeSymbol;
		}

		private static int NumberOfValueTuples(int numElements, out int remainder)
		{
			remainder = (numElements - 1) % 7 + 1;
			return (numElements - 1) / 7 + 1;
		}

		private static NamedTypeSymbol GetTupleUnderlyingType(ImmutableArray<TypeSymbol> elementTypes, SyntaxNode syntax, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			int remainder;
			int num = NumberOfValueTuples(elementTypes.Length, out remainder);
			NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(GetTupleType(remainder));
			if (diagnostics != null && syntax != null)
			{
				Binder.ReportUseSite(diagnostics, syntax, wellKnownType);
			}
			NamedTypeSymbol namedTypeSymbol = wellKnownType.Construct(ImmutableArray.Create(elementTypes, (num - 1) * 7, remainder));
			int num2 = num - 1;
			if (num2 > 0)
			{
				NamedTypeSymbol wellKnownType2 = compilation.GetWellKnownType(GetTupleType(8));
				if (diagnostics != null && syntax != null)
				{
					Binder.ReportUseSite(diagnostics, syntax, wellKnownType2);
				}
				do
				{
					ImmutableArray<TypeSymbol> typeArguments = ImmutableArray.Create(elementTypes, (num2 - 1) * 7, 7).Add(namedTypeSymbol);
					namedTypeSymbol = wellKnownType2.Construct(typeArguments);
					num2--;
				}
				while (num2 > 0);
			}
			return namedTypeSymbol;
		}

		internal static void VerifyTupleTypePresent(int cardinality, VisualBasicSyntaxNode syntax, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			int remainder;
			int num = NumberOfValueTuples(cardinality, out remainder);
			Binder.ReportUseSite(symbol: compilation.GetWellKnownType(GetTupleType(remainder)), diagBag: diagnostics, syntax: syntax);
			if (num > 1)
			{
				NamedTypeSymbol wellKnownType2 = compilation.GetWellKnownType(GetTupleType(8));
				Binder.ReportUseSite(diagnostics, syntax, wellKnownType2);
			}
		}

		private static WellKnownType GetTupleType(int arity)
		{
			if (arity > 8)
			{
				throw ExceptionUtilities.Unreachable;
			}
			return tupleTypes[arity - 1];
		}

		internal static WellKnownMember GetTupleCtor(int arity)
		{
			if (arity > 8)
			{
				throw ExceptionUtilities.Unreachable;
			}
			return tupleCtors[arity - 1];
		}

		internal static WellKnownMember GetTupleTypeMember(int arity, int position)
		{
			return tupleMembers[arity - 1][position - 1];
		}

		internal static string TupleMemberName(int position)
		{
			return "Item" + Microsoft.VisualBasic.CompilerServices.Conversions.ToString(position);
		}

		private static bool IsElementNameForbidden(string name)
		{
			return ForbiddenNames.Contains(name);
		}

		internal static int IsElementNameReserved(string name)
		{
			int result;
			if (IsElementNameForbidden(name))
			{
				result = 0;
			}
			else
			{
				if (CaseInsensitiveComparison.StartsWith(name, "Item") && int.TryParse(name.Substring(4), out var result2) && result2 > 0 && CaseInsensitiveComparison.Equals(name, TupleMemberName(result2)))
				{
					return result2;
				}
				result = -1;
			}
			return result;
		}

		private static Symbol GetWellKnownMemberInType(NamedTypeSymbol type, WellKnownMember relativeMember)
		{
			MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(relativeMember);
			return VisualBasicCompilation.GetRuntimeMember(type, ref descriptor, VisualBasicCompilation.SpecialMembersSignatureComparer.Instance, null);
		}

		internal static Symbol GetWellKnownMemberInType(NamedTypeSymbol type, WellKnownMember relativeMember, BindingDiagnosticBag diagnostics, SyntaxNode syntax)
		{
			Symbol wellKnownMemberInType = GetWellKnownMemberInType(type, relativeMember);
			if ((object)wellKnownMemberInType == null)
			{
				MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(relativeMember);
				Binder.ReportDiagnostic(diagnostics, syntax, ERRID.ERR_MissingRuntimeHelper, type.Name + "." + descriptor.Name);
			}
			else
			{
				UseSiteInfo<AssemblySymbol> useSiteInfo = wellKnownMemberInType.GetUseSiteInfo();
				diagnostics.Add(useSiteInfo, syntax.GetLocation());
			}
			return wellKnownMemberInType;
		}

		private ImmutableArray<FieldSymbol> CollectTupleElementFields()
		{
			ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance(_elementTypes.Length, null);
			ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Field)
				{
					FieldSymbol fieldSymbol = (FieldSymbol)current;
					int tupleElementIndex = fieldSymbol.TupleElementIndex;
					if (tupleElementIndex >= 0 && ((object)instance[tupleElementIndex] == null || instance[tupleElementIndex].IsDefaultTupleElement))
					{
						instance[tupleElementIndex] = fieldSymbol;
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			if (_lazyMembers.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref _lazyMembers, CreateMembers());
			}
			return _lazyMembers;
		}

		private ImmutableArray<Symbol> CreateMembers()
		{
			ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance(_elementTypes.Length, fillWithValue: false);
			ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance(Math.Max(_elementTypes.Length, _underlyingType.OriginalDefinition.GetMembers().Length));
			NamedTypeSymbol namedTypeSymbol = _underlyingType;
			int num = 0;
			ArrayBuilder<FieldSymbol> instance3 = ArrayBuilder<FieldSymbol>.GetInstance(namedTypeSymbol.Arity);
			CollectTargetTupleFields(namedTypeSymbol, instance3);
			ImmutableArray<Symbol> members = namedTypeSymbol.OriginalDefinition.GetMembers();
			while (true)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = members.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					switch (current.Kind)
					{
					case SymbolKind.Method:
						if (num == 0)
						{
							instance2.Add(new TupleMethodSymbol(this, SymbolExtensions.AsMember((MethodSymbol)current, namedTypeSymbol)));
						}
						break;
					case SymbolKind.Field:
					{
						FieldSymbol fieldSymbol = (FieldSymbol)current;
						int num2 = instance3.IndexOf(fieldSymbol, ReferenceEqualityComparer.Instance);
						if (num2 >= 0)
						{
							if (num != 0)
							{
								num2 += 7 * num;
							}
							string text = (_providedElementNames.IsDefault ? null : _providedElementNames[num2]);
							Location location = (_elementLocations.IsDefault ? null : _elementLocations[num2]);
							string text2 = TupleMemberName(num2 + 1);
							bool flag = !CaseInsensitiveComparison.Equals(text, text2);
							FieldSymbol underlyingField = fieldSymbol.AsMember(namedTypeSymbol);
							TupleElementFieldSymbol tupleElementFieldSymbol = ((num == 0) ? new TupleElementFieldSymbol(this, underlyingField, num2, location, flag, null) : new TupleVirtualElementFieldSymbol(this, underlyingField, text2, cannotUse: false, num2, location, flag, null));
							instance2.Add(tupleElementFieldSymbol);
							if (flag && !string.IsNullOrEmpty(text))
							{
								bool cannotUse = !_errorPositions.IsDefault && _errorPositions[num2];
								instance2.Add(new TupleVirtualElementFieldSymbol(this, underlyingField, text, cannotUse, num2, location, isImplicitlyDeclared: false, tupleElementFieldSymbol));
							}
							instance[num2] = true;
						}
						else if (num == 0)
						{
							instance2.Add(new TupleFieldSymbol(this, fieldSymbol.AsMember(namedTypeSymbol), -instance2.Count - 1));
						}
						break;
					}
					case SymbolKind.Property:
						if (num == 0)
						{
							instance2.Add(new TuplePropertySymbol(this, SymbolExtensions.AsMember((PropertySymbol)current, namedTypeSymbol)));
						}
						break;
					case SymbolKind.Event:
						if (num == 0)
						{
							instance2.Add(new TupleEventSymbol(this, SymbolExtensions.AsMember((EventSymbol)current, namedTypeSymbol)));
						}
						break;
					default:
						if (num == 0)
						{
							throw ExceptionUtilities.UnexpectedValue(current.Kind);
						}
						break;
					case SymbolKind.NamedType:
						break;
					}
				}
				if (namedTypeSymbol.Arity != 8)
				{
					break;
				}
				namedTypeSymbol = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[7].TupleUnderlyingType;
				num++;
				if (namedTypeSymbol.Arity != 8)
				{
					members = namedTypeSymbol.OriginalDefinition.GetMembers();
					instance3.Clear();
					CollectTargetTupleFields(namedTypeSymbol, instance3);
				}
			}
			instance3.Free();
			int num3 = instance.Count - 1;
			for (int i = 0; i <= num3; i++)
			{
				if (!instance[i])
				{
					int remainder;
					int num4 = NumberOfValueTuples(i + 1, out remainder);
					NamedTypeSymbol originalDefinition = GetNestedTupleUnderlyingType(_underlyingType, num4 - 1).OriginalDefinition;
					DiagnosticInfo useSiteDiagnosticInfo = (TypeSymbolExtensions.IsErrorType(originalDefinition) ? null : ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, originalDefinition.Name + "." + TupleMemberName(remainder)));
					string text3 = (_providedElementNames.IsDefault ? null : _providedElementNames[i]);
					Location location2 = (_elementLocations.IsDefault ? null : _elementLocations[i]);
					string text4 = TupleMemberName(i + 1);
					bool flag2 = !CaseInsensitiveComparison.Equals(text3, text4);
					TupleErrorFieldSymbol tupleErrorFieldSymbol = new TupleErrorFieldSymbol(this, text4, i, flag2 ? null : location2, _elementTypes[i], useSiteDiagnosticInfo, flag2, null);
					instance2.Add(tupleErrorFieldSymbol);
					if (flag2 && !string.IsNullOrEmpty(text3))
					{
						instance2.Add(new TupleErrorFieldSymbol(this, text3, i, location2, _elementTypes[i], useSiteDiagnosticInfo, isImplicitlyDeclared: false, tupleErrorFieldSymbol));
					}
				}
			}
			return instance2.ToImmutableAndFree();
		}

		private static void CollectTargetTupleFields(NamedTypeSymbol underlying, ArrayBuilder<FieldSymbol> fieldsForElements)
		{
			underlying = underlying.OriginalDefinition;
			int num = Math.Min(underlying.Arity, 7) - 1;
			for (int i = 0; i <= num; i++)
			{
				WellKnownMember tupleTypeMember = GetTupleTypeMember(underlying.Arity, i + 1);
				fieldsForElements.Add((FieldSymbol)GetWellKnownMemberInType(underlying, tupleTypeMember));
			}
		}

		private SmallDictionary<Symbol, Symbol> ComputeDefinitionToMemberMap()
		{
			SmallDictionary<Symbol, Symbol> smallDictionary = new SmallDictionary<Symbol, Symbol>(ReferenceEqualityComparer.Instance);
			_ = _underlyingType.OriginalDefinition;
			ImmutableArray<Symbol> members = GetMembers();
			for (int num = members.Length - 1; num >= 0; num--)
			{
				Symbol symbol = members[num];
				switch (symbol.Kind)
				{
				case SymbolKind.Event:
				{
					EventSymbol tupleUnderlyingEvent = ((EventSymbol)symbol).TupleUnderlyingEvent;
					FieldSymbol associatedField = tupleUnderlyingEvent.AssociatedField;
					if ((object)associatedField != null)
					{
						smallDictionary.Add(associatedField.OriginalDefinition, new TupleFieldSymbol(this, associatedField, -num - 1));
					}
					smallDictionary.Add(tupleUnderlyingEvent.OriginalDefinition, symbol);
					break;
				}
				case SymbolKind.Field:
				{
					FieldSymbol tupleUnderlyingField = ((FieldSymbol)symbol).TupleUnderlyingField;
					if ((object)tupleUnderlyingField != null)
					{
						smallDictionary[tupleUnderlyingField.OriginalDefinition] = symbol;
					}
					break;
				}
				case SymbolKind.Method:
					smallDictionary.Add(((MethodSymbol)symbol).TupleUnderlyingMethod.OriginalDefinition, symbol);
					break;
				case SymbolKind.Property:
					smallDictionary.Add(((PropertySymbol)symbol).TupleUnderlyingProperty.OriginalDefinition, symbol);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
				}
			}
			return smallDictionary;
		}

		public TMember GetTupleMemberSymbolForUnderlyingMember<TMember>(TMember underlyingMemberOpt) where TMember : Symbol
		{
			TMember result;
			if (underlyingMemberOpt == null)
			{
				result = null;
			}
			else
			{
				Symbol originalDefinition = underlyingMemberOpt.OriginalDefinition;
				if ((object)originalDefinition.ContainingType == _underlyingType.OriginalDefinition)
				{
					Symbol value = null;
					if (UnderlyingDefinitionToMemberMap.TryGetValue(originalDefinition, out value))
					{
						return (TMember)value;
					}
				}
				result = null;
			}
			return result;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return GetMembers().WhereAsArray((Symbol member, string name_) => CaseInsensitiveComparison.Equals(member.Name, name_), name);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingType.GetAttributes();
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			if ((object)obj == this)
			{
				return true;
			}
			if ((object)obj == null)
			{
				return false;
			}
			TupleTypeSymbol tupleTypeSymbol = obj as TupleTypeSymbol;
			if ((object)tupleTypeSymbol == null && (comparison & TypeCompareKind.IgnoreTupleNames) == 0)
			{
				return false;
			}
			if (!TupleUnderlyingType.Equals(TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(obj), comparison))
			{
				return false;
			}
			if ((comparison & TypeCompareKind.IgnoreTupleNames) == 0)
			{
				ImmutableArray<string> tupleElementNames = TupleElementNames;
				ImmutableArray<string> tupleElementNames2 = tupleTypeSymbol.TupleElementNames;
				if (tupleElementNames.IsDefault)
				{
					return tupleElementNames2.IsDefault;
				}
				if (tupleElementNames2.IsDefault)
				{
					return false;
				}
				int num = tupleElementNames.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!CaseInsensitiveComparison.Equals(tupleElementNames[i], tupleElementNames2[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return _underlyingType.GetHashCode();
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return _underlyingType.GetUseSiteInfo();
		}

		internal override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			return _underlyingType.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes);
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			return AttributeUsageInfo.Null;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<EventSymbol> GetEventsToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<MethodSymbol> GetMethodsToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<PropertySymbol> GetPropertiesToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<NamedTypeSymbol> GetInterfacesToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public static TypeSymbol TransformToTupleIfCompatible(TypeSymbol target)
		{
			if (target.IsTupleCompatible())
			{
				return Create((NamedTypeSymbol)target);
			}
			return target;
		}

		public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			NamedTypeSymbol tupleCompatibleType = (NamedTypeSymbol)TupleUnderlyingType.InternalSubstituteTypeParameters(substitution).Type;
			TupleTypeSymbol type = Create(_locations, tupleCompatibleType, _elementLocations, _providedElementNames, _errorPositions);
			return new TypeWithModifiers(type, default(ImmutableArray<CustomModifier>));
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return _underlyingType.MakeDeclaredBase(basesBeingResolved, diagnostics);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return _underlyingType.MakeDeclaredInterfaces(basesBeingResolved, diagnostics);
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return _underlyingType.MakeAcyclicBaseType(diagnostics);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return _underlyingType.MakeAcyclicInterfaces(diagnostics);
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			_underlyingType.GenerateDeclarationErrors(cancellationToken);
		}

		internal override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}

		internal static void ReportNamesMismatchesIfAny(TypeSymbol destination, BoundTupleLiteral literal, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<string> argumentNamesOpt = literal.ArgumentNamesOpt;
			if (argumentNamesOpt.IsDefault)
			{
				return;
			}
			ImmutableArray<bool> inferredNamesOpt = literal.InferredNamesOpt;
			bool isDefault = inferredNamesOpt.IsDefault;
			ImmutableArray<string> tupleElementNames = destination.TupleElementNames;
			int length = argumentNamesOpt.Length;
			bool isDefault2 = tupleElementNames.IsDefault;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				string text = argumentNamesOpt[i];
				bool flag = !isDefault && inferredNamesOpt[i];
				if (text != null && !flag && (isDefault2 || string.CompareOrdinal(tupleElementNames[i], text) != 0))
				{
					diagnostics.Add(ERRID.WRN_TupleLiteralNameMismatch, literal.Arguments[i].Syntax.Parent!.Location, text, destination);
				}
			}
		}
	}
}
