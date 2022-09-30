using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class VisualBasicSymbolMatcher : SymbolMatcher
	{
		private abstract class MatchDefs
		{
			private readonly EmitContext _sourceContext;

			private readonly ConcurrentDictionary<IDefinition, IDefinition> _matches;

			private IReadOnlyDictionary<string, INamespaceTypeDefinition> _lazyTopLevelTypes;

			public MatchDefs(EmitContext sourceContext)
			{
				_sourceContext = sourceContext;
				_matches = new ConcurrentDictionary<IDefinition, IDefinition>(ReferenceEqualityComparer.Instance);
			}

			public IDefinition VisitDef(IDefinition def)
			{
				return _matches.GetOrAdd(def, VisitDefInternal);
			}

			private IDefinition VisitDefInternal(IDefinition def)
			{
				if (def is ITypeDefinition typeDefinition)
				{
					INamespaceTypeDefinition namespaceTypeDefinition = typeDefinition.AsNamespaceTypeDefinition(_sourceContext);
					if (namespaceTypeDefinition != null)
					{
						return VisitNamespaceType(namespaceTypeDefinition);
					}
					INestedTypeDefinition nestedTypeDefinition = typeDefinition.AsNestedTypeDefinition(_sourceContext);
					ITypeDefinition typeDefinition2 = (ITypeDefinition)VisitDef(nestedTypeDefinition.ContainingTypeDefinition);
					if (typeDefinition2 == null)
					{
						return null;
					}
					return VisitTypeMembers(typeDefinition2, nestedTypeDefinition, GetNestedTypes, (INestedTypeDefinition a, INestedTypeDefinition b) => s_nameComparer.Equals(a.Name, b.Name));
				}
				if (def is ITypeDefinitionMember typeDefinitionMember)
				{
					ITypeDefinition typeDefinition3 = (ITypeDefinition)VisitDef(typeDefinitionMember.ContainingTypeDefinition);
					if (typeDefinition3 == null)
					{
						return null;
					}
					if (def is IFieldDefinition member)
					{
						return VisitTypeMembers(typeDefinition3, member, GetFields, (IFieldDefinition a, IFieldDefinition b) => s_nameComparer.Equals(a.Name, b.Name));
					}
				}
				throw ExceptionUtilities.UnexpectedValue(def);
			}

			protected abstract IEnumerable<INamespaceTypeDefinition> GetTopLevelTypes();

			protected abstract IEnumerable<INestedTypeDefinition> GetNestedTypes(ITypeDefinition def);

			protected abstract IEnumerable<IFieldDefinition> GetFields(ITypeDefinition def);

			private INamespaceTypeDefinition VisitNamespaceType(INamespaceTypeDefinition def)
			{
				if (!string.IsNullOrEmpty(def.NamespaceName))
				{
					return null;
				}
				INamespaceTypeDefinition value = null;
				GetTopLevelTypesByName().TryGetValue(def.Name, out value);
				return value;
			}

			private IReadOnlyDictionary<string, INamespaceTypeDefinition> GetTopLevelTypesByName()
			{
				if (_lazyTopLevelTypes == null)
				{
					Dictionary<string, INamespaceTypeDefinition> dictionary = new Dictionary<string, INamespaceTypeDefinition>(s_nameComparer);
					foreach (INamespaceTypeDefinition topLevelType in GetTopLevelTypes())
					{
						if (string.IsNullOrEmpty(topLevelType.NamespaceName))
						{
							dictionary.Add(topLevelType.Name, topLevelType);
						}
					}
					Interlocked.CompareExchange(ref _lazyTopLevelTypes, dictionary, null);
				}
				return _lazyTopLevelTypes;
			}

			private static T VisitTypeMembers<T>(ITypeDefinition otherContainer, T member, Func<ITypeDefinition, IEnumerable<T>> getMembers, Func<T, T, bool> predicate) where T : class, ITypeDefinitionMember
			{
				return getMembers(otherContainer).FirstOrDefault((T otherMember) => predicate(member, otherMember));
			}
		}

		private sealed class MatchDefsToMetadata : MatchDefs
		{
			private readonly PEAssemblySymbol _otherAssembly;

			public MatchDefsToMetadata(EmitContext sourceContext, PEAssemblySymbol otherAssembly)
				: base(sourceContext)
			{
				_otherAssembly = otherAssembly;
			}

			protected override IEnumerable<INamespaceTypeDefinition> GetTopLevelTypes()
			{
				ArrayBuilder<INamespaceTypeDefinition> instance = ArrayBuilder<INamespaceTypeDefinition>.GetInstance();
				GetTopLevelTypes(instance, _otherAssembly.GlobalNamespace);
				return instance.ToArrayAndFree();
			}

			protected override IEnumerable<INestedTypeDefinition> GetNestedTypes(ITypeDefinition def)
			{
				return ((PENamedTypeSymbol)def).GetTypeMembers().Cast<INestedTypeDefinition>();
			}

			protected override IEnumerable<IFieldDefinition> GetFields(ITypeDefinition def)
			{
				return ((PENamedTypeSymbol)def).GetFieldsToEmit().Cast<IFieldDefinition>();
			}

			private static void GetTopLevelTypes(ArrayBuilder<INamespaceTypeDefinition> builder, NamespaceSymbol @namespace)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = @namespace.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Namespace)
					{
						GetTopLevelTypes(builder, (NamespaceSymbol)current);
					}
					else
					{
						builder.Add((INamespaceTypeDefinition)current.GetCciAdapter());
					}
				}
			}
		}

		private sealed class MatchDefsToSource : MatchDefs
		{
			private readonly EmitContext _otherContext;

			public MatchDefsToSource(EmitContext sourceContext, EmitContext otherContext)
				: base(sourceContext)
			{
				_otherContext = otherContext;
			}

			protected override IEnumerable<INamespaceTypeDefinition> GetTopLevelTypes()
			{
				return _otherContext.Module.GetTopLevelTypeDefinitions(_otherContext);
			}

			protected override IEnumerable<INestedTypeDefinition> GetNestedTypes(ITypeDefinition def)
			{
				return def.GetNestedTypes(_otherContext);
			}

			protected override IEnumerable<IFieldDefinition> GetFields(ITypeDefinition def)
			{
				return def.GetFields(_otherContext);
			}
		}

		private sealed class MatchSymbols : VisualBasicSymbolVisitor<Symbol>
		{
			private class SymbolComparer
			{
				private readonly MatchSymbols _matcher;

				private readonly DeepTranslator _deepTranslatorOpt;

				public SymbolComparer(MatchSymbols matcher, DeepTranslator deepTranslatorOpt)
				{
					_matcher = matcher;
					_deepTranslatorOpt = deepTranslatorOpt;
				}

				public bool Equals(TypeSymbol source, TypeSymbol other)
				{
					TypeSymbol typeSymbol = (TypeSymbol)_matcher.Visit(source);
					TypeSymbol typeSymbol2 = ((_deepTranslatorOpt != null) ? ((TypeSymbol)_deepTranslatorOpt.Visit(other)) : other);
					if ((object)typeSymbol != null && (object)typeSymbol2 != null)
					{
						return TypeSymbolExtensions.IsSameType(typeSymbol, typeSymbol2, TypeCompareKind.IgnoreTupleNames);
					}
					return false;
				}
			}

			private readonly IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> _anonymousTypeMap;

			private readonly SymbolComparer _comparer;

			private readonly ConcurrentDictionary<Symbol, Symbol> _matches;

			private readonly SourceAssemblySymbol _sourceAssembly;

			private readonly AssemblySymbol _otherAssembly;

			private readonly ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> _otherSynthesizedMembersOpt;

			private readonly ConcurrentDictionary<ISymbolInternal, IReadOnlyDictionary<string, ImmutableArray<ISymbolInternal>>> _otherMembers;

			public MatchSymbols(IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMap, SourceAssemblySymbol sourceAssembly, AssemblySymbol otherAssembly, ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> otherSynthesizedMembersOpt, DeepTranslator deepTranslatorOpt)
			{
				_anonymousTypeMap = anonymousTypeMap;
				_sourceAssembly = sourceAssembly;
				_otherAssembly = otherAssembly;
				_otherSynthesizedMembersOpt = otherSynthesizedMembersOpt;
				_comparer = new SymbolComparer(this, deepTranslatorOpt);
				_matches = new ConcurrentDictionary<Symbol, Symbol>(ReferenceEqualityComparer.Instance);
				_otherMembers = new ConcurrentDictionary<ISymbolInternal, IReadOnlyDictionary<string, ImmutableArray<ISymbolInternal>>>(ReferenceEqualityComparer.Instance);
			}

			internal bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol type, out string name, out int index)
			{
				AnonymousTypeValue otherType = default(AnonymousTypeValue);
				if (TryFindAnonymousType(type, out otherType))
				{
					name = otherType.Name;
					index = otherType.UniqueIndex;
					return true;
				}
				name = null;
				index = -1;
				return false;
			}

			public override Symbol DefaultVisit(Symbol symbol)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override Symbol Visit(Symbol symbol)
			{
				return _matches.GetOrAdd(symbol, base.Visit);
			}

			public override Symbol VisitArrayType(ArrayTypeSymbol symbol)
			{
				TypeSymbol typeSymbol = (TypeSymbol)Visit(symbol.ElementType);
				if ((object)typeSymbol == null)
				{
					return null;
				}
				ImmutableArray<CustomModifier> customModifiers = VisitCustomModifiers(symbol.CustomModifiers);
				if (symbol.IsSZArray)
				{
					return ArrayTypeSymbol.CreateSZArray(typeSymbol, customModifiers, _otherAssembly);
				}
				return ArrayTypeSymbol.CreateMDArray(typeSymbol, customModifiers, symbol.Rank, symbol.Sizes, symbol.LowerBounds, _otherAssembly);
			}

			public override Symbol VisitEvent(EventSymbol symbol)
			{
				return VisitNamedTypeMember(symbol, AreEventsEqual);
			}

			public override Symbol VisitField(FieldSymbol symbol)
			{
				return VisitNamedTypeMember(symbol, AreFieldsEqual);
			}

			public override Symbol VisitMethod(MethodSymbol symbol)
			{
				return VisitNamedTypeMember(symbol, AreMethodsEqual);
			}

			public override Symbol VisitModule(ModuleSymbol module)
			{
				AssemblySymbol assemblySymbol = (AssemblySymbol)Visit(module.ContainingAssembly);
				if ((object)assemblySymbol == null)
				{
					return null;
				}
				if (module.Ordinal == 0)
				{
					return assemblySymbol.Modules[0];
				}
				int num = assemblySymbol.Modules.Length - 1;
				for (int i = 1; i <= num; i++)
				{
					ModuleSymbol moduleSymbol = assemblySymbol.Modules[i];
					if (StringComparer.Ordinal.Equals(moduleSymbol.Name, module.Name))
					{
						return moduleSymbol;
					}
				}
				return null;
			}

			public override Symbol VisitAssembly(AssemblySymbol assembly)
			{
				if (assembly.IsLinked)
				{
					return assembly;
				}
				if (IdentityEqualIgnoringVersionWildcard(assembly, _sourceAssembly))
				{
					return _otherAssembly;
				}
				ImmutableArray<AssemblySymbol>.Enumerator enumerator = _otherAssembly.Modules[0].ReferencedAssemblySymbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AssemblySymbol current = enumerator.Current;
					if (IdentityEqualIgnoringVersionWildcard(assembly, current))
					{
						return current;
					}
				}
				return null;
			}

			private static bool IdentityEqualIgnoringVersionWildcard(AssemblySymbol left, AssemblySymbol right)
			{
				AssemblyIdentity identity = left.Identity;
				AssemblyIdentity identity2 = right.Identity;
				if (AssemblyIdentityComparer.SimpleNameComparer.Equals(identity.Name, identity2.Name) && (left.AssemblyVersionPattern ?? identity.Version).Equals(right.AssemblyVersionPattern ?? identity2.Version))
				{
					return AssemblyIdentity.EqualIgnoringNameAndVersion(identity, identity2);
				}
				return false;
			}

			public override Symbol VisitNamespace(NamespaceSymbol @namespace)
			{
				Symbol symbol = Visit(@namespace.ContainingSymbol);
				return symbol.Kind switch
				{
					SymbolKind.NetModule => ((ModuleSymbol)symbol).GlobalNamespace, 
					SymbolKind.Namespace => FindMatchingMember(symbol, @namespace, AreNamespacesEqual), 
					_ => throw ExceptionUtilities.UnexpectedValue(symbol.Kind), 
				};
			}

			public override Symbol VisitNamedType(NamedTypeSymbol type)
			{
				NamedTypeSymbol originalDefinition = type.OriginalDefinition;
				if ((object)originalDefinition != type)
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)Visit(originalDefinition);
					if ((object)namedTypeSymbol == null)
					{
						return null;
					}
					ImmutableArray<TypeParameterSymbol> allTypeParameters = TypeSymbolExtensions.GetAllTypeParameters(namedTypeSymbol);
					bool flag = false;
					ImmutableArray<TypeWithModifiers> args = TypeSymbolExtensions.GetAllTypeArgumentsWithModifiers(type).SelectAsArray(delegate(TypeWithModifiers t, MatchSymbols v)
					{
						TypeSymbol typeSymbol = (TypeSymbol)v.Visit(t.Type);
						if ((object)typeSymbol == null)
						{
							flag = true;
							typeSymbol = t.Type;
						}
						return new TypeWithModifiers(typeSymbol, v.VisitCustomModifiers(t.CustomModifiers));
					}, this);
					if (flag)
					{
						return null;
					}
					TypeSubstitution substitution = TypeSubstitution.Create(namedTypeSymbol, allTypeParameters, args);
					return namedTypeSymbol.Construct(substitution);
				}
				if (type.IsTupleType)
				{
					NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)Visit(type.TupleUnderlyingType);
					if ((object)namedTypeSymbol2 == null || !namedTypeSymbol2.IsTupleOrCompatibleWithTupleOfCardinality(type.TupleElementTypes.Length))
					{
						return null;
					}
					return namedTypeSymbol2;
				}
				Symbol symbol = Visit(type.ContainingSymbol);
				if ((object)symbol == null)
				{
					return null;
				}
				switch (symbol.Kind)
				{
				case SymbolKind.Namespace:
					if (type is AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol type2)
					{
						AnonymousTypeValue otherType = default(AnonymousTypeValue);
						TryFindAnonymousType(type2, out otherType);
						return (NamedTypeSymbol)(otherType.Type?.GetInternalSymbol());
					}
					if (type.IsAnonymousType)
					{
						return Visit(AnonymousTypeManager.TranslateAnonymousTypeSymbol(type));
					}
					return FindMatchingMember(symbol, type, AreNamedTypesEqual);
				case SymbolKind.NamedType:
					return FindMatchingMember(symbol, type, AreNamedTypesEqual);
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
				}
			}

			public override Symbol VisitParameter(ParameterSymbol parameter)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override Symbol VisitProperty(PropertySymbol symbol)
			{
				return VisitNamedTypeMember(symbol, ArePropertiesEqual);
			}

			public override Symbol VisitTypeParameter(TypeParameterSymbol symbol)
			{
				if (symbol is IndexedTypeParameterSymbol result)
				{
					return result;
				}
				Symbol symbol2 = Visit(symbol.ContainingSymbol);
				ImmutableArray<TypeParameterSymbol> typeParameters;
				switch (symbol2.Kind)
				{
				case SymbolKind.ErrorType:
				case SymbolKind.NamedType:
					typeParameters = ((NamedTypeSymbol)symbol2).TypeParameters;
					break;
				case SymbolKind.Method:
					typeParameters = ((MethodSymbol)symbol2).TypeParameters;
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol2.Kind);
				}
				return typeParameters[symbol.Ordinal];
			}

			private ImmutableArray<CustomModifier> VisitCustomModifiers(ImmutableArray<CustomModifier> modifiers)
			{
				return modifiers.SelectAsArray(VisitCustomModifier);
			}

			private CustomModifier VisitCustomModifier(CustomModifier modifier)
			{
				NamedTypeSymbol modifier2 = (NamedTypeSymbol)Visit((Symbol)modifier.Modifier);
				if (!modifier.IsOptional)
				{
					return VisualBasicCustomModifier.CreateRequired(modifier2);
				}
				return VisualBasicCustomModifier.CreateOptional(modifier2);
			}

			internal bool TryFindAnonymousType(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol type, out AnonymousTypeValue otherType)
			{
				return _anonymousTypeMap.TryGetValue(type.GetAnonymousTypeKey(), out otherType);
			}

			private Symbol VisitNamedTypeMember<T>(T member, Func<T, T, bool> predicate) where T : Symbol
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)Visit(member.ContainingType);
				if ((object)namedTypeSymbol == null)
				{
					return null;
				}
				return FindMatchingMember(namedTypeSymbol, member, predicate);
			}

			private T FindMatchingMember<T>(ISymbolInternal otherTypeOrNamespace, T sourceMember, Func<T, T, bool> predicate) where T : Symbol
			{
				IReadOnlyDictionary<string, ImmutableArray<ISymbolInternal>> orAdd = _otherMembers.GetOrAdd(otherTypeOrNamespace, GetAllEmittedMembers);
				ImmutableArray<ISymbolInternal> value = default(ImmutableArray<ISymbolInternal>);
				if (orAdd.TryGetValue(sourceMember.Name, out value))
				{
					ImmutableArray<ISymbolInternal>.Enumerator enumerator = value.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (enumerator.Current is T val && predicate(sourceMember, val))
						{
							return val;
						}
					}
				}
				return null;
			}

			private bool AreArrayTypesEqual(ArrayTypeSymbol type, ArrayTypeSymbol other)
			{
				if (type.HasSameShapeAs(other))
				{
					return AreTypesEqual(type.ElementType, other.ElementType);
				}
				return false;
			}

			private bool AreEventsEqual(EventSymbol @event, EventSymbol other)
			{
				return _comparer.Equals(@event.Type, other.Type);
			}

			private bool AreFieldsEqual(FieldSymbol field, FieldSymbol other)
			{
				return _comparer.Equals(field.Type, other.Type);
			}

			private bool AreMethodsEqual(MethodSymbol method, MethodSymbol other)
			{
				method = SubstituteTypeParameters(method);
				other = SubstituteTypeParameters(other);
				if (_comparer.Equals(method.ReturnType, other.ReturnType) && method.Parameters.SequenceEqual(other.Parameters, AreParametersEqual))
				{
					return method.TypeParameters.SequenceEqual(other.TypeParameters, AreTypesEqual);
				}
				return false;
			}

			private static MethodSymbol SubstituteTypeParameters(MethodSymbol method)
			{
				int length = method.TypeParameters.Length;
				if (length == 0)
				{
					return method;
				}
				return method.Construct(IndexedTypeParameterSymbol.Take(length).Cast<TypeParameterSymbol, TypeSymbol>());
			}

			private bool AreNamedTypesEqual(NamedTypeSymbol type, NamedTypeSymbol other)
			{
				return type.TypeArgumentsNoUseSiteDiagnostics.SequenceEqual(other.TypeArgumentsNoUseSiteDiagnostics, AreTypesEqual);
			}

			private bool AreNamespacesEqual(NamespaceSymbol @namespace, NamespaceSymbol other)
			{
				return true;
			}

			private bool AreParametersEqual(ParameterSymbol parameter, ParameterSymbol other)
			{
				if (s_nameComparer.Equals(parameter.Name, other.Name) && parameter.IsByRef == other.IsByRef)
				{
					return _comparer.Equals(parameter.Type, other.Type);
				}
				return false;
			}

			private bool ArePropertiesEqual(PropertySymbol property, PropertySymbol other)
			{
				if (_comparer.Equals(property.Type, other.Type))
				{
					return property.Parameters.SequenceEqual(other.Parameters, AreParametersEqual);
				}
				return false;
			}

			private static bool AreTypeParametersEqual(TypeParameterSymbol type, TypeParameterSymbol other)
			{
				return true;
			}

			private bool AreTypesEqual(TypeSymbol type, TypeSymbol other)
			{
				if (type.Kind != other.Kind)
				{
					return false;
				}
				switch (type.Kind)
				{
				case SymbolKind.ArrayType:
					return AreArrayTypesEqual((ArrayTypeSymbol)type, (ArrayTypeSymbol)other);
				case SymbolKind.ErrorType:
				case SymbolKind.NamedType:
					return AreNamedTypesEqual((NamedTypeSymbol)type, (NamedTypeSymbol)other);
				case SymbolKind.TypeParameter:
					return AreTypeParametersEqual((TypeParameterSymbol)type, (TypeParameterSymbol)other);
				default:
					throw ExceptionUtilities.UnexpectedValue(type.Kind);
				}
			}

			private IReadOnlyDictionary<string, ImmutableArray<ISymbolInternal>> GetAllEmittedMembers(ISymbolInternal symbol)
			{
				ArrayBuilder<ISymbolInternal> instance = ArrayBuilder<ISymbolInternal>.GetInstance();
				if (symbol.Kind == SymbolKind.NamedType)
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
					instance.AddRange(namedTypeSymbol.GetEventsToEmit());
					instance.AddRange(namedTypeSymbol.GetFieldsToEmit());
					instance.AddRange(namedTypeSymbol.GetMethodsToEmit());
					instance.AddRange(namedTypeSymbol.GetTypeMembers());
					instance.AddRange(namedTypeSymbol.GetPropertiesToEmit());
				}
				else
				{
					instance.AddRange(((NamespaceSymbol)symbol).GetMembers());
				}
				ImmutableArray<ISymbolInternal> value = default(ImmutableArray<ISymbolInternal>);
				if (_otherSynthesizedMembersOpt != null && _otherSynthesizedMembersOpt.TryGetValue(symbol, out value))
				{
					instance.AddRange(value);
				}
				Dictionary<string, ImmutableArray<ISymbolInternal>> result = instance.ToDictionary((ISymbolInternal s) => s.Name, s_nameComparer);
				instance.Free();
				return result;
			}
		}

		internal sealed class DeepTranslator : VisualBasicSymbolVisitor<Symbol>
		{
			private readonly ConcurrentDictionary<Symbol, Symbol> _matches;

			private readonly NamedTypeSymbol _systemObject;

			public DeepTranslator(NamedTypeSymbol systemObject)
			{
				_matches = new ConcurrentDictionary<Symbol, Symbol>(ReferenceEqualityComparer.Instance);
				_systemObject = systemObject;
			}

			public override Symbol DefaultVisit(Symbol symbol)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override Symbol Visit(Symbol symbol)
			{
				return _matches.GetOrAdd(symbol, base.Visit);
			}

			public override Symbol VisitArrayType(ArrayTypeSymbol symbol)
			{
				TypeSymbol elementType = (TypeSymbol)Visit(symbol.ElementType);
				ImmutableArray<CustomModifier> customModifiers = VisitCustomModifiers(symbol.CustomModifiers);
				if (symbol.IsSZArray)
				{
					return ArrayTypeSymbol.CreateSZArray(elementType, customModifiers, symbol.BaseTypeNoUseSiteDiagnostics.ContainingAssembly);
				}
				return ArrayTypeSymbol.CreateMDArray(elementType, customModifiers, symbol.Rank, symbol.Sizes, symbol.LowerBounds, symbol.BaseTypeNoUseSiteDiagnostics.ContainingAssembly);
			}

			public override Symbol VisitNamedType(NamedTypeSymbol type)
			{
				if (type.IsTupleType)
				{
					type = type.TupleUnderlyingType;
				}
				NamedTypeSymbol originalDefinition = type.OriginalDefinition;
				if ((object)originalDefinition != type)
				{
					ImmutableArray<TypeWithModifiers> args = TypeSymbolExtensions.GetAllTypeArgumentsWithModifiers(type).SelectAsArray((TypeWithModifiers t, DeepTranslator v) => new TypeWithModifiers((TypeSymbol)v.Visit(t.Type), v.VisitCustomModifiers(t.CustomModifiers)), this);
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)Visit(originalDefinition);
					TypeSubstitution substitution = TypeSubstitution.Create(namedTypeSymbol, TypeSymbolExtensions.GetAllTypeParameters(namedTypeSymbol), args);
					return namedTypeSymbol.Construct(substitution);
				}
				if (type.IsAnonymousType)
				{
					return Visit(AnonymousTypeManager.TranslateAnonymousTypeSymbol(type));
				}
				return type;
			}

			public override Symbol VisitTypeParameter(TypeParameterSymbol symbol)
			{
				return symbol;
			}

			private ImmutableArray<CustomModifier> VisitCustomModifiers(ImmutableArray<CustomModifier> modifiers)
			{
				return modifiers.SelectAsArray(VisitCustomModifier);
			}

			private CustomModifier VisitCustomModifier(CustomModifier modifier)
			{
				NamedTypeSymbol modifier2 = (NamedTypeSymbol)Visit((Symbol)modifier.Modifier);
				if (!modifier.IsOptional)
				{
					return VisualBasicCustomModifier.CreateRequired(modifier2);
				}
				return VisualBasicCustomModifier.CreateOptional(modifier2);
			}
		}

		private static readonly StringComparer s_nameComparer = CaseInsensitiveComparison.Comparer;

		private readonly MatchDefs _defs;

		private readonly MatchSymbols _symbols;

		public VisualBasicSymbolMatcher(IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMap, SourceAssemblySymbol sourceAssembly, EmitContext sourceContext, SourceAssemblySymbol otherAssembly, EmitContext otherContext, ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> otherSynthesizedMembersOpt)
		{
			_defs = new MatchDefsToSource(sourceContext, otherContext);
			_symbols = new MatchSymbols(anonymousTypeMap, sourceAssembly, otherAssembly, otherSynthesizedMembersOpt, new DeepTranslator(otherAssembly.GetSpecialType(SpecialType.System_Object)));
		}

		public VisualBasicSymbolMatcher(IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMap, SourceAssemblySymbol sourceAssembly, EmitContext sourceContext, PEAssemblySymbol otherAssembly)
		{
			_defs = new MatchDefsToMetadata(sourceContext, otherAssembly);
			_symbols = new MatchSymbols(anonymousTypeMap, sourceAssembly, otherAssembly, null, null);
		}

		public override IDefinition MapDefinition(IDefinition definition)
		{
			if (definition.GetInternalSymbol() is Symbol symbol)
			{
				return (IDefinition)(_symbols.Visit(symbol)?.GetCciAdapter());
			}
			return _defs.VisitDef(definition);
		}

		public override INamespace MapNamespace(INamespace @namespace)
		{
			return (INamespace)(_symbols.Visit((NamespaceSymbol)(@namespace?.GetInternalSymbol()))?.GetCciAdapter());
		}

		public override ITypeReference MapReference(ITypeReference reference)
		{
			if (reference.GetInternalSymbol() is Symbol symbol)
			{
				return (ITypeReference)(_symbols.Visit(symbol)?.GetCciAdapter());
			}
			return null;
		}

		internal bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol template, out string name, out int index)
		{
			return _symbols.TryGetAnonymousTypeName(template, out name, out index);
		}
	}
}
