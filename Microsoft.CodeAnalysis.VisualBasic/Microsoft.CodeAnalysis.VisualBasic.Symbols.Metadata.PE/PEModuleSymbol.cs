using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEModuleSymbol : NonMissingModuleSymbol
	{
		private readonly AssemblySymbol _assemblySymbol;

		private readonly int _ordinal;

		private readonly PEModule _module;

		private readonly PENamespaceSymbol _globalNamespace;

		private NamedTypeSymbol _lazySystemTypeSymbol;

		private const int s_defaultTypeMapCapacity = 31;

		internal readonly ConcurrentDictionary<TypeDefinitionHandle, TypeSymbol> TypeHandleToTypeMap;

		internal readonly ConcurrentDictionary<TypeReferenceHandle, TypeSymbol> TypeRefHandleToTypeMap;

		internal readonly ImmutableArray<MetadataLocation> MetadataLocation;

		internal readonly MetadataImportOptions ImportOptions;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ImmutableArray<VisualBasicAttributeData> _lazyAssemblyAttributes;

		private ICollection<string> _lazyTypeNames;

		private ICollection<string> _lazyNamespaceNames;

		internal override int Ordinal => _ordinal;

		internal override Machine Machine => _module.Machine;

		internal override bool Bit32Required => _module.Bit32Required;

		internal PEModule Module => _module;

		public override Symbol ContainingSymbol => _assemblySymbol;

		public override string Name => _module.Name;

		public override NamespaceSymbol GlobalNamespace => _globalNamespace;

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(MetadataLocation);

		public override AssemblySymbol ContainingAssembly => _assemblySymbol;

		internal override ICollection<string> TypeNames
		{
			get
			{
				if (_lazyTypeNames == null)
				{
					Interlocked.CompareExchange(ref _lazyTypeNames, _module.TypeNames.AsCaseInsensitiveCollection(), null);
				}
				return _lazyTypeNames;
			}
		}

		internal override ICollection<string> NamespaceNames
		{
			get
			{
				if (_lazyNamespaceNames == null)
				{
					Interlocked.CompareExchange(ref _lazyNamespaceNames, _module.NamespaceNames.AsCaseInsensitiveCollection(), null);
				}
				return _lazyNamespaceNames;
			}
		}

		internal DocumentationProvider DocumentationProvider
		{
			get
			{
				if (ContainingAssembly is PEAssemblySymbol pEAssemblySymbol)
				{
					return pEAssemblySymbol.DocumentationProvider;
				}
				return DocumentationProvider.Default;
			}
		}

		internal NamedTypeSymbol SystemTypeSymbol
		{
			get
			{
				if ((object)_lazySystemTypeSymbol == null)
				{
					Interlocked.CompareExchange(ref _lazySystemTypeSymbol, GetWellKnownType(WellKnownType.System_Type), null);
				}
				return _lazySystemTypeSymbol;
			}
		}

		internal override bool HasAssemblyCompilationRelaxationsAttribute => AttributeDataExtensions.IndexOfAttribute(GetAssemblyAttributes(), this, AttributeDescription.CompilationRelaxationsAttribute) >= 0;

		internal override bool HasAssemblyRuntimeCompatibilityAttribute => AttributeDataExtensions.IndexOfAttribute(GetAssemblyAttributes(), this, AttributeDescription.RuntimeCompatibilityAttribute) >= 0;

		internal override CharSet? DefaultMarshallingCharSet
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PEModuleSymbol(PEAssemblySymbol assemblySymbol, PEModule module, MetadataImportOptions importOptions, int ordinal)
			: this((AssemblySymbol)assemblySymbol, module, importOptions, ordinal)
		{
		}

		internal PEModuleSymbol(SourceAssemblySymbol assemblySymbol, PEModule module, MetadataImportOptions importOptions, int ordinal)
			: this((AssemblySymbol)assemblySymbol, module, importOptions, ordinal)
		{
		}

		internal PEModuleSymbol(RetargetingAssemblySymbol assemblySymbol, PEModule module, MetadataImportOptions importOptions, int ordinal)
			: this((AssemblySymbol)assemblySymbol, module, importOptions, ordinal)
		{
		}

		private PEModuleSymbol(AssemblySymbol assemblySymbol, PEModule module, MetadataImportOptions importOptions, int ordinal)
		{
			TypeHandleToTypeMap = new ConcurrentDictionary<TypeDefinitionHandle, TypeSymbol>(2, 31);
			TypeRefHandleToTypeMap = new ConcurrentDictionary<TypeReferenceHandle, TypeSymbol>(2, 31);
			MetadataLocation = ImmutableArray.Create(new MetadataLocation(this));
			_assemblySymbol = assemblySymbol;
			_ordinal = ordinal;
			_module = module;
			_globalNamespace = new PEGlobalNamespaceSymbol(this);
			ImportOptions = importOptions;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				LoadCustomAttributes(EntityHandle.ModuleDefinition, ref _lazyCustomAttributes);
			}
			return _lazyCustomAttributes;
		}

		internal ImmutableArray<VisualBasicAttributeData> GetAssemblyAttributes()
		{
			if (_lazyAssemblyAttributes.IsDefault)
			{
				ArrayBuilder<VisualBasicAttributeData> arrayBuilder = null;
				string name = ContainingAssembly.CorLibrary.Name;
				EntityHandle resolutionScope = Module.GetAssemblyRef(name);
				if (!resolutionScope.IsNil)
				{
					IEnumerator enumerator = MetadataWriter.dummyAssemblyAttributeParentQualifier.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string text = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(enumerator.Current);
						EntityHandle typeRef = Module.GetTypeRef(resolutionScope, "System.Runtime.CompilerServices", "AssemblyAttributesGoHere" + text);
						if (typeRef.IsNil)
						{
							continue;
						}
						try
						{
							foreach (CustomAttributeHandle item in Module.GetCustomAttributesOrThrow(typeRef))
							{
								if (arrayBuilder == null)
								{
									arrayBuilder = ArrayBuilder<VisualBasicAttributeData>.GetInstance();
								}
								arrayBuilder.Add(new PEAttributeData(this, item));
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
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyAssemblyAttributes, arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<VisualBasicAttributeData>.Empty, default(ImmutableArray<VisualBasicAttributeData>));
			}
			return _lazyAssemblyAttributes;
		}

		internal ImmutableArray<VisualBasicAttributeData> GetCustomAttributesForToken(EntityHandle token)
		{
			CustomAttributeHandle filteredOutAttribute = default(CustomAttributeHandle);
			CustomAttributeHandle filteredOutAttribute2 = default(CustomAttributeHandle);
			return GetCustomAttributesForToken(token, out filteredOutAttribute, default(AttributeDescription), out filteredOutAttribute2);
		}

		internal ImmutableArray<VisualBasicAttributeData> GetCustomAttributesForToken(EntityHandle token, out CustomAttributeHandle filteredOutAttribute1, AttributeDescription filterOut1, out CustomAttributeHandle filteredOutAttribute2 = default(CustomAttributeHandle), AttributeDescription filterOut2 = default(AttributeDescription))
		{
			ArrayBuilder<VisualBasicAttributeData> arrayBuilder = null;
			filteredOutAttribute1 = default(CustomAttributeHandle);
			filteredOutAttribute2 = default(CustomAttributeHandle);
			try
			{
				foreach (CustomAttributeHandle item in Module.GetCustomAttributesOrThrow(token))
				{
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<VisualBasicAttributeData>.GetInstance();
					}
					if (filterOut1.Signatures != null && Module.GetTargetAttributeSignatureIndex(item, filterOut1) != -1)
					{
						filteredOutAttribute1 = item;
					}
					else if (filterOut2.Signatures != null && Module.GetTargetAttributeSignatureIndex(item, filterOut2) != -1)
					{
						filteredOutAttribute2 = item;
					}
					else
					{
						arrayBuilder.Add(new PEAttributeData(this, item));
					}
				}
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				ProjectData.ClearProjectError();
			}
			return arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal void LoadCustomAttributes(EntityHandle token, ref ImmutableArray<VisualBasicAttributeData> lazyCustomAttributes)
		{
			ImmutableArray<VisualBasicAttributeData> customAttributesForToken = GetCustomAttributesForToken(token);
			ImmutableInterlocked.InterlockedCompareExchange(ref lazyCustomAttributes, customAttributesForToken, default(ImmutableArray<VisualBasicAttributeData>));
		}

		internal void OnNewTypeDeclarationsLoaded(Dictionary<string, ImmutableArray<PENamedTypeSymbol>> typesDict)
		{
			bool flag = _ordinal == 0 && _assemblySymbol.KeepLookingForDeclaredSpecialTypes;
			foreach (ImmutableArray<PENamedTypeSymbol> value in typesDict.Values)
			{
				ImmutableArray<PENamedTypeSymbol>.Enumerator enumerator2 = value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					PENamedTypeSymbol current = enumerator2.Current;
					TypeHandleToTypeMap.TryAdd(current.Handle, current);
					if (flag && current.SpecialType != 0)
					{
						_assemblySymbol.RegisterDeclaredSpecialType(current);
						flag = _assemblySymbol.KeepLookingForDeclaredSpecialTypes;
					}
				}
			}
		}

		internal override ImmutableArray<byte> GetHash(AssemblyHashAlgorithm algorithmId)
		{
			return _module.GetHash(algorithmId);
		}

		public NamedTypeSymbol GetEventRegistrationTokenType()
		{
			return GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken);
		}

		private NamedTypeSymbol GetWellKnownType(WellKnownType type)
		{
			MetadataTypeName emittedName = MetadataTypeName.FromFullName(type.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
			NamedTypeSymbol namedTypeSymbol = LookupTopLevelMetadataType(ref emittedName);
			if (IsAcceptableSystemTypeSymbol(namedTypeSymbol))
			{
				return namedTypeSymbol;
			}
			NamedTypeSymbol namedTypeSymbol2 = null;
			ImmutableArray<AssemblySymbol>.Enumerator enumerator = GetReferencedAssemblySymbols().GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol namedTypeSymbol3 = enumerator.Current.LookupTopLevelMetadataType(ref emittedName, digThroughForwardedTypes: true);
				if (IsAcceptableSystemTypeSymbol(namedTypeSymbol3))
				{
					if ((object)namedTypeSymbol2 == null)
					{
						namedTypeSymbol2 = namedTypeSymbol3;
					}
					else if ((object)namedTypeSymbol2 != namedTypeSymbol3)
					{
						namedTypeSymbol2 = null;
						break;
					}
				}
			}
			if ((object)namedTypeSymbol2 != null)
			{
				return namedTypeSymbol2;
			}
			return namedTypeSymbol;
		}

		private static bool IsAcceptableSystemTypeSymbol(NamedTypeSymbol candidate)
		{
			if (candidate.Kind != SymbolKind.ErrorType)
			{
				return !(candidate is MissingMetadataTypeSymbol);
			}
			return false;
		}

		internal NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName, out bool isNoPiaLocalType)
		{
			PENamespaceSymbol pENamespaceSymbol = (PENamespaceSymbol)GlobalNamespace.LookupNestedNamespace(emittedName.NamespaceSegments);
			if ((object)pENamespaceSymbol == null)
			{
				isNoPiaLocalType = false;
				return new MissingMetadataTypeSymbol.TopLevel(this, ref emittedName);
			}
			return pENamespaceSymbol.LookupMetadataType(ref emittedName, out isNoPiaLocalType);
		}

		internal (AssemblySymbol FirstSymbol, AssemblySymbol SecondSymbol) GetAssembliesForForwardedType(ref MetadataTypeName fullName, bool ignoreCase, out string matchedName)
		{
			(int, int) assemblyRefsForForwardedType = Module.GetAssemblyRefsForForwardedType(fullName.FullName, ignoreCase, out matchedName);
			(AssemblySymbol, AssemblySymbol) result;
			if (assemblyRefsForForwardedType.Item1 < 0)
			{
				result = (null, null);
			}
			else
			{
				AssemblySymbol referencedAssemblySymbol = GetReferencedAssemblySymbol(assemblyRefsForForwardedType.Item1);
				if (assemblyRefsForForwardedType.Item2 < 0)
				{
					result = (referencedAssemblySymbol, null);
				}
				else
				{
					AssemblySymbol referencedAssemblySymbol2 = GetReferencedAssemblySymbol(assemblyRefsForForwardedType.Item2);
					result = (referencedAssemblySymbol, referencedAssemblySymbol2);
				}
			}
			return result;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_64_GetForwardedTypes))]
		internal IEnumerable<NamedTypeSymbol> GetForwardedTypes()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_64_GetForwardedTypes(-2)
			{
				_0024VB_0024Me = this
			};
		}

		private AssemblySymbol GetReferencedAssemblySymbol(AssemblyReferenceHandle assemblyRef)
		{
			int assemblyReferenceIndexOrThrow;
			try
			{
				assemblyReferenceIndexOrThrow = Module.GetAssemblyReferenceIndexOrThrow(assemblyRef);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				AssemblySymbol result = null;
				ProjectData.ClearProjectError();
				return result;
			}
			return GetReferencedAssemblySymbol(assemblyReferenceIndexOrThrow);
		}

		public override ModuleMetadata GetMetadata()
		{
			return _module.GetNonDisposableMetadata();
		}
	}
}
