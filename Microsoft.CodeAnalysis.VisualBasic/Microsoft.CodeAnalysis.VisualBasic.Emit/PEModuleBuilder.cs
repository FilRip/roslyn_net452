using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal abstract class PEModuleBuilder : PEModuleBuilder<VisualBasicCompilation, SourceModuleSymbol, AssemblySymbol, TypeSymbol, NamedTypeSymbol, MethodSymbol, SyntaxNode, EmbeddedTypesManager, ModuleCompilationState>
	{
		private readonly ConcurrentDictionary<MethodSymbol, bool> _disableJITOptimization;

		private readonly string _metadataName;

		private ImmutableArray<ExportedType> _lazyExportedTypes;

		private readonly int _lazyNumberOfTypesFromOtherModules;

		private ImmutableArray<UsedNamespaceOrType> _lazyTranslatedImports;

		private string _lazyDefaultNamespace;

		protected readonly ConcurrentDictionary<Symbol, IModuleReference> m_AssemblyOrModuleSymbolToModuleRefMap;

		private readonly ConcurrentDictionary<Symbol, object> _genericInstanceMap;

		private readonly ConcurrentSet<TypeSymbol> _reportedErrorTypesMap;

		private readonly EmbeddedTypesManager _embeddedTypesManagerOpt;

		private ConcurrentSet<Symbol> _addedEmbeddedSymbols;

		internal abstract bool AllowOmissionOfConditionalCalls { get; }

		public override string Name => _metadataName;

		internal sealed override string ModuleName => _metadataName;

		internal sealed override AssemblySymbol CorLibrary => SourceModule.ContainingSourceAssembly.CorLibrary;

		public sealed override bool GenerateVisualBasicStylePdb => true;

		public override IEnumerable<string> LinkedAssembliesDebugInfo => from a in SourceModule.ReferencedAssemblySymbols
			where a.IsLinked
			select a.Name;

		public sealed override string DefaultNamespace
		{
			get
			{
				if (_lazyDefaultNamespace != null)
				{
					return _lazyDefaultNamespace;
				}
				NamespaceSymbol rootNamespace = SourceModule.RootNamespace;
				if (rootNamespace.IsGlobalNamespace)
				{
					return string.Empty;
				}
				_lazyDefaultNamespace = rootNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
				return _lazyDefaultNamespace;
			}
		}

		internal virtual bool IgnoreAccessibility => false;

		public override EmbeddedTypesManager EmbeddedTypesManagerOpt => _embeddedTypesManagerOpt;

		internal PEModuleBuilder(SourceModuleSymbol sourceModule, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources)
			: base(sourceModule.ContainingSourceAssembly.DeclaringCompilation, sourceModule, serializationProperties, manifestResources, outputKind, emitOptions, new ModuleCompilationState())
		{
			_disableJITOptimization = new ConcurrentDictionary<MethodSymbol, bool>(ReferenceEqualityComparer.Instance);
			m_AssemblyOrModuleSymbolToModuleRefMap = new ConcurrentDictionary<Symbol, IModuleReference>();
			_genericInstanceMap = new ConcurrentDictionary<Symbol, object>();
			_reportedErrorTypesMap = new ConcurrentSet<TypeSymbol>();
			_addedEmbeddedSymbols = null;
			string metadataName = sourceModule.MetadataName;
			_metadataName = ((EmbeddedOperators.CompareString(metadataName, "?", TextCompare: false) != 0) ? metadataName : (emitOptions.OutputNameOverride ?? metadataName));
			m_AssemblyOrModuleSymbolToModuleRefMap.Add(sourceModule, this);
			if (sourceModule.AnyReferencedAssembliesAreLinked)
			{
				_embeddedTypesManagerOpt = new EmbeddedTypesManager(this);
			}
		}

		public sealed override ImmutableArray<UsedNamespaceOrType> GetImports()
		{
			return _lazyTranslatedImports;
		}

		public void TranslateImports(DiagnosticBag diagnostics)
		{
			if (_lazyTranslatedImports.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref _lazyTranslatedImports, NamespaceScopeBuilder.BuildNamespaceScope(this, SourceModule.XmlNamespaces, SourceModule.AliasImports, SourceModule.MemberImports, diagnostics));
			}
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_23_GetAssemblyReferencesFromAddedModules))]
		protected sealed override IEnumerable<IAssemblyReference> GetAssemblyReferencesFromAddedModules(DiagnosticBag diagnostics)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_23_GetAssemblyReferencesFromAddedModules(-2)
			{
				_0024VB_0024Me = this,
				_0024P_diagnostics = diagnostics
			};
		}

		private void ValidateReferencedAssembly(AssemblySymbol assembly, AssemblyReference asmRef, DiagnosticBag diagnostics)
		{
			AssemblyIdentity identity = SourceModule.ContainingAssembly.Identity;
			AssemblyIdentity identity2 = asmRef.Identity;
			if (identity.IsStrongName && !identity2.IsStrongName && asmRef.Identity.ContentType != AssemblyContentType.WindowsRuntime)
			{
				DiagnosticBagExtensions.Add(diagnostics, ErrorFactory.ErrorInfo(ERRID.WRN_ReferencedAssemblyDoesNotHaveStrongName, assembly), NoLocation.Singleton);
			}
			if (OutputKind != OutputKind.NetModule && !string.IsNullOrEmpty(identity2.CultureName) && !string.Equals(identity2.CultureName, identity.CultureName, StringComparison.OrdinalIgnoreCase))
			{
				DiagnosticBagExtensions.Add(diagnostics, ErrorFactory.ErrorInfo(ERRID.WRN_RefCultureMismatch, assembly, identity2.CultureName), NoLocation.Singleton);
			}
			Machine machine = assembly.Machine;
			if ((object)assembly != assembly.CorLibrary && (machine != Machine.I386 || assembly.Bit32Required))
			{
				Machine machine2 = SourceModule.Machine;
				if ((machine2 != Machine.I386 || SourceModule.Bit32Required) && machine2 != machine)
				{
					DiagnosticBagExtensions.Add(diagnostics, ErrorFactory.ErrorInfo(ERRID.WRN_ConflictingMachineAssembly, assembly), NoLocation.Singleton);
				}
			}
			if (_embeddedTypesManagerOpt != null && _embeddedTypesManagerOpt.IsFrozen)
			{
				_embeddedTypesManagerOpt.ReportIndirectReferencesToLinkedAssemblies(assembly, diagnostics);
			}
		}

		internal sealed override ICustomAttribute SynthesizeAttribute(WellKnownMember attributeConstructor)
		{
			return Compilation.TrySynthesizeAttribute(attributeConstructor);
		}

		public sealed override IEnumerable<ICustomAttribute> GetSourceAssemblyAttributes(bool isRefAssembly)
		{
			return SourceModule.ContainingSourceAssembly.GetAssemblyCustomAttributesToEmit(CompilationState, isRefAssembly, OutputKind.IsNetModule());
		}

		public sealed override IEnumerable<SecurityAttribute> GetSourceAssemblySecurityAttributes()
		{
			return SourceModule.ContainingSourceAssembly.GetSecurityAttributes();
		}

		public sealed override IEnumerable<ICustomAttribute> GetSourceModuleAttributes()
		{
			return SourceModule.GetCustomAttributesToEmit(CompilationState);
		}

		public sealed override MultiDictionary<DebugSourceDocument, DefinitionWithLocation> GetSymbolToLocationMap()
		{
			MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result = new MultiDictionary<DebugSourceDocument, DefinitionWithLocation>();
			Stack<NamespaceOrTypeSymbol> stack = new Stack<NamespaceOrTypeSymbol>();
			stack.Push(SourceModule.GlobalNamespace);
			Location location = null;
			while (stack.Count > 0)
			{
				NamespaceOrTypeSymbol namespaceOrTypeSymbol = stack.Pop();
				switch (namespaceOrTypeSymbol.Kind)
				{
				case SymbolKind.Namespace:
				{
					location = GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol);
					if ((object)location == null)
					{
						break;
					}
					ImmutableArray<Symbol>.Enumerator enumerator2 = namespaceOrTypeSymbol.GetMembers().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current2 = enumerator2.Current;
						SymbolKind kind = current2.Kind;
						if ((uint)(kind - 11) <= 1u)
						{
							stack.Push((NamespaceOrTypeSymbol)current2);
							continue;
						}
						throw ExceptionUtilities.UnexpectedValue(current2.Kind);
					}
					break;
				}
				case SymbolKind.NamedType:
				{
					location = GetSmallestSourceLocationOrNull(namespaceOrTypeSymbol);
					if ((object)location == null)
					{
						break;
					}
					AddSymbolLocation(result, location, (IDefinition)namespaceOrTypeSymbol.GetCciAdapter());
					ImmutableArray<Symbol>.Enumerator enumerator = namespaceOrTypeSymbol.GetMembers().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						switch (current.Kind)
						{
						case SymbolKind.NamedType:
							stack.Push((NamespaceOrTypeSymbol)current);
							break;
						case SymbolKind.Method:
						{
							MethodSymbol method = (MethodSymbol)current;
							if (!SymbolExtensions.IsDefaultValueTypeConstructor(method) && !MethodSymbolExtensions.IsPartialWithoutImplementation(method))
							{
								AddSymbolLocation(result, current);
							}
							break;
						}
						case SymbolKind.Field:
						case SymbolKind.Property:
							AddSymbolLocation(result, current);
							break;
						case SymbolKind.Event:
						{
							AddSymbolLocation(result, current);
							FieldSymbol associatedField = ((EventSymbol)current).AssociatedField;
							if ((object)associatedField != null)
							{
								AddSymbolLocation(result, associatedField);
							}
							break;
						}
						default:
							throw ExceptionUtilities.UnexpectedValue(current.Kind);
						}
					}
					break;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(namespaceOrTypeSymbol.Kind);
				}
			}
			return result;
		}

		private void AddSymbolLocation(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result, Symbol symbol)
		{
			Location smallestSourceLocationOrNull = GetSmallestSourceLocationOrNull(symbol);
			if ((object)smallestSourceLocationOrNull != null)
			{
				AddSymbolLocation(result, smallestSourceLocationOrNull, (IDefinition)symbol.GetCciAdapter());
			}
		}

		private void AddSymbolLocation(MultiDictionary<DebugSourceDocument, DefinitionWithLocation> result, Location location, IDefinition definition)
		{
			FileLinePositionSpan lineSpan = location.GetLineSpan();
			DebugSourceDocument debugSourceDocument = DebugDocumentsBuilder.TryGetDebugDocument(lineSpan.Path, location.SourceTree!.FilePath);
			if (debugSourceDocument != null)
			{
				result.Add(debugSourceDocument, new DefinitionWithLocation(definition, lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character, lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character));
			}
		}

		private Location GetSmallestSourceLocationOrNull(Symbol symbol)
		{
			VisualBasicCompilation declaringCompilation = symbol.DeclaringCompilation;
			Location location = null;
			ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Location current = enumerator.Current;
				if (current.IsInSource && ((object)location == null || declaringCompilation.CompareSourceLocations(location, current) > 0))
				{
					location = current;
				}
			}
			return location;
		}

		internal virtual VariableSlotAllocator TryCreateVariableSlotAllocator(MethodSymbol method, MethodSymbol topLevelMethod, DiagnosticBag diagnostics)
		{
			return null;
		}

		internal virtual ImmutableArray<AnonymousTypeKey> GetPreviousAnonymousTypes()
		{
			return ImmutableArray<AnonymousTypeKey>.Empty;
		}

		internal virtual int GetNextAnonymousTypeIndex(bool fromDelegates)
		{
			return 0;
		}

		internal virtual bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol template, out string name, out int index)
		{
			name = null;
			index = -1;
			return false;
		}

		public sealed override IEnumerable<INamespaceTypeDefinition> GetAnonymousTypeDefinitions(EmitContext context)
		{
			if (context.MetadataOnly)
			{
				return SpecializedCollections.EmptyEnumerable<INamespaceTypeDefinition>();
			}
			return SourceModule.ContainingSourceAssembly.DeclaringCompilation.AnonymousTypeManager.AllCreatedTemplates;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_40_GetTopLevelSourceTypeDefinitions))]
		public override IEnumerable<INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_40_GetTopLevelSourceTypeDefinitions(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		public sealed override ImmutableArray<ExportedType> GetExportedTypes(DiagnosticBag diagnostics)
		{
			if (_lazyExportedTypes.IsDefault)
			{
				_lazyExportedTypes = CalculateExportedTypes();
				if (_lazyExportedTypes.Length > 0)
				{
					ReportExportedTypeNameCollisions(_lazyExportedTypes, diagnostics);
				}
			}
			return _lazyExportedTypes;
		}

		private ImmutableArray<ExportedType> CalculateExportedTypes()
		{
			ArrayBuilder<ExportedType> instance = ArrayBuilder<ExportedType>.GetInstance();
			SourceAssemblySymbol containingSourceAssembly = SourceModule.ContainingSourceAssembly;
			if (!OutputKind.IsNetModule())
			{
				ImmutableArray<ModuleSymbol> modules = containingSourceAssembly.Modules;
				int num = modules.Length - 1;
				for (int i = 1; i <= num; i++)
				{
					GetExportedTypes(modules[i].GlobalNamespace, -1, instance);
				}
			}
			GetForwardedTypes(containingSourceAssembly, instance);
			return instance.ToImmutableAndFree();
		}

		internal static HashSet<NamedTypeSymbol> GetForwardedTypes(SourceAssemblySymbol sourceAssembly, ArrayBuilder<ExportedType> builderOpt)
		{
			HashSet<NamedTypeSymbol> hashSet = new HashSet<NamedTypeSymbol>();
			GetForwardedTypes(hashSet, sourceAssembly.GetSourceDecodedWellKnownAttributeData(), builderOpt);
			if (!sourceAssembly.DeclaringCompilation.Options.OutputKind.IsNetModule())
			{
				GetForwardedTypes(hashSet, sourceAssembly.GetNetModuleDecodedWellKnownAttributeData(), builderOpt);
			}
			return hashSet;
		}

		private void ReportExportedTypeNameCollisions(ImmutableArray<ExportedType> exportedTypes, DiagnosticBag diagnostics)
		{
			SourceAssemblySymbol containingSourceAssembly = SourceModule.ContainingSourceAssembly;
			Dictionary<string, NamedTypeSymbol> dictionary = new Dictionary<string, NamedTypeSymbol>();
			ImmutableArray<ExportedType>.Enumerator enumerator = _lazyExportedTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ITypeReference type = enumerator.Current.Type;
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type.GetInternalSymbol();
				if ((object)namedTypeSymbol.ContainingType != null)
				{
					continue;
				}
				string text = MetadataHelpers.BuildQualifiedName(((INamespaceTypeReference)type).NamespaceName, MetadataWriter.GetMangledName((INamedTypeReference)type));
				if (ContainsTopLevelType(text))
				{
					if ((object)namedTypeSymbol.ContainingAssembly == containingSourceAssembly)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ExportedTypeConflictsWithDeclaration, NoLocation.Singleton, namedTypeSymbol, namedTypeSymbol.ContainingModule);
					}
					else
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ForwardedTypeConflictsWithDeclaration, NoLocation.Singleton, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol));
					}
					continue;
				}
				NamedTypeSymbol value = null;
				if (dictionary.TryGetValue(text, out value))
				{
					if ((object)namedTypeSymbol.ContainingAssembly == containingSourceAssembly)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ExportedTypesConflict, NoLocation.Singleton, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol.ContainingModule), CustomSymbolDisplayFormatter.DefaultErrorFormat(value), CustomSymbolDisplayFormatter.DefaultErrorFormat(value.ContainingModule));
					}
					else if ((object)value.ContainingAssembly == containingSourceAssembly)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ForwardedTypeConflictsWithExportedType, NoLocation.Singleton, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), namedTypeSymbol.ContainingAssembly, CustomSymbolDisplayFormatter.DefaultErrorFormat(value), CustomSymbolDisplayFormatter.DefaultErrorFormat(value.ContainingModule));
					}
					else
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ForwardedTypesConflict, NoLocation.Singleton, CustomSymbolDisplayFormatter.DefaultErrorFormat(namedTypeSymbol), namedTypeSymbol.ContainingAssembly, CustomSymbolDisplayFormatter.DefaultErrorFormat(value), value.ContainingAssembly);
					}
				}
				else
				{
					dictionary.Add(text, namedTypeSymbol);
				}
			}
		}

		private void GetExportedTypes(NamespaceOrTypeSymbol symbol, int parentIndex, ArrayBuilder<ExportedType> builder)
		{
			int parentIndex2;
			if (symbol.Kind == SymbolKind.NamedType)
			{
				if (symbol.DeclaredAccessibility != Accessibility.Public)
				{
					return;
				}
				parentIndex2 = builder.Count;
				builder.Add(new ExportedType(((NamedTypeSymbol)symbol).GetCciAdapter(), parentIndex, isForwarder: false));
			}
			else
			{
				parentIndex2 = -1;
			}
			ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is NamespaceOrTypeSymbol symbol2)
				{
					GetExportedTypes(symbol2, parentIndex2, builder);
				}
			}
		}

		private static void GetForwardedTypes(HashSet<NamedTypeSymbol> seenTopLevelTypes, CommonAssemblyWellKnownAttributeData<NamedTypeSymbol> wellKnownAttributeData, ArrayBuilder<ExportedType> builderOpt)
		{
			int? num = wellKnownAttributeData?.ForwardedTypes?.Count;
			if (!(num.HasValue ? new bool?(num.GetValueOrDefault() > 0) : null).GetValueOrDefault())
			{
				return;
			}
			ArrayBuilder<(NamedTypeSymbol, int)> instance = ArrayBuilder<(NamedTypeSymbol, int)>.GetInstance();
			IEnumerable<NamedTypeSymbol> enumerable = wellKnownAttributeData.ForwardedTypes;
			if (builderOpt != null)
			{
				enumerable = enumerable.OrderBy((NamedTypeSymbol t) => t.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.QualifiedNameArityFormat));
			}
			foreach (NamedTypeSymbol item in enumerable)
			{
				NamedTypeSymbol originalDefinition = item.OriginalDefinition;
				if (!seenTopLevelTypes.Add(originalDefinition) || builderOpt == null)
				{
					continue;
				}
				instance.Push((originalDefinition, -1));
				while (instance.Count > 0)
				{
					(NamedTypeSymbol, int) tuple = instance.Pop();
					if (tuple.Item1.DeclaredAccessibility != Accessibility.Private)
					{
						int count = builderOpt.Count;
						builderOpt.Add(new ExportedType(tuple.Item1.GetCciAdapter(), tuple.Item2, isForwarder: true));
						ImmutableArray<NamedTypeSymbol> typeMembers = tuple.Item1.GetTypeMembers();
						for (int i = typeMembers.Length - 1; i >= 0; i += -1)
						{
							instance.Push((typeMembers[i], count));
						}
					}
				}
			}
			instance.Free();
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_47_GetReferencedAssembliesUsedSoFar))]
		internal IEnumerable<AssemblySymbol> GetReferencedAssembliesUsedSoFar()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_47_GetReferencedAssembliesUsedSoFar(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal sealed override INamedTypeReference GetSpecialType(SpecialType specialType, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			return Translate(GetUntranslatedSpecialType(specialType, syntaxNodeOpt, diagnostics), syntaxNodeOpt, diagnostics, fromImplements: false, needDeclaration: true);
		}

		private NamedTypeSymbol GetUntranslatedSpecialType(SpecialType specialType, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol specialType2 = SourceModule.ContainingAssembly.GetSpecialType(specialType);
			UseSiteInfo<AssemblySymbol> useSiteInfoForSpecialType = Binder.GetUseSiteInfoForSpecialType(specialType2);
			if (useSiteInfoForSpecialType.DiagnosticInfo != null)
			{
				Binder.ReportDiagnostic(diagnostics, (syntaxNodeOpt != null) ? syntaxNodeOpt.GetLocation() : NoLocation.Singleton, useSiteInfoForSpecialType.DiagnosticInfo);
			}
			return specialType2;
		}

		public sealed override IMethodReference GetInitArrayHelper()
		{
			return ((MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__InitializeArrayArrayRuntimeFieldHandle))?.GetCciAdapter();
		}

		public sealed override bool IsPlatformType(ITypeReference typeRef, PlatformType platformType)
		{
			if (typeRef.GetInternalSymbol() is NamedTypeSymbol namedTypeSymbol)
			{
				if (platformType == PlatformType.SystemType)
				{
					return (object)namedTypeSymbol == Compilation.GetWellKnownType(WellKnownType.System_Type);
				}
				return (int)namedTypeSymbol.SpecialType == (sbyte)platformType;
			}
			return false;
		}

		protected sealed override IAssemblyReference GetCorLibraryReferenceToEmit(EmitContext context)
		{
			AssemblySymbol corLibrary = CorLibrary;
			if (!corLibrary.IsMissing && !corLibrary.IsLinked && (object)corLibrary != SourceModule.ContainingAssembly)
			{
				return Translate(corLibrary, context.Diagnostics);
			}
			return null;
		}

		internal sealed override IEnumerable<INestedTypeDefinition> GetSynthesizedNestedTypes(NamedTypeSymbol container)
		{
			return container.GetSynthesizedNestedTypes();
		}

		public void SetDisableJITOptimization(MethodSymbol methodSymbol)
		{
			_disableJITOptimization.TryAdd(methodSymbol, value: true);
		}

		public bool JITOptimizationIsDisabled(MethodSymbol methodSymbol)
		{
			return _disableJITOptimization.ContainsKey(methodSymbol);
		}

		protected sealed override IMethodDefinition CreatePrivateImplementationDetailsStaticConstructor(PrivateImplementationDetails details, SyntaxNode syntaxOpt, DiagnosticBag diagnostics)
		{
			return new SynthesizedPrivateImplementationDetailsSharedConstructor(SourceModule, details, GetUntranslatedSpecialType(SpecialType.System_Void, syntaxOpt, diagnostics)).GetCciAdapter();
		}

		public override IEnumerable<INamespaceTypeDefinition> GetAdditionalTopLevelTypeDefinitions(EmitContext context)
		{
			return GetAdditionalTopLevelTypes();
		}

		public override IEnumerable<INamespaceTypeDefinition> GetEmbeddedTypeDefinitions(EmitContext context)
		{
			return GetEmbeddedTypes(context.Diagnostics);
		}

		private void ProcessReferencedSymbol(Symbol symbol)
		{
			if (symbol.EmbeddedSymbolKind != 0)
			{
				if (_addedEmbeddedSymbols == null)
				{
					Interlocked.CompareExchange(ref _addedEmbeddedSymbols, new ConcurrentSet<Symbol>(ReferenceEqualityComparer.Instance), null);
				}
				SourceModule.ContainingSourceAssembly.DeclaringCompilation.EmbeddedSymbolManager.MarkSymbolAsReferenced(symbol.OriginalDefinition, _addedEmbeddedSymbols);
			}
		}

		internal sealed override IAssemblyReference Translate(AssemblySymbol assembly, DiagnosticBag diagnostics)
		{
			if ((object)SourceModule.ContainingAssembly == assembly)
			{
				return (IAssemblyReference)this;
			}
			IModuleReference value = null;
			if (m_AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(assembly, out value))
			{
				return (IAssemblyReference)value;
			}
			AssemblyReference assemblyReference = new AssemblyReference(assembly);
			AssemblyReference assemblyReference2 = (AssemblyReference)m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(assembly, assemblyReference);
			if (assemblyReference2 == assemblyReference)
			{
				ValidateReferencedAssembly(assembly, assemblyReference2, diagnostics);
			}
			m_AssemblyOrModuleSymbolToModuleRefMap.TryAdd(assembly.Modules[0], assemblyReference2);
			return assemblyReference2;
		}

		internal IModuleReference Translate(ModuleSymbol module, DiagnosticBag diagnostics)
		{
			if ((object)SourceModule == module)
			{
				return this;
			}
			IModuleReference value = null;
			if (m_AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(module, out value))
			{
				return value;
			}
			value = TranslateModule(module, diagnostics);
			return m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(module, value);
		}

		protected virtual IModuleReference TranslateModule(ModuleSymbol module, DiagnosticBag diagnostics)
		{
			AssemblySymbol containingAssembly = module.ContainingAssembly;
			if ((object)containingAssembly != null && (object)containingAssembly.Modules[0] == module)
			{
				IModuleReference moduleReference = new AssemblyReference(containingAssembly);
				IModuleReference orAdd = m_AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(containingAssembly, moduleReference);
				if (orAdd == moduleReference)
				{
					ValidateReferencedAssembly(containingAssembly, (AssemblyReference)moduleReference, diagnostics);
				}
				else
				{
					moduleReference = orAdd;
				}
				return moduleReference;
			}
			return new ModuleReference(this, module);
		}

		internal INamedTypeReference Translate(NamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool fromImplements = false, bool needDeclaration = false)
		{
			if (namedTypeSymbol.IsAnonymousType)
			{
				namedTypeSymbol = AnonymousTypeManager.TranslateAnonymousTypeSymbol(namedTypeSymbol);
			}
			else if (namedTypeSymbol.IsTupleType)
			{
				namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType;
				CheckTupleUnderlyingType(namedTypeSymbol, syntaxNodeOpt, diagnostics);
			}
			if (namedTypeSymbol.OriginalDefinition.Kind == SymbolKind.ErrorType)
			{
				ErrorTypeSymbol errorTypeSymbol = (ErrorTypeSymbol)namedTypeSymbol.OriginalDefinition;
				DiagnosticInfo diagnosticInfo = errorTypeSymbol.GetUseSiteInfo().DiagnosticInfo ?? errorTypeSymbol.ErrorInfo;
				if (diagnosticInfo == null && namedTypeSymbol.Kind == SymbolKind.ErrorType)
				{
					errorTypeSymbol = (ErrorTypeSymbol)namedTypeSymbol;
					diagnosticInfo = errorTypeSymbol.GetUseSiteInfo().DiagnosticInfo ?? errorTypeSymbol.ErrorInfo;
				}
				if (_reportedErrorTypesMap.Add(errorTypeSymbol))
				{
					diagnostics.Add(new VBDiagnostic(diagnosticInfo ?? ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, string.Empty), (syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.GetLocation()));
				}
				return ErrorType.Singleton;
			}
			ProcessReferencedSymbol(namedTypeSymbol);
			if ((object)namedTypeSymbol != namedTypeSymbol.OriginalDefinition)
			{
				if (!namedTypeSymbol.IsUnboundGenericType)
				{
					return (INamedTypeReference)GetCciAdapter(namedTypeSymbol);
				}
				namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
			}
			else if (!needDeclaration)
			{
				object value = null;
				NamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
				if (namedTypeSymbol.Arity > 0)
				{
					if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out value))
					{
						return (INamedTypeReference)value;
					}
					INamedTypeReference value2 = (((object)containingType == null) ? new GenericNamespaceTypeInstanceReference(namedTypeSymbol) : ((!NamedTypeSymbolExtensions.IsOrInGenericType(containingType)) ? ((INamedTypeReference)new GenericNestedTypeInstanceReference(namedTypeSymbol)) : ((INamedTypeReference)new SpecializedGenericNestedTypeInstanceReference(namedTypeSymbol))));
					return (INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, value2);
				}
				if (NamedTypeSymbolExtensions.IsOrInGenericType(containingType))
				{
					if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out value))
					{
						return (INamedTypeReference)value;
					}
					INamedTypeReference value2 = new SpecializedNestedTypeReference(namedTypeSymbol);
					return (INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, value2);
				}
			}
			return _embeddedTypesManagerOpt?.EmbedTypeIfNeedTo(namedTypeSymbol, fromImplements, syntaxNodeOpt, diagnostics) ?? namedTypeSymbol.GetCciAdapter();
		}

		private object GetCciAdapter(Symbol symbol)
		{
			return _genericInstanceMap.GetOrAdd(symbol, (Symbol s) => s.GetCciAdapter());
		}

		private void CheckTupleUnderlyingType(NamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			if (((object)baseTypeNoUseSiteDiagnostics != null && baseTypeNoUseSiteDiagnostics.SpecialType == SpecialType.System_ValueType) || !_reportedErrorTypesMap.Add(namedTypeSymbol))
			{
				return;
			}
			Location location = ((syntaxNodeOpt == null) ? NoLocation.Singleton : syntaxNodeOpt.GetLocation());
			if ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				DiagnosticInfo diagnosticInfo = baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					DiagnosticBagExtensions.Add(diagnostics, diagnosticInfo, location);
					return;
				}
			}
			diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_PredefinedValueTupleTypeMustBeStruct, namedTypeSymbol.MetadataName), location));
		}

		internal IGenericParameterReference Translate(TypeParameterSymbol param)
		{
			return param.GetCciAdapter();
		}

		internal sealed override ITypeReference Translate(TypeSymbol typeSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			switch (typeSymbol.Kind)
			{
			case SymbolKind.ArrayType:
				return Translate((ArrayTypeSymbol)typeSymbol);
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
				return Translate((NamedTypeSymbol)typeSymbol, syntaxNodeOpt, diagnostics);
			case SymbolKind.TypeParameter:
				return Translate((TypeParameterSymbol)typeSymbol);
			default:
				throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind);
			}
		}

		internal IFieldReference Translate(FieldSymbol fieldSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool needDeclaration = false)
		{
			ProcessReferencedSymbol(fieldSymbol);
			if ((object)fieldSymbol != fieldSymbol.OriginalDefinition)
			{
				return (IFieldReference)GetCciAdapter(fieldSymbol);
			}
			if (!needDeclaration && NamedTypeSymbolExtensions.IsOrInGenericType(fieldSymbol.ContainingType))
			{
				object value = null;
				if (_genericInstanceMap.TryGetValue(fieldSymbol, out value))
				{
					return (IFieldReference)value;
				}
				IFieldReference value2 = new SpecializedFieldReference(fieldSymbol);
				return (IFieldReference)_genericInstanceMap.GetOrAdd(fieldSymbol, value2);
			}
			if (_embeddedTypesManagerOpt != null)
			{
				return _embeddedTypesManagerOpt.EmbedFieldIfNeedTo(fieldSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics);
			}
			return fieldSymbol.GetCciAdapter();
		}

		public static TypeMemberVisibility MemberVisibility(Symbol symbol)
		{
			switch (symbol.DeclaredAccessibility)
			{
			case Accessibility.Public:
				return TypeMemberVisibility.Public;
			case Accessibility.Private:
				if (symbol.ContainingType.TypeKind == TypeKind.Submission)
				{
					return TypeMemberVisibility.Public;
				}
				return TypeMemberVisibility.Private;
			case Accessibility.Internal:
				if (symbol.ContainingAssembly.IsInteractive)
				{
					return TypeMemberVisibility.Public;
				}
				return TypeMemberVisibility.Assembly;
			case Accessibility.Protected:
				if (symbol.ContainingType.TypeKind == TypeKind.Submission)
				{
					return TypeMemberVisibility.Public;
				}
				return TypeMemberVisibility.Family;
			case Accessibility.ProtectedAndInternal:
				return TypeMemberVisibility.FamilyAndAssembly;
			case Accessibility.ProtectedOrInternal:
				if (symbol.ContainingAssembly.IsInteractive)
				{
					return TypeMemberVisibility.Public;
				}
				return TypeMemberVisibility.FamilyOrAssembly;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
			}
		}

		internal override IMethodReference Translate(MethodSymbol symbol, DiagnosticBag diagnostics, bool needDeclaration)
		{
			return Translate(symbol, null, diagnostics, needDeclaration);
		}

		internal IMethodReference Translate(MethodSymbol methodSymbol, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool needDeclaration = false)
		{
			NamedTypeSymbol namedTypeSymbol = methodSymbol.ContainingType;
			if (namedTypeSymbol.IsAnonymousType)
			{
				methodSymbol = AnonymousTypeManager.TranslateAnonymousTypeMethodSymbol(methodSymbol);
			}
			else if (methodSymbol.IsTupleMethod)
			{
				namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType;
				methodSymbol = methodSymbol.TupleUnderlyingMethod;
			}
			ProcessReferencedSymbol(methodSymbol);
			if ((object)methodSymbol.OriginalDefinition != methodSymbol)
			{
				return (IMethodReference)GetCciAdapter(methodSymbol);
			}
			if (!needDeclaration)
			{
				bool isGenericMethod = methodSymbol.IsGenericMethod;
				bool flag = NamedTypeSymbolExtensions.IsOrInGenericType(namedTypeSymbol);
				if (isGenericMethod || flag)
				{
					object value = null;
					if (_genericInstanceMap.TryGetValue(methodSymbol, out value))
					{
						return (IMethodReference)value;
					}
					IMethodReference value2 = ((!isGenericMethod) ? new SpecializedMethodReference(methodSymbol) : ((!flag) ? ((IMethodReference)new GenericMethodInstanceReference(methodSymbol)) : ((IMethodReference)new SpecializedGenericMethodInstanceReference(methodSymbol))));
					return (IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, value2);
				}
			}
			if (_embeddedTypesManagerOpt != null)
			{
				return _embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics);
			}
			return methodSymbol.GetCciAdapter();
		}

		internal IMethodReference TranslateOverriddenMethodReference(MethodSymbol methodSymbol, VisualBasicSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
		{
			if (NamedTypeSymbolExtensions.IsOrInGenericType(methodSymbol.ContainingType))
			{
				if (methodSymbol.IsDefinition)
				{
					object value = null;
					if (_genericInstanceMap.TryGetValue(methodSymbol, out value))
					{
						return (IMethodReference)value;
					}
					IMethodReference value2 = new SpecializedMethodReference(methodSymbol);
					return (IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, value2);
				}
				return new SpecializedMethodReference(methodSymbol);
			}
			if (_embeddedTypesManagerOpt != null)
			{
				return _embeddedTypesManagerOpt.EmbedMethodIfNeedTo(methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics);
			}
			return methodSymbol.GetCciAdapter();
		}

		internal ImmutableArray<IParameterTypeInformation> Translate(ImmutableArray<ParameterSymbol> @params)
		{
			if (!@params.Any() || !MustBeWrapped(@params.First()))
			{
				return StaticCast<IParameterTypeInformation>.From(@params);
			}
			return TranslateAll(@params);
		}

		private static bool MustBeWrapped(ParameterSymbol param)
		{
			if (param.IsDefinition && ContainerIsGeneric(param.ContainingSymbol))
			{
				return true;
			}
			return false;
		}

		private ImmutableArray<IParameterTypeInformation> TranslateAll(ImmutableArray<ParameterSymbol> @params)
		{
			ArrayBuilder<IParameterTypeInformation> instance = ArrayBuilder<IParameterTypeInformation>.GetInstance();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = @params.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				instance.Add(CreateParameterTypeInformationWrapper(current));
			}
			return instance.ToImmutableAndFree();
		}

		private IParameterTypeInformation CreateParameterTypeInformationWrapper(ParameterSymbol param)
		{
			object value = null;
			if (_genericInstanceMap.TryGetValue(param, out value))
			{
				return (IParameterTypeInformation)value;
			}
			IParameterTypeInformation value2 = new ParameterTypeInformation(param);
			return (IParameterTypeInformation)_genericInstanceMap.GetOrAdd(param, value2);
		}

		private static bool ContainerIsGeneric(Symbol container)
		{
			if (container.Kind != SymbolKind.Method || !((MethodSymbol)container).IsGenericMethod)
			{
				return container.ContainingType.IsGenericType;
			}
			return true;
		}

		internal IArrayTypeReference Translate(ArrayTypeSymbol symbol)
		{
			return (IArrayTypeReference)GetCciAdapter(symbol);
		}
	}
}
