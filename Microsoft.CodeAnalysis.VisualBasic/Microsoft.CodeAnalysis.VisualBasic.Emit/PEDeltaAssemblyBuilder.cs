using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class PEDeltaAssemblyBuilder : PEAssemblyBuilderBase, IPEDeltaAssemblyBuilder
	{
		private readonly EmitBaseline _previousGeneration;

		private readonly VisualBasicDefinitionMap _previousDefinitions;

		private readonly SymbolChanges _changes;

		private readonly VisualBasicSymbolMatcher.DeepTranslator _deepTranslator;

		public override int CurrentGenerationOrdinal => _previousGeneration.Ordinal + 1;

		internal EmitBaseline PreviousGeneration => _previousGeneration;

		internal VisualBasicDefinitionMap PreviousDefinitions => _previousDefinitions;

		internal override bool SupportsPrivateImplClass => false;

		internal SymbolChanges Changes => _changes;

		internal override bool AllowOmissionOfConditionalCalls => true;

		public override IEnumerable<string> LinkedAssembliesDebugInfo => SpecializedCollections.EmptyEnumerable<string>();

		public PEDeltaAssemblyBuilder(SourceAssemblySymbol sourceAssembly, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources, EmitBaseline previousGeneration, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol)
			: base(sourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources, ImmutableArray<NamedTypeSymbol>.Empty)
		{
			EmitBaseline initialBaseline = previousGeneration.InitialBaseline;
			EmitContext sourceContext = new EmitContext(this, null, new DiagnosticBag(), metadataOnly: false, includePrivateMembers: true);
			MetadataDecoder metadataDecoder = (MetadataDecoder)GetOrCreateMetadataSymbols(initialBaseline, sourceAssembly.DeclaringCompilation).MetadataDecoder;
			PEAssemblySymbol otherAssembly = (PEAssemblySymbol)metadataDecoder.ModuleSymbol.ContainingAssembly;
			VisualBasicSymbolMatcher mapToMetadata = new VisualBasicSymbolMatcher(initialBaseline.LazyMetadataSymbols!.AnonymousTypes, sourceAssembly, sourceContext, otherAssembly);
			VisualBasicSymbolMatcher mapToPrevious = null;
			if (previousGeneration.Ordinal > 0)
			{
				SourceAssemblySymbol sourceAssembly2 = ((VisualBasicCompilation)previousGeneration.Compilation).SourceAssembly;
				mapToPrevious = new VisualBasicSymbolMatcher(otherContext: new EmitContext((PEModuleBuilder)previousGeneration.PEModuleBuilder, null, new DiagnosticBag(), metadataOnly: false, includePrivateMembers: true), anonymousTypeMap: previousGeneration.AnonymousTypeMap, sourceAssembly: sourceAssembly, sourceContext: sourceContext, otherAssembly: sourceAssembly2, otherSynthesizedMembersOpt: previousGeneration.SynthesizedMembers);
			}
			_previousDefinitions = new VisualBasicDefinitionMap(edits, metadataDecoder, mapToMetadata, mapToPrevious);
			_previousGeneration = previousGeneration;
			_changes = new VisualBasicSymbolChanges(_previousDefinitions, edits, isAddedSymbol);
			_deepTranslator = new VisualBasicSymbolMatcher.DeepTranslator(sourceAssembly.GetSpecialType(SpecialType.System_Object));
		}

		internal override ITypeReference EncTranslateLocalVariableType(TypeSymbol type, DiagnosticBag diagnostics)
		{
			TypeSymbol typeSymbol = (TypeSymbol)_deepTranslator.Visit(type);
			return Translate(typeSymbol ?? type, null, diagnostics);
		}

		private static EmitBaseline.MetadataSymbols GetOrCreateMetadataSymbols(EmitBaseline initialBaseline, VisualBasicCompilation compilation)
		{
			if (initialBaseline.LazyMetadataSymbols != null)
			{
				return initialBaseline.LazyMetadataSymbols;
			}
			ModuleMetadata originalMetadata = initialBaseline.OriginalMetadata;
			VisualBasicCompilation visualBasicCompilation = compilation.RemoveAllSyntaxTrees();
			ImmutableDictionary<AssemblyIdentity, AssemblyIdentity> assemblyReferenceIdentityMap = null;
			PEAssemblySymbol pEAssemblySymbol = visualBasicCompilation.GetBoundReferenceManager().CreatePEAssemblyForAssemblyMetadata(AssemblyMetadata.Create(originalMetadata), MetadataImportOptions.All, out assemblyReferenceIdentityMap);
			MetadataDecoder metadataDecoder = new MetadataDecoder(pEAssemblySymbol.PrimaryModule);
			IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMapFromMetadata = GetAnonymousTypeMapFromMetadata(originalMetadata.MetadataReader, metadataDecoder);
			EmitBaseline.MetadataSymbols value = new EmitBaseline.MetadataSymbols(anonymousTypeMapFromMetadata, metadataDecoder, assemblyReferenceIdentityMap);
			return InterlockedOperations.Initialize(ref initialBaseline.LazyMetadataSymbols, value);
		}

		internal static IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> GetAnonymousTypeMapFromMetadata(MetadataReader reader, MetadataDecoder metadataDecoder)
		{
			Dictionary<AnonymousTypeKey, AnonymousTypeValue> dictionary = new Dictionary<AnonymousTypeKey, AnonymousTypeValue>();
			foreach (TypeDefinitionHandle typeDefinition2 in reader.TypeDefinitions)
			{
				TypeDefinition typeDefinition = reader.GetTypeDefinition(typeDefinition2);
				if (typeDefinition.Namespace.IsNil && reader.StringComparer.StartsWith(typeDefinition.Name, "VB$Anonymous"))
				{
					string @string = reader.GetString(typeDefinition.Name);
					short arity = 0;
					string name = MetadataHelpers.InferTypeArityAndUnmangleMetadataName(@string, out arity);
					int index = 0;
					if (GeneratedNames.TryParseAnonymousTypeTemplateName("VB$AnonymousType_", name, out index))
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)metadataDecoder.GetTypeOfToken(typeDefinition2);
						AnonymousTypeKey anonymousTypeKey = GetAnonymousTypeKey(namedTypeSymbol);
						AnonymousTypeValue value = new AnonymousTypeValue(name, index, namedTypeSymbol.GetCciAdapter());
						dictionary.Add(anonymousTypeKey, value);
					}
					else if (GeneratedNames.TryParseAnonymousTypeTemplateName("VB$AnonymousDelegate_", name, out index))
					{
						NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)metadataDecoder.GetTypeOfToken(typeDefinition2);
						AnonymousTypeKey anonymousDelegateKey = GetAnonymousDelegateKey(namedTypeSymbol2);
						AnonymousTypeValue value2 = new AnonymousTypeValue(name, index, namedTypeSymbol2.GetCciAdapter());
						dictionary.Add(anonymousDelegateKey, value2);
					}
				}
			}
			return dictionary;
		}

		private static AnonymousTypeKey GetAnonymousTypeKey(NamedTypeSymbol type)
		{
			int length = type.TypeParameters.Length;
			AnonymousTypeKey result;
			if (length == 0)
			{
				result = new AnonymousTypeKey(ImmutableArray<AnonymousTypeKeyField>.Empty);
			}
			else
			{
				AnonymousTypeKeyField[] array = new AnonymousTypeKeyField[length - 1 + 1];
				ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Property)
					{
						PropertySymbol propertySymbol = (PropertySymbol)current;
						TypeSymbol type2 = propertySymbol.Type;
						if (type2.TypeKind == TypeKind.TypeParameter)
						{
							int ordinal = ((TypeParameterSymbol)type2).Ordinal;
							array[ordinal] = new AnonymousTypeKeyField(propertySymbol.Name, propertySymbol.IsReadOnly, ignoreCase: true);
						}
					}
				}
				result = new AnonymousTypeKey(ImmutableArray.Create(array));
			}
			return result;
		}

		private static AnonymousTypeKey GetAnonymousDelegateKey(NamedTypeSymbol type)
		{
			MethodSymbol methodSymbol = (MethodSymbol)type.GetMembers("Invoke")[0];
			ArrayBuilder<AnonymousTypeKeyField> instance = ArrayBuilder<AnonymousTypeKeyField>.GetInstance();
			instance.AddRange(methodSymbol.Parameters.SelectAsArray((ParameterSymbol p) => new AnonymousTypeKeyField(p.Name, p.IsByRef, ignoreCase: true)));
			instance.Add(new AnonymousTypeKeyField(AnonymousTypeDescriptor.GetReturnParameterName(!methodSymbol.IsSub), isKey: false, ignoreCase: true));
			return new AnonymousTypeKey(instance.ToImmutableAndFree(), isDelegate: true);
		}

		internal IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> GetAnonymousTypeMap()
		{
			return Compilation.AnonymousTypeManager.GetAnonymousTypeMap();
		}

		IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> IPEDeltaAssemblyBuilder.GetAnonymousTypeMap()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAnonymousTypeMap
			return this.GetAnonymousTypeMap();
		}

		internal override VariableSlotAllocator TryCreateVariableSlotAllocator(MethodSymbol method, MethodSymbol topLevelMethod, DiagnosticBag diagnostics)
		{
			return _previousDefinitions.TryCreateVariableSlotAllocator(_previousGeneration, Compilation, method, topLevelMethod, diagnostics);
		}

		internal override ImmutableArray<AnonymousTypeKey> GetPreviousAnonymousTypes()
		{
			return ImmutableArray.CreateRange(_previousGeneration.AnonymousTypeMap.Keys);
		}

		internal override int GetNextAnonymousTypeIndex(bool fromDelegates)
		{
			return _previousGeneration.GetNextAnonymousTypeIndex(fromDelegates);
		}

		internal override bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol template, out string name, out int index)
		{
			return _previousDefinitions.TryGetAnonymousTypeName(template, out name, out index);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_25_GetTopLevelTypeDefinitions))]
		public override IEnumerable<INamespaceTypeDefinition> GetTopLevelTypeDefinitions(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_25_GetTopLevelTypeDefinitions(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		public override IEnumerable<INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context)
		{
			return _changes.GetTopLevelSourceTypeDefinitions(context);
		}

		internal void OnCreatedIndices(DiagnosticBag diagnostics)
		{
			EmbeddedTypesManager embeddedTypesManagerOpt = EmbeddedTypesManagerOpt;
			if (embeddedTypesManagerOpt == null)
			{
				return;
			}
			foreach (NamedTypeSymbol key in embeddedTypesManagerOpt.EmbeddedTypesMap.Keys)
			{
				DiagnosticBagExtensions.Add(diagnostics, ErrorFactory.ErrorInfo(ERRID.ERR_EncNoPIAReference, key.AdaptedNamedTypeSymbol), Location.None);
			}
		}

		void IPEDeltaAssemblyBuilder.OnCreatedIndices(DiagnosticBag diagnostics)
		{
			//ILSpy generated this explicit interface implementation from .override directive in OnCreatedIndices
			this.OnCreatedIndices(diagnostics);
		}
	}
}
