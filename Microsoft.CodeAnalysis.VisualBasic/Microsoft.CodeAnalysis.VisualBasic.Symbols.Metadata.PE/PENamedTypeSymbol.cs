using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal class PENamedTypeSymbol : InstanceTypeSymbol
	{
		private class DeclarationOrderSymbolComparer : IComparer<ISymbol>
		{
			public static readonly DeclarationOrderSymbolComparer Instance = new DeclarationOrderSymbolComparer();

			private DeclarationOrderSymbolComparer()
			{
			}

			public int Compare(ISymbol x, ISymbol y)
			{
				if (x == y)
				{
					return 0;
				}
				int num = x.Kind.ToSortOrder() - y.Kind.ToSortOrder();
				if (num != 0)
				{
					return num;
				}
				switch (x.Kind)
				{
				case SymbolKind.Field:
					return HandleComparer.Default.Compare(((PEFieldSymbol)x).Handle, ((PEFieldSymbol)y).Handle);
				case SymbolKind.Method:
					if (SymbolExtensions.IsDefaultValueTypeConstructor((MethodSymbol)x))
					{
						return -1;
					}
					if (SymbolExtensions.IsDefaultValueTypeConstructor((MethodSymbol)y))
					{
						return 1;
					}
					return HandleComparer.Default.Compare(((PEMethodSymbol)x).Handle, ((PEMethodSymbol)y).Handle);
				case SymbolKind.Property:
					return HandleComparer.Default.Compare(((PEPropertySymbol)x).Handle, ((PEPropertySymbol)y).Handle);
				case SymbolKind.Event:
					return HandleComparer.Default.Compare(((PEEventSymbol)x).Handle, ((PEEventSymbol)y).Handle);
				case SymbolKind.NamedType:
					return HandleComparer.Default.Compare(((PENamedTypeSymbol)x).Handle, ((PENamedTypeSymbol)y).Handle);
				default:
					throw ExceptionUtilities.UnexpectedValue(x.Kind);
				}
			}

			int IComparer<ISymbol>.Compare(ISymbol x, ISymbol y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(x, y);
			}
		}

		private readonly NamespaceOrTypeSymbol _container;

		private readonly TypeDefinitionHandle _handle;

		private readonly GenericParameterHandleCollection _genericParameterHandles;

		private readonly string _name;

		private readonly TypeAttributes _flags;

		private readonly ushort _arity;

		private readonly bool _mangleName;

		private Dictionary<string, ImmutableArray<PENamedTypeSymbol>> _lazyNestedTypes;

		private ICollection<string> _lazyMemberNames;

		private Dictionary<string, ImmutableArray<Symbol>> _lazyMembers;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private NamedTypeSymbol _lazyEnumUnderlyingType;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ImmutableArray<string> _lazyConditionalAttributeSymbols;

		private AttributeUsageInfo _lazyAttributeUsageInfo;

		private TypeSymbol _lazyCoClassType;

		private int _lazyTypeKind;

		private Tuple<CultureInfo, string> _lazyDocComment;

		private string _lazyDefaultPropertyName;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private byte _lazyMightContainExtensionMethods;

		private int _lazyHasCodeAnalysisEmbeddedAttribute;

		private int _lazyHasVisualBasicEmbeddedAttribute;

		private ObsoleteAttributeData _lazyObsoleteAttributeData;

		private ThreeState _lazyIsExtensibleInterface;

		internal PEModuleSymbol ContainingPEModule
		{
			get
			{
				Symbol symbol = _container;
				while (symbol.Kind != SymbolKind.Namespace)
				{
					symbol = symbol.ContainingSymbol;
				}
				return ((PENamespaceSymbol)symbol).ContainingPEModule;
			}
		}

		public override ModuleSymbol ContainingModule => ContainingPEModule;

		public override int Arity => _arity;

		internal override bool MangleName => _mangleName;

		internal override TypeLayout Layout => ContainingPEModule.Module.GetTypeLayout(_handle);

		internal override CharSet MarshallingCharSet
		{
			get
			{
				CharSet charSet = _flags.ToCharSet();
				if (charSet == (CharSet)0)
				{
					return CharSet.Ansi;
				}
				return charSet;
			}
		}

		public override bool IsSerializable => (_flags & TypeAttributes.Serializable) != 0;

		internal override bool HasSpecialName => (_flags & TypeAttributes.SpecialName) != 0;

		internal int MetadataArity
		{
			get
			{
				GenericParameterHandleCollection genericParameterHandles = _genericParameterHandles;
				return genericParameterHandles.Count;
			}
		}

		internal TypeDefinitionHandle Handle => _handle;

		public override Symbol ContainingSymbol => _container;

		public override NamedTypeSymbol ContainingType => _container as NamedTypeSymbol;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				Accessibility result = Accessibility.Private;
				switch (_flags & TypeAttributes.VisibilityMask)
				{
				case TypeAttributes.NestedAssembly:
					result = Accessibility.Internal;
					break;
				case TypeAttributes.VisibilityMask:
					result = Accessibility.ProtectedOrInternal;
					break;
				case TypeAttributes.NestedFamANDAssem:
					result = Accessibility.ProtectedAndInternal;
					break;
				case TypeAttributes.NestedPrivate:
					result = Accessibility.Private;
					break;
				case TypeAttributes.Public:
				case TypeAttributes.NestedPublic:
					result = Accessibility.Public;
					break;
				case TypeAttributes.NestedFamily:
					result = Accessibility.Protected;
					break;
				case TypeAttributes.NotPublic:
					result = Accessibility.Internal;
					break;
				}
				return result;
			}
		}

		public override NamedTypeSymbol EnumUnderlyingType
		{
			get
			{
				if ((object)_lazyEnumUnderlyingType == null && TypeKind == TypeKind.Enum)
				{
					NamedTypeSymbol namedTypeSymbol = null;
					ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (current.IsShared || current.Kind != SymbolKind.Field)
						{
							continue;
						}
						TypeSymbol type = ((FieldSymbol)current).Type;
						if (type.SpecialType.IsClrInteger())
						{
							if ((object)namedTypeSymbol != null)
							{
								namedTypeSymbol = new UnsupportedMetadataTypeSymbol();
								break;
							}
							namedTypeSymbol = (NamedTypeSymbol)type;
						}
					}
					Interlocked.CompareExchange(ref _lazyEnumUnderlyingType, namedTypeSymbol ?? new UnsupportedMetadataTypeSymbol(), null);
				}
				return _lazyEnumUnderlyingType;
			}
		}

		public override IEnumerable<string> MemberNames
		{
			get
			{
				EnsureNonTypeMemberNamesAreLoaded();
				return _lazyMemberNames;
			}
		}

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(ContainingPEModule.MetadataLocation);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override string Name => _name;

		internal TypeAttributes TypeDefFlags => _flags;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				EnsureTypeParametersAreLoaded();
				return _lazyTypeParameters;
			}
		}

		public override bool IsMustInherit
		{
			get
			{
				if ((_flags & TypeAttributes.Abstract) != 0)
				{
					return (_flags & TypeAttributes.Sealed) == 0;
				}
				return false;
			}
		}

		internal override bool IsMetadataAbstract => (_flags & TypeAttributes.Abstract) != 0;

		public override bool IsNotInheritable => (_flags & TypeAttributes.Sealed) != 0;

		internal override bool IsMetadataSealed => (_flags & TypeAttributes.Sealed) != 0;

		internal override bool IsWindowsRuntimeImport => (_flags & TypeAttributes.WindowsRuntime) != 0;

		internal override bool ShouldAddWinRTMembers => IsWindowsRuntimeImport;

		public sealed override bool MightContainExtensionMethods
		{
			get
			{
				if (_lazyMightContainExtensionMethods == 0)
				{
					bool flag = false;
					if (_container.Kind == SymbolKind.Namespace && _arity == 0)
					{
						PEModuleSymbol containingPEModule = ContainingPEModule;
						if (containingPEModule.MightContainExtensionMethods && containingPEModule.Module.HasExtensionAttribute(_handle, ignoreCase: true))
						{
							flag = true;
						}
					}
					if (flag)
					{
						_lazyMightContainExtensionMethods = 2;
					}
					else
					{
						_lazyMightContainExtensionMethods = 1;
					}
				}
				return _lazyMightContainExtensionMethods == 2;
			}
		}

		internal override bool HasCodeAnalysisEmbeddedAttribute
		{
			get
			{
				if (_lazyHasCodeAnalysisEmbeddedAttribute == 0)
				{
					Interlocked.CompareExchange(ref _lazyHasCodeAnalysisEmbeddedAttribute, (int)ContainingPEModule.Module.HasCodeAnalysisEmbeddedAttribute(_handle).ToThreeState(), 0);
				}
				return _lazyHasCodeAnalysisEmbeddedAttribute == 2;
			}
		}

		internal override bool HasVisualBasicEmbeddedAttribute
		{
			get
			{
				if (_lazyHasVisualBasicEmbeddedAttribute == 0)
				{
					Interlocked.CompareExchange(ref _lazyHasVisualBasicEmbeddedAttribute, (int)ContainingPEModule.Module.HasVisualBasicEmbeddedAttribute(_handle).ToThreeState(), 0);
				}
				return _lazyHasVisualBasicEmbeddedAttribute == 2;
			}
		}

		public override TypeKind TypeKind
		{
			get
			{
				if (_lazyTypeKind == 0)
				{
					TypeKind lazyTypeKind;
					if ((_flags & TypeAttributes.ClassSemanticsMask) != 0)
					{
						lazyTypeKind = TypeKind.Interface;
					}
					else
					{
						TypeSymbol declaredBase = GetDeclaredBase(default(BasesBeingResolved));
						lazyTypeKind = TypeKind.Class;
						if ((object)declaredBase != null)
						{
							SpecialType specialType = declaredBase.SpecialType;
							if (specialType == SpecialType.System_Enum)
							{
								lazyTypeKind = TypeKind.Enum;
							}
							else if (specialType == SpecialType.System_MulticastDelegate || (specialType == SpecialType.System_Delegate && SpecialType != SpecialType.System_MulticastDelegate))
							{
								lazyTypeKind = TypeKind.Delegate;
							}
							else if (specialType == SpecialType.System_ValueType && SpecialType != SpecialType.System_Enum)
							{
								lazyTypeKind = TypeKind.Struct;
							}
							else if (Arity == 0 && (object)ContainingType == null && ContainingPEModule.Module.HasAttribute(_handle, AttributeDescription.StandardModuleAttribute))
							{
								lazyTypeKind = TypeKind.Module;
							}
						}
					}
					_lazyTypeKind = (int)lazyTypeKind;
				}
				return (TypeKind)_lazyTypeKind;
			}
		}

		internal override bool IsInterface => (_flags & TypeAttributes.ClassSemanticsMask) != 0;

		internal override bool IsComImport => (_flags & TypeAttributes.Import) != 0;

		internal override TypeSymbol CoClassType
		{
			get
			{
				if ((object)_lazyCoClassType == ErrorTypeSymbol.UnknownResultType)
				{
					Interlocked.CompareExchange(ref _lazyCoClassType, MakeComImportCoClassType(), ErrorTypeSymbol.UnknownResultType);
				}
				return _lazyCoClassType;
			}
		}

		internal override string DefaultPropertyName
		{
			get
			{
				if (_lazyDefaultPropertyName == null)
				{
					string defaultPropertyName = GetDefaultPropertyName();
					Interlocked.CompareExchange(ref _lazyDefaultPropertyName, defaultPropertyName ?? string.Empty, null);
				}
				if (!string.IsNullOrEmpty(_lazyDefaultPropertyName))
				{
					return _lazyDefaultPropertyName;
				}
				return null;
			}
		}

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, ContainingPEModule);
				return _lazyObsoleteAttributeData;
			}
		}

		internal override bool HasDeclarativeSecurity
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyIsExtensibleInterface == ThreeState.Unknown)
				{
					_lazyIsExtensibleInterface = DecodeIsExtensibleInterface().ToThreeState();
				}
				return _lazyIsExtensibleInterface.Value();
			}
		}

		internal sealed override VisualBasicCompilation DeclaringCompilation => null;

		internal PENamedTypeSymbol(PEModuleSymbol moduleSymbol, PENamespaceSymbol containingNamespace, TypeDefinitionHandle handle)
			: this(moduleSymbol, containingNamespace, 0, handle)
		{
		}

		internal PENamedTypeSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, TypeDefinitionHandle handle)
			: this(moduleSymbol, containingType, (ushort)containingType.MetadataArity, handle)
		{
		}

		private PENamedTypeSymbol(PEModuleSymbol moduleSymbol, NamespaceOrTypeSymbol container, ushort containerMetadataArity, TypeDefinitionHandle handle)
		{
			_lazyAttributeUsageInfo = AttributeUsageInfo.Null;
			_lazyCoClassType = ErrorTypeSymbol.UnknownResultType;
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			_lazyMightContainExtensionMethods = 0;
			_lazyHasCodeAnalysisEmbeddedAttribute = 0;
			_lazyHasVisualBasicEmbeddedAttribute = 0;
			_lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
			_lazyIsExtensibleInterface = ThreeState.Unknown;
			_handle = handle;
			_container = container;
			bool flag = false;
			string text;
			try
			{
				text = moduleSymbol.Module.GetTypeDefNameOrThrow(handle);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				text = string.Empty;
				flag = true;
				ProjectData.ClearProjectError();
			}
			try
			{
				_flags = moduleSymbol.Module.GetTypeDefFlagsOrThrow(handle);
			}
			catch (BadImageFormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				BadImageFormatException ex4 = ex3;
				flag = true;
				ProjectData.ClearProjectError();
			}
			int num;
			try
			{
				_genericParameterHandles = moduleSymbol.Module.GetTypeDefGenericParamsOrThrow(handle);
				num = _genericParameterHandles.Count;
			}
			catch (BadImageFormatException ex5)
			{
				ProjectData.SetProjectError(ex5);
				BadImageFormatException ex6 = ex5;
				_genericParameterHandles = default(GenericParameterHandleCollection);
				num = 0;
				flag = true;
				ProjectData.ClearProjectError();
			}
			if (num > containerMetadataArity)
			{
				_arity = (ushort)(num - containerMetadataArity);
			}
			if (_arity == 0)
			{
				_lazyTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
				_name = text;
				_mangleName = false;
			}
			else
			{
				_name = MetadataHelpers.UnmangleMetadataNameForArity(text, _arity);
				_mangleName = (object)_name != text;
			}
			if (flag || num < containerMetadataArity)
			{
				_lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, this));
			}
		}

		internal override IEnumerable<NamedTypeSymbol> GetInterfacesToEmit()
		{
			return base.InterfacesNoUseSiteDiagnostics;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			if ((_flags & TypeAttributes.ClassSemanticsMask) == 0)
			{
				PEModuleSymbol containingPEModule = ContainingPEModule;
				try
				{
					EntityHandle baseTypeOfTypeOrThrow = containingPEModule.Module.GetBaseTypeOfTypeOrThrow(_handle);
					if (!baseTypeOfTypeOrThrow.IsNil)
					{
						return (NamedTypeSymbol)TupleTypeDecoder.DecodeTupleTypesIfApplicable(new MetadataDecoder(containingPEModule, this).GetTypeOfToken(baseTypeOfTypeOrThrow), _handle, containingPEModule);
					}
				}
				catch (BadImageFormatException ex)
				{
					ProjectData.SetProjectError(ex);
					BadImageFormatException mrEx = ex;
					NamedTypeSymbol result = new UnsupportedMetadataTypeSymbol(mrEx);
					ProjectData.ClearProjectError();
					return result;
				}
			}
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			try
			{
				PEModuleSymbol containingPEModule = ContainingPEModule;
				InterfaceImplementationHandleCollection interfaceImplementationsOrThrow = containingPEModule.Module.GetInterfaceImplementationsOrThrow(_handle);
				if (interfaceImplementationsOrThrow.Count == 0)
				{
					return ImmutableArray<NamedTypeSymbol>.Empty;
				}
				NamedTypeSymbol[] array = new NamedTypeSymbol[interfaceImplementationsOrThrow.Count - 1 + 1];
				MetadataDecoder metadataDecoder = new MetadataDecoder(containingPEModule, this);
				int num = 0;
				foreach (InterfaceImplementationHandle item in interfaceImplementationsOrThrow)
				{
					EntityHandle @interface = containingPEModule.Module.MetadataReader.GetInterfaceImplementation(item).Interface;
					NamedTypeSymbol namedTypeSymbol = ((NamedTypeSymbol)TupleTypeDecoder.DecodeTupleTypesIfApplicable(metadataDecoder.GetTypeOfToken(@interface), item, containingPEModule)) as NamedTypeSymbol;
					array[num] = (((object)namedTypeSymbol != null) ? namedTypeSymbol : new UnsupportedMetadataTypeSymbol());
					num++;
				}
				return array.AsImmutableOrNull();
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException mrEx = ex;
				ImmutableArray<NamedTypeSymbol> result = ImmutableArray.Create((NamedTypeSymbol)new UnsupportedMetadataTypeSymbol(mrEx));
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static ErrorTypeSymbol CyclicInheritanceError(DiagnosticInfo diag)
		{
			return new ExtendedErrorTypeSymbol(diag, reportErrorWhenReferenced: true);
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			DiagnosticInfo dependencyDiagnosticsForImportedClass = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedClass(this);
			if (dependencyDiagnosticsForImportedClass != null)
			{
				return CyclicInheritanceError(dependencyDiagnosticsForImportedClass);
			}
			return GetDeclaredBase(default(BasesBeingResolved));
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = GetDeclaredInterfacesNoUseSiteDiagnostics(default(BasesBeingResolved));
			if (!IsInterface)
			{
				return declaredInterfacesNoUseSiteDiagnostics;
			}
			return (from t in declaredInterfacesNoUseSiteDiagnostics
				select new VB_0024AnonymousType_2<NamedTypeSymbol, DiagnosticInfo>(t, BaseTypeAnalysis.GetDependencyDiagnosticsForImportedBaseInterface(this, t)) into _0024VB_0024It
				select (_0024VB_0024It.diag != null) ? CyclicInheritanceError(_0024VB_0024It.diag) : _0024VB_0024It.t).AsImmutable();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				if ((_lazyTypeKind == 0 && ((_flags & TypeAttributes.ClassSemanticsMask) != 0 || Arity != 0 || (object)ContainingType != null)) || TypeKind != TypeKind.Module)
				{
					ContainingPEModule.LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
				}
				else
				{
					PEModuleSymbol containingPEModule = ContainingPEModule;
					EntityHandle token = _handle;
					AttributeDescription standardModuleAttribute = AttributeDescription.StandardModuleAttribute;
					CustomAttributeHandle filteredOutAttribute = default(CustomAttributeHandle);
					CustomAttributeHandle filteredOutAttribute2;
					ImmutableArray<VisualBasicAttributeData> customAttributesForToken = containingPEModule.GetCustomAttributesForToken(token, out filteredOutAttribute2, standardModuleAttribute, out filteredOutAttribute);
					ImmutableInterlocked.InterlockedInitialize(ref _lazyCustomAttributes, customAttributesForToken);
				}
			}
			return _lazyCustomAttributes;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_63_GetCustomAttributesToEmit))]
		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_63_GetCustomAttributesToEmit(-2)
			{
				_0024VB_0024Me = this,
				_0024P_compilationState = compilationState
			};
		}

		private void EnsureNonTypeMemberNamesAreLoaded()
		{
			if (_lazyMemberNames != null)
			{
				return;
			}
			PEModule module = ContainingPEModule.Module;
			HashSet<string> hashSet = new HashSet<string>();
			try
			{
				foreach (MethodDefinitionHandle item in module.GetMethodsOfTypeOrThrow(_handle))
				{
					try
					{
						hashSet.Add(module.GetMethodDefNameOrThrow(item));
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				BadImageFormatException ex4 = ex3;
				ProjectData.ClearProjectError();
			}
			try
			{
				foreach (PropertyDefinitionHandle item2 in module.GetPropertiesOfTypeOrThrow(_handle))
				{
					try
					{
						hashSet.Add(module.GetPropertyDefNameOrThrow(item2));
					}
					catch (BadImageFormatException ex5)
					{
						ProjectData.SetProjectError(ex5);
						BadImageFormatException ex6 = ex5;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex7)
			{
				ProjectData.SetProjectError(ex7);
				BadImageFormatException ex8 = ex7;
				ProjectData.ClearProjectError();
			}
			try
			{
				foreach (EventDefinitionHandle item3 in module.GetEventsOfTypeOrThrow(_handle))
				{
					try
					{
						hashSet.Add(module.GetEventDefNameOrThrow(item3));
					}
					catch (BadImageFormatException ex9)
					{
						ProjectData.SetProjectError(ex9);
						BadImageFormatException ex10 = ex9;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex11)
			{
				ProjectData.SetProjectError(ex11);
				BadImageFormatException ex12 = ex11;
				ProjectData.ClearProjectError();
			}
			try
			{
				foreach (FieldDefinitionHandle item4 in module.GetFieldsOfTypeOrThrow(_handle))
				{
					try
					{
						hashSet.Add(module.GetFieldDefNameOrThrow(item4));
					}
					catch (BadImageFormatException ex13)
					{
						ProjectData.SetProjectError(ex13);
						BadImageFormatException ex14 = ex13;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex15)
			{
				ProjectData.SetProjectError(ex15);
				BadImageFormatException ex16 = ex15;
				ProjectData.ClearProjectError();
			}
			Interlocked.CompareExchange(ref _lazyMemberNames, SpecializedCollections.ReadOnlySet(hashSet), null);
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			EnsureNestedTypesAreLoaded();
			EnsureNonTypeMembersAreLoaded();
			return _lazyMembers.Flatten(DeclarationOrderSymbolComparer.Instance);
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			EnsureNestedTypesAreLoaded();
			EnsureNonTypeMembersAreLoaded();
			return _lazyMembers.Flatten().ConditionallyDeOrder();
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			return GetMembers<FieldSymbol>(GetMembers(), SymbolKind.Field, 0);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_70_GetMethodsToEmit))]
		internal override IEnumerable<MethodSymbol> GetMethodsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_70_GetMethodsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal override IEnumerable<PropertySymbol> GetPropertiesToEmit()
		{
			return GetMembers<PropertySymbol>(GetMembers(), SymbolKind.Property);
		}

		internal override IEnumerable<EventSymbol> GetEventsToEmit()
		{
			return GetMembers<EventSymbol>(GetMembers(), SymbolKind.Event);
		}

		private void EnsureNonTypeMembersAreLoaded()
		{
			if (_lazyMembers != null)
			{
				return;
			}
			Dictionary<MethodDefinitionHandle, PEMethodSymbol> dictionary = CreateMethods();
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			bool flag = (TypeKind == TypeKind.Struct || TypeKind == TypeKind.Enum) && !base.IsShared;
			foreach (PEMethodSymbol value4 in dictionary.Values)
			{
				instance.Add(value4);
				if (flag)
				{
					flag = !value4.IsParameterlessConstructor();
				}
			}
			if (flag)
			{
				instance.Add(new SynthesizedConstructorSymbol(null, this, base.IsShared, isDebuggable: false, null, null));
			}
			HashSet<string> witheventPropertyNames = null;
			CreateProperties(dictionary, instance);
			CreateFields(instance, out witheventPropertyNames);
			CreateEvents(dictionary, instance);
			Dictionary<string, ImmutableArray<Symbol>> dictionary2 = new Dictionary<string, ImmutableArray<Symbol>>(CaseInsensitiveComparison.Comparer);
			IEnumerable<IGrouping<string, Symbol>> enumerable = instance.GroupBy((Symbol m) => m.Name, CaseInsensitiveComparison.Comparer);
			foreach (IGrouping<string, Symbol> item in enumerable)
			{
				dictionary2.Add(item.Key, ImmutableArray.CreateRange(item));
			}
			instance.Free();
			if (witheventPropertyNames != null)
			{
				foreach (string item2 in witheventPropertyNames)
				{
					ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
					if (dictionary2.TryGetValue(item2, out value) && value.Length == 1 && value[0] is PEPropertySymbol pEPropertySymbol && IsValidWithEventsProperty(pEPropertySymbol))
					{
						pEPropertySymbol.SetIsWithEvents(value: true);
					}
				}
			}
			foreach (ImmutableArray<PENamedTypeSymbol> value5 in _lazyNestedTypes.Values)
			{
				string name = value5[0].Name;
				ImmutableArray<Symbol> value2 = default(ImmutableArray<Symbol>);
				if (!dictionary2.TryGetValue(name, out value2))
				{
					dictionary2.Add(name, StaticCast<Symbol>.From(value5));
				}
				else
				{
					dictionary2[name] = value2.Concat(StaticCast<Symbol>.From(value5));
				}
			}
			if (Interlocked.CompareExchange(ref _lazyMembers, dictionary2, null) == null)
			{
				ICollection<string> value3 = SpecializedCollections.ReadOnlyCollection(dictionary2.Keys);
				Interlocked.Exchange(ref _lazyMemberNames, value3);
			}
		}

		private bool IsValidWithEventsProperty(PEPropertySymbol prop)
		{
			if (prop.IsReadOnly | prop.IsWriteOnly)
			{
				return false;
			}
			if (!prop.IsOverridable)
			{
				return false;
			}
			return true;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			EnsureNestedTypesAreLoaded();
			EnsureNonTypeMembersAreLoaded();
			ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
			if (_lazyMembers.TryGetValue(name, out value))
			{
				return value;
			}
			return ImmutableArray<Symbol>.Empty;
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			EnsureNestedTypesAreLoaded();
			return StaticCast<NamedTypeSymbol>.From(_lazyNestedTypes.Flatten());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			EnsureNestedTypesAreLoaded();
			return StaticCast<NamedTypeSymbol>.From(_lazyNestedTypes.Flatten(DeclarationOrderSymbolComparer.Instance));
		}

		private void EnsureNestedTypesAreLoaded()
		{
			if (_lazyNestedTypes == null)
			{
				Dictionary<string, ImmutableArray<PENamedTypeSymbol>> dictionary = CreateNestedTypes();
				Interlocked.CompareExchange(ref _lazyNestedTypes, dictionary, null);
				if (_lazyNestedTypes == dictionary)
				{
					ContainingPEModule.OnNewTypeDeclarationsLoaded(dictionary);
				}
			}
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			EnsureNestedTypesAreLoaded();
			ImmutableArray<PENamedTypeSymbol> value = default(ImmutableArray<PENamedTypeSymbol>);
			if (_lazyNestedTypes.TryGetValue(name, out value))
			{
				return StaticCast<NamedTypeSymbol>.From(value);
			}
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol type, int arity_) => type.Arity == arity_, arity);
		}

		private void EnsureTypeParametersAreLoaded()
		{
			if (_lazyTypeParameters.IsDefault)
			{
				PETypeParameterSymbol[] array = new PETypeParameterSymbol[_arity - 1 + 1];
				PEModuleSymbol containingPEModule = ContainingPEModule;
				GenericParameterHandleCollection genericParameterHandles = _genericParameterHandles;
				int num = genericParameterHandles.Count - Arity;
				int num2 = array.Length - 1;
				for (int i = 0; i <= num2; i++)
				{
					int num3 = i;
					ushort ordinal = (ushort)i;
					genericParameterHandles = _genericParameterHandles;
					array[num3] = new PETypeParameterSymbol(containingPEModule, this, ordinal, genericParameterHandles[num + i]);
				}
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, StaticCast<TypeParameterSymbol>.From(array.AsImmutableOrNull()), default(ImmutableArray<TypeParameterSymbol>));
			}
		}

		internal override bool GetGuidString(ref string guidString)
		{
			return ContainingPEModule.Module.HasGuidAttribute(_handle, out guidString);
		}

		internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map, NamespaceSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				EnsureNestedTypesAreLoaded();
				EnsureNonTypeMembersAreLoaded();
				if (!appendThrough.BuildExtensionMethodsMap(map, _lazyMembers))
				{
					_lazyMightContainExtensionMethods = 1;
				}
			}
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamedTypeSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				EnsureNestedTypesAreLoaded();
				EnsureNonTypeMembersAreLoaded();
				if (!appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, _lazyMembers))
				{
					_lazyMightContainExtensionMethods = 1;
				}
			}
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return PEDocumentationCommentUtils.GetDocumentationComment(this, ContainingPEModule, preferredCulture, cancellationToken, ref _lazyDocComment);
		}

		private TypeSymbol MakeComImportCoClassType()
		{
			if (!IsInterface)
			{
				return null;
			}
			string value = null;
			if (!ContainingPEModule.Module.HasStringValuedAttribute(_handle, AttributeDescription.CoClassAttribute, out value))
			{
				return null;
			}
			return new MetadataDecoder(ContainingPEModule).GetTypeSymbolForSerializedType(value);
		}

		private string GetDefaultPropertyName()
		{
			string memberName = null;
			ContainingPEModule.Module.HasDefaultMemberAttribute(_handle, out memberName);
			if (memberName != null)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(memberName).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Kind == SymbolKind.Property)
					{
						return memberName;
					}
				}
			}
			return null;
		}

		private Dictionary<string, ImmutableArray<PENamedTypeSymbol>> CreateNestedTypes()
		{
			ArrayBuilder<PENamedTypeSymbol> instance = ArrayBuilder<PENamedTypeSymbol>.GetInstance();
			PEModuleSymbol containingPEModule = ContainingPEModule;
			PEModule module = containingPEModule.Module;
			try
			{
				ImmutableArray<TypeDefinitionHandle>.Enumerator enumerator = module.GetNestedTypeDefsOrThrow(_handle).GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeDefinitionHandle current = enumerator.Current;
					if (module.ShouldImportNestedType(current))
					{
						instance.Add(new PENamedTypeSymbol(containingPEModule, this, current));
					}
				}
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				ProjectData.ClearProjectError();
			}
			IEnumerable<IGrouping<string, PENamedTypeSymbol>> enumerable = instance.GroupBy((PENamedTypeSymbol t) => t.Name, CaseInsensitiveComparison.Comparer);
			Dictionary<string, ImmutableArray<PENamedTypeSymbol>> dictionary = new Dictionary<string, ImmutableArray<PENamedTypeSymbol>>(CaseInsensitiveComparison.Comparer);
			foreach (IGrouping<string, PENamedTypeSymbol> item in enumerable)
			{
				dictionary.Add(item.Key, item.ToArray().AsImmutableOrNull());
			}
			instance.Free();
			return dictionary;
		}

		private void CreateFields(ArrayBuilder<Symbol> members, out HashSet<string> witheventPropertyNames)
		{
			PEModuleSymbol containingPEModule = ContainingPEModule;
			PEModule module = containingPEModule.Module;
			try
			{
				bool flag = default(bool);
				foreach (FieldDefinitionHandle item in module.GetFieldsOfTypeOrThrow(_handle))
				{
					try
					{
						flag = module.ShouldImportField(item, containingPEModule.ImportOptions);
						if (!flag)
						{
							switch (TypeKind)
							{
							case TypeKind.Struct:
							{
								SpecialType specialType = SpecialType;
								if ((specialType == SpecialType.None || specialType == SpecialType.System_Nullable_T) && (module.GetFieldDefFlagsOrThrow(item) & FieldAttributes.Static) == 0)
								{
									flag = true;
								}
								break;
							}
							case TypeKind.Enum:
								if ((module.GetFieldDefFlagsOrThrow(item) & FieldAttributes.Static) == 0)
								{
									flag = true;
								}
								break;
							}
						}
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						ProjectData.ClearProjectError();
					}
					if (flag)
					{
						members.Add(new PEFieldSymbol(containingPEModule, this, item));
					}
					string propertyName = null;
					if (module.HasAccessedThroughPropertyAttribute(item, out propertyName))
					{
						if (witheventPropertyNames == null)
						{
							witheventPropertyNames = new HashSet<string>(CaseInsensitiveComparison.Comparer);
						}
						witheventPropertyNames.Add(propertyName);
					}
				}
			}
			catch (BadImageFormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				BadImageFormatException ex4 = ex3;
				ProjectData.ClearProjectError();
			}
		}

		private Dictionary<MethodDefinitionHandle, PEMethodSymbol> CreateMethods()
		{
			Dictionary<MethodDefinitionHandle, PEMethodSymbol> dictionary = new Dictionary<MethodDefinitionHandle, PEMethodSymbol>();
			PEModuleSymbol containingPEModule = ContainingPEModule;
			PEModule module = containingPEModule.Module;
			try
			{
				foreach (MethodDefinitionHandle item in module.GetMethodsOfTypeOrThrow(_handle))
				{
					if (module.ShouldImportMethod(item, containingPEModule.ImportOptions))
					{
						dictionary.Add(item, new PEMethodSymbol(containingPEModule, this, item));
					}
				}
				return dictionary;
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				ProjectData.ClearProjectError();
				return dictionary;
			}
		}

		private void CreateProperties(Dictionary<MethodDefinitionHandle, PEMethodSymbol> methodHandleToSymbol, ArrayBuilder<Symbol> members)
		{
			PEModuleSymbol containingPEModule = ContainingPEModule;
			PEModule module = containingPEModule.Module;
			try
			{
				foreach (PropertyDefinitionHandle item in module.GetPropertiesOfTypeOrThrow(_handle))
				{
					try
					{
						PropertyAccessors propertyMethodsOrThrow = module.GetPropertyMethodsOrThrow(item);
						PEMethodSymbol accessorMethod = GetAccessorMethod(containingPEModule, methodHandleToSymbol, propertyMethodsOrThrow.Getter);
						PEMethodSymbol accessorMethod2 = GetAccessorMethod(containingPEModule, methodHandleToSymbol, propertyMethodsOrThrow.Setter);
						if ((object)accessorMethod != null || (object)accessorMethod2 != null)
						{
							members.Add(PEPropertySymbol.Create(containingPEModule, this, item, accessorMethod, accessorMethod2));
						}
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				BadImageFormatException ex4 = ex3;
				ProjectData.ClearProjectError();
			}
		}

		private void CreateEvents(Dictionary<MethodDefinitionHandle, PEMethodSymbol> methodHandleToSymbol, ArrayBuilder<Symbol> members)
		{
			PEModuleSymbol containingPEModule = ContainingPEModule;
			PEModule module = containingPEModule.Module;
			try
			{
				foreach (EventDefinitionHandle item in module.GetEventsOfTypeOrThrow(_handle))
				{
					try
					{
						EventAccessors eventMethodsOrThrow = module.GetEventMethodsOrThrow(item);
						PEMethodSymbol accessorMethod = GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Adder);
						PEMethodSymbol accessorMethod2 = GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Remover);
						PEMethodSymbol accessorMethod3 = GetAccessorMethod(containingPEModule, methodHandleToSymbol, eventMethodsOrThrow.Raiser);
						if ((object)accessorMethod != null && (object)accessorMethod2 != null)
						{
							members.Add(new PEEventSymbol(containingPEModule, this, item, accessorMethod, accessorMethod2, accessorMethod3));
						}
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						ProjectData.ClearProjectError();
					}
				}
			}
			catch (BadImageFormatException ex3)
			{
				ProjectData.SetProjectError(ex3);
				BadImageFormatException ex4 = ex3;
				ProjectData.ClearProjectError();
			}
		}

		private static PEMethodSymbol GetAccessorMethod(PEModuleSymbol moduleSymbol, Dictionary<MethodDefinitionHandle, PEMethodSymbol> methodHandleToSymbol, MethodDefinitionHandle methodDef)
		{
			if (methodDef.IsNil)
			{
				return null;
			}
			PEMethodSymbol value = null;
			methodHandleToSymbol.TryGetValue(methodDef, out value);
			return value;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			AssemblySymbol primaryDependency = base.PrimaryDependency;
			if (!_lazyCachedUseSiteInfo.IsInitialized)
			{
				_lazyCachedUseSiteInfo.Initialize(primaryDependency, CalculateUseSiteInfoImpl());
			}
			return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
		}

		private UseSiteInfo<AssemblySymbol> CalculateUseSiteInfoImpl()
		{
			UseSiteInfo<AssemblySymbol> result = CalculateUseSiteInfo();
			if (result.DiagnosticInfo == null)
			{
				UseSiteInfo<AssemblySymbol> result2;
				if (ContainingPEModule.Module.HasRequiredAttributeAttribute(Handle))
				{
					result2 = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, this));
				}
				else
				{
					TypeKind typeKind = TypeKind;
					SpecialType specialType = SpecialType;
					if ((typeKind == TypeKind.Class || typeKind == TypeKind.Module) && specialType != SpecialType.System_Enum && specialType != SpecialType.System_MulticastDelegate)
					{
						TypeSymbol declaredBase = GetDeclaredBase(default(BasesBeingResolved));
						if ((object)declaredBase != null && declaredBase.SpecialType == SpecialType.None)
						{
							AssemblySymbol containingAssembly = declaredBase.ContainingAssembly;
							if ((object)containingAssembly != null && containingAssembly.IsMissing && declaredBase is MissingMetadataTypeSymbol.TopLevel topLevel && topLevel.Arity == 0)
							{
								SpecialType typeFromMetadataName = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(topLevel.NamespaceName, topLevel.MetadataName));
								if ((uint)(typeFromMetadataName - 2) <= 3u)
								{
									return topLevel.GetUseSiteInfo();
								}
							}
						}
					}
					if (MatchesContainingTypeParameters())
					{
						goto IL_0107;
					}
					result2 = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_NestingViolatesCLS1, this));
				}
				return result2;
			}
			goto IL_0107;
			IL_0107:
			return result;
		}

		private bool MatchesContainingTypeParameters()
		{
			GenericParameterHandleCollection genericParameterHandles = _genericParameterHandles;
			if (genericParameterHandles.Count == 0)
			{
				return true;
			}
			NamedTypeSymbol containingType = ContainingType;
			if ((object)containingType == null)
			{
				return true;
			}
			ImmutableArray<TypeParameterSymbol> allTypeParameters = TypeSymbolExtensions.GetAllTypeParameters(containingType);
			int length = allTypeParameters.Length;
			if (length == 0)
			{
				return true;
			}
			PENamedTypeSymbol pENamedTypeSymbol = new PENamedTypeSymbol(ContainingPEModule, (PENamespaceSymbol)base.ContainingNamespace, _handle);
			ImmutableArray<TypeParameterSymbol> typeParameters = pENamedTypeSymbol.TypeParameters;
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(containingType, allTypeParameters, IndexedTypeParameterSymbol.Take(length).As<TypeSymbol>());
			TypeSubstitution typeSubstitution2 = TypeSubstitution.Create(pENamedTypeSymbol, typeParameters, IndexedTypeParameterSymbol.Take(typeParameters.Length).As<TypeSymbol>());
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeParameterSymbol typeParameter = allTypeParameters[i];
				TypeParameterSymbol typeParameter2 = typeParameters[i];
				if (!MethodSignatureComparer.HaveSameConstraints(typeParameter, typeSubstitution, typeParameter2, typeSubstitution2))
				{
					return false;
				}
			}
			return true;
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			if (_lazyConditionalAttributeSymbols.IsDefault)
			{
				ImmutableArray<string> conditionalAttributeValues = ContainingPEModule.Module.GetConditionalAttributeValues(_handle);
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyConditionalAttributeSymbols, conditionalAttributeValues, default(ImmutableArray<string>));
			}
			return _lazyConditionalAttributeSymbols;
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			if (_lazyAttributeUsageInfo.IsNull)
			{
				_lazyAttributeUsageInfo = DecodeAttributeUsageInfo();
			}
			return _lazyAttributeUsageInfo;
		}

		private AttributeUsageInfo DecodeAttributeUsageInfo()
		{
			CustomAttributeHandle attributeUsageAttributeHandle = ContainingPEModule.Module.GetAttributeUsageAttributeHandle(_handle);
			if (!attributeUsageAttributeHandle.IsNil)
			{
				MetadataDecoder metadataDecoder = new MetadataDecoder(ContainingPEModule);
				TypedConstant[] positionalArgs = null;
				KeyValuePair<string, TypedConstant>[] namedArgs = null;
				if (metadataDecoder.GetCustomAttribute(attributeUsageAttributeHandle, out positionalArgs, out namedArgs))
				{
					return AttributeData.DecodeAttributeUsageAttribute(positionalArgs[0], namedArgs.AsImmutableOrNull());
				}
			}
			return base.BaseTypeNoUseSiteDiagnostics?.GetAttributeUsageInfo() ?? AttributeUsageInfo.Default;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		private bool DecodeIsExtensibleInterface()
		{
			if (TypeSymbolExtensions.IsInterfaceType(this))
			{
				if (HasAttributeForExtensibleInterface())
				{
					return true;
				}
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = base.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsExtensibleInterfaceNoUseSiteDiagnostics)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool HasAttributeForExtensibleInterface()
		{
			PEModule module = ContainingPEModule.Module;
			Microsoft.Cci.TypeLibTypeFlags flags = (Microsoft.Cci.TypeLibTypeFlags)0;
			if (module.HasTypeLibTypeAttribute(_handle, out flags) && (flags & Microsoft.Cci.TypeLibTypeFlags.FNonExtensible) == 0)
			{
				return true;
			}
			ComInterfaceType interfaceType = ComInterfaceType.InterfaceIsDual;
			if (module.HasInterfaceTypeAttribute(_handle, out interfaceType) && (interfaceType & ComInterfaceType.InterfaceIsIDispatch) != 0)
			{
				return true;
			}
			return false;
		}

		private static int GetIndexOfFirstMember(ImmutableArray<Symbol> members, SymbolKind kind)
		{
			int length = members.Length;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (members[i].Kind == kind)
				{
					return i;
				}
			}
			return length;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_152_GetMembers<>))]
		private static IEnumerable<TSymbol> GetMembers<TSymbol>(ImmutableArray<Symbol> members, SymbolKind kind, int offset = -1) where TSymbol : Symbol
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_152_GetMembers<TSymbol>(-2)
			{
				_0024P_members = members,
				_0024P_kind = kind,
				_0024P_offset = offset
			};
		}

		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}
