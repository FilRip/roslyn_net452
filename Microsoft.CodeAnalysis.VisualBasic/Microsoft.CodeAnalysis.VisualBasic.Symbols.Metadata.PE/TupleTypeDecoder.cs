using System.Collections.Immutable;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal struct TupleTypeDecoder
	{
		private readonly ImmutableArray<string> _elementNames;

		private int _namesIndex;

		private bool _foundUsableErrorType;

		private bool _decodingFailed;

		private TupleTypeDecoder(ImmutableArray<string> elementNames)
		{
			this = default(TupleTypeDecoder);
			_elementNames = elementNames;
			_namesIndex = ((!elementNames.IsDefault) ? elementNames.Length : 0);
			_foundUsableErrorType = false;
			_decodingFailed = false;
		}

		public static TypeSymbol DecodeTupleTypesIfApplicable(TypeSymbol metadataType, EntityHandle targetSymbolToken, PEModuleSymbol containingModule)
		{
			ImmutableArray<string> tupleElementNames = default(ImmutableArray<string>);
			bool flag = containingModule.Module.HasTupleElementNamesAttribute(targetSymbolToken, out tupleElementNames);
			if (flag && tupleElementNames.IsDefaultOrEmpty)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			return DecodeTupleTypesInternal(metadataType, tupleElementNames, flag);
		}

		public static TypeSymbol DecodeTupleTypesIfApplicable(TypeSymbol metadataType, ImmutableArray<string> elementNames)
		{
			return DecodeTupleTypesInternal(metadataType, elementNames, !elementNames.IsDefaultOrEmpty);
		}

		private static TypeSymbol DecodeTupleTypesInternal(TypeSymbol metadataType, ImmutableArray<string> elementNames, bool hasTupleElementNamesAttribute)
		{
			TupleTypeDecoder tupleTypeDecoder = new TupleTypeDecoder(elementNames);
			TypeSymbol result = tupleTypeDecoder.DecodeType(metadataType);
			if (!tupleTypeDecoder._decodingFailed && (!hasTupleElementNamesAttribute || tupleTypeDecoder._namesIndex == 0))
			{
				return result;
			}
			if (tupleTypeDecoder._foundUsableErrorType)
			{
				return metadataType;
			}
			return new UnsupportedMetadataTypeSymbol();
		}

		private TypeSymbol DecodeType(TypeSymbol type)
		{
			switch (type.Kind)
			{
			case SymbolKind.ErrorType:
				_foundUsableErrorType = true;
				return type;
			case SymbolKind.DynamicType:
			case SymbolKind.PointerType:
			case SymbolKind.TypeParameter:
				return type;
			case SymbolKind.NamedType:
				return type.IsTupleType ? DecodeNamedType(type.TupleUnderlyingType) : DecodeNamedType((NamedTypeSymbol)type);
			case SymbolKind.ArrayType:
				return DecodeArrayType((ArrayTypeSymbol)type);
			default:
				throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
			}
		}

		private NamedTypeSymbol DecodeNamedType(NamedTypeSymbol type)
		{
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = type.TypeArgumentsNoUseSiteDiagnostics;
			ImmutableArray<TypeSymbol> immutableArray = DecodeTypeArguments(typeArgumentsNoUseSiteDiagnostics);
			NamedTypeSymbol namedTypeSymbol = type;
			NamedTypeSymbol containingType = type.ContainingType;
			NamedTypeSymbol namedTypeSymbol2 = null;
			namedTypeSymbol2 = (((object)containingType == null || !containingType.IsGenericType) ? containingType : DecodeNamedType(containingType));
			bool flag = (object)namedTypeSymbol2 != containingType;
			if (typeArgumentsNoUseSiteDiagnostics != immutableArray || flag)
			{
				ImmutableArray<TypeWithModifiers> newTypeArgs = (type.HasTypeArgumentsCustomModifiers ? immutableArray.SelectAsArray((TypeSymbol t, int i, NamedTypeSymbol m) => new TypeWithModifiers(t, m.GetTypeArgumentCustomModifiers(i)), type) : immutableArray.SelectAsArray((TypeSymbol t) => new TypeWithModifiers(t, default(ImmutableArray<CustomModifier>))));
				if (flag)
				{
					namedTypeSymbol = SymbolExtensions.AsMember(namedTypeSymbol.OriginalDefinition, namedTypeSymbol2);
					return namedTypeSymbol.TypeParameters.IsEmpty ? namedTypeSymbol : Construct(namedTypeSymbol, newTypeArgs);
				}
				namedTypeSymbol = Construct(type, newTypeArgs);
			}
			if (namedTypeSymbol.IsTupleCompatible(out var tupleCardinality))
			{
				ImmutableArray<string> elementNames = EatElementNamesIfAvailable(tupleCardinality);
				namedTypeSymbol = TupleTypeSymbol.Create(namedTypeSymbol, elementNames);
			}
			return namedTypeSymbol;
		}

		private static NamedTypeSymbol Construct(NamedTypeSymbol type, ImmutableArray<TypeWithModifiers> newTypeArgs)
		{
			NamedTypeSymbol originalDefinition = type.OriginalDefinition;
			TypeSubstitution typeSubstitution = type.ConstructedFrom.ContainingType?.TypeSubstitution;
			TypeSubstitution substitution = ((typeSubstitution == null) ? TypeSubstitution.Create(originalDefinition, originalDefinition.TypeParameters, newTypeArgs) : TypeSubstitution.Create(typeSubstitution, originalDefinition, newTypeArgs));
			return originalDefinition.Construct(substitution);
		}

		private ImmutableArray<TypeSymbol> DecodeTypeArguments(ImmutableArray<TypeSymbol> typeArgs)
		{
			if (typeArgs.IsEmpty)
			{
				return typeArgs;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(typeArgs.Length);
			bool flag = false;
			for (int i = typeArgs.Length - 1; i >= 0; i += -1)
			{
				TypeSymbol typeSymbol = typeArgs[i];
				TypeSymbol typeSymbol2 = DecodeType(typeSymbol);
				flag = flag || (object)typeSymbol2 != typeSymbol;
				instance.Add(typeSymbol2);
			}
			if (!flag)
			{
				instance.Free();
				return typeArgs;
			}
			instance.ReverseContents();
			return instance.ToImmutableAndFree();
		}

		private ArrayTypeSymbol DecodeArrayType(ArrayTypeSymbol type)
		{
			TypeSymbol typeSymbol = DecodeType(type.ElementType);
			if ((object)typeSymbol != type.ElementType)
			{
				return type.WithElementType(typeSymbol);
			}
			return type;
		}

		private ImmutableArray<string> EatElementNamesIfAvailable(int numberOfElements)
		{
			if (_elementNames.IsDefault)
			{
				return _elementNames;
			}
			ImmutableArray<string> result;
			if (numberOfElements > _namesIndex)
			{
				_namesIndex = 0;
				_decodingFailed = true;
				result = default(ImmutableArray<string>);
			}
			else
			{
				int num = _namesIndex - numberOfElements;
				bool flag = true;
				_namesIndex = num;
				int num2 = numberOfElements - 1;
				for (int i = 0; i <= num2; i++)
				{
					if (_elementNames[num + i] != null)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance(numberOfElements);
					int num3 = numberOfElements - 1;
					for (int j = 0; j <= num3; j++)
					{
						instance.Add(_elementNames[num + j]);
					}
					return instance.ToImmutableAndFree();
				}
				result = default(ImmutableArray<string>);
			}
			return result;
		}
	}
}
