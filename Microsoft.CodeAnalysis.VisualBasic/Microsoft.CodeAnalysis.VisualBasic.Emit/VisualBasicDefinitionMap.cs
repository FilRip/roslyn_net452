using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class VisualBasicDefinitionMap : DefinitionMap
	{
		private readonly MetadataDecoder _metadataDecoder;

		private readonly VisualBasicSymbolMatcher _mapToMetadata;

		private readonly VisualBasicSymbolMatcher _mapToPrevious;

		protected override SymbolMatcher MapToMetadataSymbolMatcher => _mapToMetadata;

		protected override SymbolMatcher MapToPreviousSymbolMatcher => _mapToPrevious;

		internal override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance;

		public VisualBasicDefinitionMap(IEnumerable<SemanticEdit> edits, MetadataDecoder metadataDecoder, VisualBasicSymbolMatcher mapToMetadata, VisualBasicSymbolMatcher mapToPrevious)
			: base(edits)
		{
			_metadataDecoder = metadataDecoder;
			_mapToMetadata = mapToMetadata;
			_mapToPrevious = mapToPrevious ?? mapToMetadata;
		}

		protected override ISymbolInternal GetISymbolInternalOrNull(ISymbol symbol)
		{
			return symbol as Symbol;
		}

		protected override LambdaSyntaxFacts GetLambdaSyntaxFacts()
		{
			return VisualBasicLambdaSyntaxFacts.Instance;
		}

		internal bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeOrDelegateTemplateSymbol template, out string name, out int index)
		{
			return _mapToPrevious.TryGetAnonymousTypeName(template, out name, out index);
		}

		internal override bool TryGetTypeHandle(ITypeDefinition def, out TypeDefinitionHandle handle)
		{
			if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PENamedTypeSymbol pENamedTypeSymbol)
			{
				handle = pENamedTypeSymbol.Handle;
				return true;
			}
			handle = default(TypeDefinitionHandle);
			return false;
		}

		internal override bool TryGetEventHandle(IEventDefinition def, out EventDefinitionHandle handle)
		{
			if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEEventSymbol pEEventSymbol)
			{
				handle = pEEventSymbol.Handle;
				return true;
			}
			handle = default(EventDefinitionHandle);
			return false;
		}

		internal override bool TryGetFieldHandle(IFieldDefinition def, out FieldDefinitionHandle handle)
		{
			if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEFieldSymbol pEFieldSymbol)
			{
				handle = pEFieldSymbol.Handle;
				return true;
			}
			handle = default(FieldDefinitionHandle);
			return false;
		}

		internal override bool TryGetMethodHandle(IMethodDefinition def, out MethodDefinitionHandle handle)
		{
			if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEMethodSymbol pEMethodSymbol)
			{
				handle = pEMethodSymbol.Handle;
				return true;
			}
			handle = default(MethodDefinitionHandle);
			return false;
		}

		internal override bool TryGetPropertyHandle(IPropertyDefinition def, out PropertyDefinitionHandle handle)
		{
			if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEPropertySymbol pEPropertySymbol)
			{
				handle = pEPropertySymbol.Handle;
				return true;
			}
			handle = default(PropertyDefinitionHandle);
			return false;
		}

		protected override ITypeSymbolInternal TryGetStateMachineType(EntityHandle methodHandle)
		{
			string value = null;
			if (_metadataDecoder.Module.HasStringValuedAttribute(methodHandle, AttributeDescription.AsyncStateMachineAttribute, out value) || _metadataDecoder.Module.HasStringValuedAttribute(methodHandle, AttributeDescription.IteratorStateMachineAttribute, out value))
			{
				return _metadataDecoder.GetTypeSymbolForSerializedType(value);
			}
			return null;
		}

		protected override void GetStateMachineFieldMapFromMetadata(ITypeSymbolInternal stateMachineType, ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo, out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap, out IReadOnlyDictionary<ITypeReference, int> awaiterMap, out int awaiterSlotCount)
		{
			Dictionary<EncHoistedLocalInfo, int> dictionary = new Dictionary<EncHoistedLocalInfo, int>();
			Dictionary<ITypeReference, int> dictionary2 = new Dictionary<ITypeReference, int>(SymbolEquivalentEqualityComparer.Instance);
			int num = -1;
			ImmutableArray<Symbol>.Enumerator enumerator = ((TypeSymbol)stateMachineType).GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.Field)
				{
					continue;
				}
				string name = current.Name;
				int slotIndex;
				switch (GeneratedNames.GetKind(name))
				{
				case GeneratedNameKind.StateMachineAwaiterField:
					if (GeneratedNames.TryParseSlotIndex("$A", name, out slotIndex))
					{
						FieldSymbol fieldSymbol2 = (FieldSymbol)current;
						dictionary2[(ITypeReference)fieldSymbol2.Type.GetCciAdapter()] = slotIndex;
						if (slotIndex > num)
						{
							num = slotIndex;
						}
					}
					break;
				case GeneratedNameKind.HoistedSynthesizedLocalField:
				case GeneratedNameKind.StateMachineHoistedUserVariableField:
				{
					string variableName = null;
					if (GeneratedNames.TryParseSlotIndex("$S", name, out slotIndex) || GeneratedNames.TryParseStateMachineHoistedUserVariableName(name, out variableName, out slotIndex))
					{
						FieldSymbol fieldSymbol = (FieldSymbol)current;
						if (slotIndex < localSlotDebugInfo.Length)
						{
							EncHoistedLocalInfo key = new EncHoistedLocalInfo(localSlotDebugInfo[slotIndex], (ITypeReference)fieldSymbol.Type.GetCciAdapter());
							dictionary[key] = slotIndex;
						}
					}
					break;
				}
				}
			}
			hoistedLocalMap = dictionary;
			awaiterMap = dictionary2;
			awaiterSlotCount = num + 1;
		}

		protected override ImmutableArray<EncLocalInfo> GetLocalSlotMapFromMetadata(StandaloneSignatureHandle handle, EditAndContinueMethodDebugInformation debugInfo)
		{
			ImmutableArray<LocalInfo<TypeSymbol>> localsOrThrow = _metadataDecoder.GetLocalsOrThrow(handle);
			return CreateLocalSlotMap(debugInfo, localsOrThrow);
		}

		private static ImmutableArray<EncLocalInfo> CreateLocalSlotMap(EditAndContinueMethodDebugInformation methodEncInfo, ImmutableArray<LocalInfo<TypeSymbol>> slotMetadata)
		{
			EncLocalInfo[] array = new EncLocalInfo[slotMetadata.Length - 1 + 1];
			ImmutableArray<LocalSlotDebugInfo> localSlots = methodEncInfo.LocalSlots;
			if (!localSlots.IsDefault)
			{
				int num = Math.Min(localSlots.Length, slotMetadata.Length);
				Dictionary<EncLocalInfo, int> dictionary = new Dictionary<EncLocalInfo, int>();
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					LocalSlotDebugInfo slotInfo = localSlots[i];
					if (slotInfo.SynthesizedKind.IsLongLived())
					{
						LocalInfo<TypeSymbol> localInfo = slotMetadata[i];
						if (localInfo.CustomModifiers.IsDefaultOrEmpty)
						{
							EncLocalInfo key = new EncLocalInfo(slotInfo, (ITypeReference)localInfo.Type.GetCciAdapter(), localInfo.Constraints, localInfo.SignatureOpt);
							dictionary.Add(key, i);
						}
					}
				}
				foreach (KeyValuePair<EncLocalInfo, int> item in dictionary)
				{
					array[item.Value] = item.Key;
				}
			}
			int num3 = array.Length - 1;
			for (int j = 0; j <= num3; j++)
			{
				if (array[j].IsDefault)
				{
					array[j] = new EncLocalInfo(slotMetadata[j].SignatureOpt);
				}
			}
			return ImmutableArray.Create(array);
		}
	}
}
