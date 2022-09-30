using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
{
	internal sealed class EmbeddedTypesManager : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>
	{
		private readonly ConcurrentDictionary<AssemblySymbol, string> _assemblyGuidMap;

		private readonly ConcurrentDictionary<Symbol, bool> _reportedSymbolsMap;

		private NamedTypeSymbol _lazySystemStringType;

		private readonly MethodSymbol[] _lazyWellKnownTypeMethods;

		public EmbeddedTypesManager(PEModuleBuilder moduleBeingBuilt)
			: base(moduleBeingBuilt)
		{
			_assemblyGuidMap = new ConcurrentDictionary<AssemblySymbol, string>(ReferenceEqualityComparer.Instance);
			_reportedSymbolsMap = new ConcurrentDictionary<Symbol, bool>(ReferenceEqualityComparer.Instance);
			_lazySystemStringType = ErrorTypeSymbol.UnknownResultType;
			_lazyWellKnownTypeMethods = new MethodSymbol[418];
			int num = 0;
			do
			{
				_lazyWellKnownTypeMethods[num] = ErrorMethodSymbol.UnknownMethod;
				num++;
			}
			while (num <= 417);
		}

		public NamedTypeSymbol GetSystemStringType(SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			if ((object)_lazySystemStringType == ErrorTypeSymbol.UnknownResultType)
			{
				NamedTypeSymbol namedTypeSymbol = ModuleBeingBuilt.Compilation.GetSpecialType(SpecialType.System_String);
				UseSiteInfo<AssemblySymbol> useSiteInfo = namedTypeSymbol.GetUseSiteInfo();
				if (TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
				{
					namedTypeSymbol = null;
				}
				if (TypeSymbol.Equals(Interlocked.CompareExchange(ref _lazySystemStringType, namedTypeSymbol, ErrorTypeSymbol.UnknownResultType), ErrorTypeSymbol.UnknownResultType, TypeCompareKind.ConsiderEverything) && useSiteInfo.DiagnosticInfo != null)
				{
					ReportDiagnostic(diagnostics, syntaxNodeOpt, useSiteInfo.DiagnosticInfo);
				}
			}
			return _lazySystemStringType;
		}

		public MethodSymbol GetWellKnownMethod(WellKnownMember method, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			return LazyGetWellKnownTypeMethod(ref _lazyWellKnownTypeMethods[(int)method], method, syntaxNodeOpt, diagnostics);
		}

		private MethodSymbol LazyGetWellKnownTypeMethod(ref MethodSymbol lazyMethod, WellKnownMember method, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			if ((object)lazyMethod == ErrorMethodSymbol.UnknownMethod)
			{
				UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
				MethodSymbol value = (MethodSymbol)Binder.GetWellKnownTypeMember(ModuleBeingBuilt.Compilation, method, out useSiteInfo);
				if (Interlocked.CompareExchange(ref lazyMethod, value, ErrorMethodSymbol.UnknownMethod) == ErrorMethodSymbol.UnknownMethod && useSiteInfo.DiagnosticInfo != null)
				{
					ReportDiagnostic(diagnostics, syntaxNodeOpt, useSiteInfo.DiagnosticInfo);
				}
			}
			return lazyMethod;
		}

		internal override int GetTargetAttributeSignatureIndex(Symbol underlyingSymbol, VisualBasicAttributeData attrData, AttributeDescription description)
		{
			return attrData.GetTargetAttributeSignatureIndex(underlyingSymbol.AdaptedSymbol, description);
		}

		internal override VisualBasicAttributeData CreateSynthesizedAttribute(WellKnownMember constructor, VisualBasicAttributeData attrData, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			MethodSymbol wellKnownMethod = GetWellKnownMethod(constructor, syntaxNodeOpt, diagnostics);
			if ((object)wellKnownMethod == null)
			{
				return null;
			}
			return constructor switch
			{
				WellKnownMember.System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor => new SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(attrData.CommonConstructorArguments[0], attrData.CommonConstructorArguments[0]), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty), 
				WellKnownMember.System_Runtime_InteropServices_CoClassAttribute__ctor => new SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(new TypedConstant(wellKnownMethod.Parameters[0].Type, TypedConstantKind.Type, wellKnownMethod.ContainingAssembly.GetSpecialType(SpecialType.System_Object))), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty), 
				_ => new SynthesizedAttributeData(wellKnownMethod, attrData.CommonConstructorArguments, attrData.CommonNamedArguments), 
			};
		}

		internal string GetAssemblyGuidString(AssemblySymbol assembly)
		{
			string value = null;
			if (_assemblyGuidMap.TryGetValue(assembly, out value))
			{
				return value;
			}
			assembly.GetGuidString(ref value);
			return _assemblyGuidMap.GetOrAdd(assembly, value);
		}

		protected override void OnGetTypesCompleted(ImmutableArray<EmbeddedType> types, DiagnosticBag diagnostics)
		{
			ImmutableArray<EmbeddedType>.Enumerator enumerator = types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EmbeddedType current = enumerator.Current;
				_assemblyGuidMap.TryAdd(current.UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly, null);
			}
			foreach (AssemblySymbol item in ModuleBeingBuilt.GetReferencedAssembliesUsedSoFar())
			{
				ReportIndirectReferencesToLinkedAssemblies(item, diagnostics);
			}
		}

		protected override void ReportNameCollisionBetweenEmbeddedTypes(EmbeddedType typeA, EmbeddedType typeB, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol adaptedNamedTypeSymbol = typeA.UnderlyingNamedType.AdaptedNamedTypeSymbol;
			NamedTypeSymbol adaptedNamedTypeSymbol2 = typeB.UnderlyingNamedType.AdaptedNamedTypeSymbol;
			ReportDiagnostic(diagnostics, ERRID.ERR_DuplicateLocalTypes3, null, adaptedNamedTypeSymbol, adaptedNamedTypeSymbol.ContainingAssembly, adaptedNamedTypeSymbol2.ContainingAssembly);
		}

		protected override void ReportNameCollisionWithAlreadyDeclaredType(EmbeddedType type, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol adaptedNamedTypeSymbol = type.UnderlyingNamedType.AdaptedNamedTypeSymbol;
			ReportDiagnostic(diagnostics, ERRID.ERR_LocalTypeNameClash2, null, adaptedNamedTypeSymbol, adaptedNamedTypeSymbol.ContainingAssembly);
		}

		internal override void ReportIndirectReferencesToLinkedAssemblies(AssemblySymbol assembly, DiagnosticBag diagnostics)
		{
			ImmutableArray<ModuleSymbol>.Enumerator enumerator = assembly.Modules.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ImmutableArray<AssemblySymbol>.Enumerator enumerator2 = enumerator.Current.GetReferencedAssemblySymbols().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					AssemblySymbol current = enumerator2.Current;
					if (!current.IsMissing && current.IsLinked && _assemblyGuidMap.ContainsKey(current))
					{
						ReportDiagnostic(diagnostics, ERRID.WRN_IndirectRefToLinkedAssembly2, null, current, assembly);
					}
				}
			}
		}

		internal static bool IsValidEmbeddableType(NamedTypeSymbol type, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, EmbeddedTypesManager typeManagerOpt = null)
		{
			if (type.SpecialType != 0 || TypeSymbolExtensions.IsErrorType(type) || !type.ContainingAssembly.IsLinked)
			{
				return false;
			}
			ERRID eRRID = ERRID.ERR_None;
			switch (type.TypeKind)
			{
			case TypeKind.Interface:
			{
				ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembersUnordered().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind != SymbolKind.NamedType)
					{
						if (!current.IsMustOverride)
						{
							eRRID = ERRID.ERR_DefaultInterfaceImplementationInNoPIAType;
						}
						else if (current.IsNotOverridable)
						{
							eRRID = ERRID.ERR_ReAbstractionInNoPIAType;
						}
					}
				}
				if (eRRID != 0)
				{
					break;
				}
				goto case TypeKind.Delegate;
			}
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Struct:
				if (type.IsTupleType)
				{
					type = type.TupleUnderlyingType;
				}
				if ((object)type.ContainingType != null)
				{
					eRRID = ERRID.ERR_NestedInteropType;
				}
				else if (type.IsGenericType)
				{
					eRRID = ERRID.ERR_CannotEmbedInterfaceWithGeneric;
				}
				break;
			default:
				eRRID = ERRID.ERR_CannotLinkClassWithNoPIA1;
				break;
			}
			if (eRRID != 0)
			{
				ReportNotEmbeddableSymbol(eRRID, type, syntaxNodeOpt, diagnostics, typeManagerOpt);
				return false;
			}
			return true;
		}

		private void VerifyNotFrozen()
		{
			if (IsFrozen)
			{
				throw ExceptionUtilities.UnexpectedValue(IsFrozen);
			}
		}

		private static void ReportNotEmbeddableSymbol(ERRID id, Symbol symbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, EmbeddedTypesManager typeManagerOpt)
		{
			if (typeManagerOpt == null || typeManagerOpt._reportedSymbolsMap.TryAdd(symbol.OriginalDefinition, value: true))
			{
				ReportDiagnostic(diagnostics, id, syntaxNodeOpt, symbol.OriginalDefinition);
			}
		}

		internal static void ReportDiagnostic(DiagnosticBag diagnostics, ERRID id, SyntaxNode syntaxNodeOpt, params object[] args)
		{
			ReportDiagnostic(diagnostics, syntaxNodeOpt, ErrorFactory.ErrorInfo(id, args));
		}

		private static void ReportDiagnostic(DiagnosticBag diagnostics, SyntaxNode syntaxNodeOpt, DiagnosticInfo info)
		{
			diagnostics.Add(new VBDiagnostic(info, (syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.GetLocation()));
		}

		internal INamedTypeReference EmbedTypeIfNeedTo(NamedTypeSymbol namedType, bool fromImplements, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			if (IsValidEmbeddableType(namedType, syntaxNodeOpt, diagnostics, this))
			{
				return EmbedType(namedType, fromImplements, syntaxNodeOpt, diagnostics);
			}
			return null;
		}

		private EmbeddedType EmbedType(NamedTypeSymbol namedType, bool fromImplements, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol cciAdapter = namedType.GetCciAdapter();
			EmbeddedType embeddedType = new EmbeddedType(this, cciAdapter);
			EmbeddedType orAdd = EmbeddedTypesMap.GetOrAdd(cciAdapter, embeddedType);
			bool isInterface = namedType.IsInterface;
			if (isInterface && fromImplements)
			{
				orAdd.EmbedAllMembersOfImplementedInterface(syntaxNodeOpt, diagnostics);
			}
			if (embeddedType != orAdd)
			{
				return orAdd;
			}
			VerifyNotFrozen();
			new TypeReferenceIndexer(new EmitContext(ModuleBeingBuilt, syntaxNodeOpt, diagnostics, metadataOnly: false, includePrivateMembers: true)).VisitTypeDefinitionNoMembers(embeddedType);
			if (!isInterface)
			{
				if (namedType.TypeKind != TypeKind.Struct)
				{
					_ = namedType.TypeKind;
					_ = 5;
				}
				foreach (FieldSymbol item in namedType.GetFieldsToEmit())
				{
					EmbedField(embeddedType, item.GetCciAdapter(), syntaxNodeOpt, diagnostics);
				}
				foreach (MethodSymbol item2 in namedType.GetMethodsToEmit())
				{
					EmbedMethod(embeddedType, item2.GetCciAdapter(), syntaxNodeOpt, diagnostics);
				}
			}
			return embeddedType;
		}

		internal override EmbeddedField EmbedField(EmbeddedType type, FieldSymbol field, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			EmbeddedField embeddedField = new EmbeddedField(type, field);
			EmbeddedField orAdd = EmbeddedFieldsMap.GetOrAdd(field, embeddedField);
			if (embeddedField != orAdd)
			{
				return orAdd;
			}
			VerifyNotFrozen();
			EmbedReferences(embeddedField, syntaxNodeOpt, diagnostics);
			TypeKind typeKind = field.AdaptedFieldSymbol.ContainingType.TypeKind;
			if (typeKind == TypeKind.Interface || typeKind == TypeKind.Delegate || (typeKind == TypeKind.Struct && (field.AdaptedFieldSymbol.IsShared || field.AdaptedFieldSymbol.DeclaredAccessibility != Accessibility.Public)))
			{
				ReportNotEmbeddableSymbol(ERRID.ERR_InvalidStructMemberNoPIA1, type.UnderlyingNamedType.AdaptedNamedTypeSymbol, syntaxNodeOpt, diagnostics, this);
			}
			return embeddedField;
		}

		internal override EmbeddedMethod EmbedMethod(EmbeddedType type, MethodSymbol method, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			EmbeddedMethod embeddedMethod = new EmbeddedMethod(type, method);
			EmbeddedMethod orAdd = EmbeddedMethodsMap.GetOrAdd(method, embeddedMethod);
			if (embeddedMethod != orAdd)
			{
				return orAdd;
			}
			VerifyNotFrozen();
			EmbedReferences(embeddedMethod, syntaxNodeOpt, diagnostics);
			TypeKind typeKind = type.UnderlyingNamedType.AdaptedNamedTypeSymbol.TypeKind;
			if (typeKind == TypeKind.Enum || typeKind == TypeKind.Struct)
			{
				ReportNotEmbeddableSymbol(ERRID.ERR_InvalidStructMemberNoPIA1, type.UnderlyingNamedType.AdaptedNamedTypeSymbol, syntaxNodeOpt, diagnostics, this);
			}
			else if (embeddedMethod.HasBody())
			{
				ReportDiagnostic(diagnostics, ERRID.ERR_InteropMethodWithBody1, syntaxNodeOpt, method.AdaptedMethodSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
			}
			Symbol associatedSymbol = method.AdaptedMethodSymbol.AssociatedSymbol;
			if ((object)associatedSymbol != null)
			{
				switch (associatedSymbol.Kind)
				{
				case SymbolKind.Property:
					EmbedProperty(type, ((PropertySymbol)associatedSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics);
					break;
				case SymbolKind.Event:
					EmbedEvent(type, ((EventSymbol)associatedSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding: false);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(associatedSymbol.Kind);
				}
			}
			return embeddedMethod;
		}

		internal override EmbeddedProperty EmbedProperty(EmbeddedType type, PropertySymbol property, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			MethodSymbol getMethod = property.AdaptedPropertySymbol.GetMethod;
			MethodSymbol setMethod = property.AdaptedPropertySymbol.SetMethod;
			EmbeddedMethod getter = (((object)getMethod != null) ? EmbedMethod(type, getMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics) : null);
			EmbeddedMethod setter = (((object)setMethod != null) ? EmbedMethod(type, setMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics) : null);
			EmbeddedProperty embeddedProperty = new EmbeddedProperty(property, getter, setter);
			EmbeddedProperty orAdd = EmbeddedPropertiesMap.GetOrAdd(property, embeddedProperty);
			if (embeddedProperty != orAdd)
			{
				return orAdd;
			}
			VerifyNotFrozen();
			EmbedReferences(embeddedProperty, syntaxNodeOpt, diagnostics);
			return embeddedProperty;
		}

		internal override EmbeddedEvent EmbedEvent(EmbeddedType type, EventSymbol @event, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding)
		{
			MethodSymbol addMethod = @event.AdaptedEventSymbol.AddMethod;
			MethodSymbol removeMethod = @event.AdaptedEventSymbol.RemoveMethod;
			MethodSymbol raiseMethod = @event.AdaptedEventSymbol.RaiseMethod;
			EmbeddedMethod adder = (((object)addMethod != null) ? EmbedMethod(type, addMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics) : null);
			EmbeddedMethod remover = (((object)removeMethod != null) ? EmbedMethod(type, removeMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics) : null);
			EmbeddedMethod caller = (((object)raiseMethod != null) ? EmbedMethod(type, raiseMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics) : null);
			EmbeddedEvent embeddedEvent = new EmbeddedEvent(@event, adder, remover, caller);
			EmbeddedEvent orAdd = EmbeddedEventsMap.GetOrAdd(@event, embeddedEvent);
			if (embeddedEvent != orAdd)
			{
				if (isUsedForComAwareEventBinding)
				{
					orAdd.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding);
				}
				return orAdd;
			}
			VerifyNotFrozen();
			EmbedReferences(embeddedEvent, syntaxNodeOpt, diagnostics);
			embeddedEvent.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding);
			return embeddedEvent;
		}

		protected override EmbeddedType GetEmbeddedTypeForMember(Symbol member, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol containingType = member.AdaptedSymbol.ContainingType;
			if (IsValidEmbeddableType(containingType, syntaxNodeOpt, diagnostics, this))
			{
				return EmbedType(containingType, fromImplements: false, syntaxNodeOpt, diagnostics);
			}
			return null;
		}

		internal static ImmutableArray<EmbeddedParameter> EmbedParameters(CommonEmbeddedMember containingPropertyOrMethod, ImmutableArray<ParameterSymbol> underlyingParameters)
		{
			return underlyingParameters.SelectAsArray((ParameterSymbol parameter, CommonEmbeddedMember container) => new EmbeddedParameter(container, parameter.GetCciAdapter()), containingPropertyOrMethod);
		}

		protected override VisualBasicAttributeData CreateCompilerGeneratedAttribute()
		{
			return ModuleBeingBuilt.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor);
		}
	}
}
