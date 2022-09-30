using System.Collections.Immutable;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal struct TupleTypeDecoder
    {
        private readonly ImmutableArray<string?> _elementNames;

        private int _namesIndex;

        private bool _foundUsableErrorType;

        private bool _decodingFailed;

        private TupleTypeDecoder(ImmutableArray<string?> elementNames)
        {
            _elementNames = elementNames;
            _namesIndex = ((!elementNames.IsDefault) ? elementNames.Length : 0);
            _decodingFailed = false;
            _foundUsableErrorType = false;
        }

        public static TypeSymbol DecodeTupleTypesIfApplicable(TypeSymbol metadataType, EntityHandle targetHandle, PEModuleSymbol containingModule)
        {
            bool flag = containingModule.Module.HasTupleElementNamesAttribute(targetHandle, out ImmutableArray<string> tupleElementNames);
            if (flag && tupleElementNames.IsDefaultOrEmpty)
            {
                return new UnsupportedMetadataTypeSymbol();
            }
            return DecodeTupleTypesInternal(metadataType, tupleElementNames, flag);
        }

        public static TypeWithAnnotations DecodeTupleTypesIfApplicable(TypeWithAnnotations metadataType, EntityHandle targetHandle, PEModuleSymbol containingModule)
        {
            bool flag = containingModule.Module.HasTupleElementNamesAttribute(targetHandle, out ImmutableArray<string> tupleElementNames);
            if (flag && tupleElementNames.IsDefaultOrEmpty)
            {
                return TypeWithAnnotations.Create(new UnsupportedMetadataTypeSymbol());
            }
            TypeSymbol type = metadataType.Type;
            TypeSymbol typeSymbol = DecodeTupleTypesInternal(type, tupleElementNames, flag);
            if ((object)typeSymbol != type)
            {
                return TypeWithAnnotations.Create(typeSymbol, metadataType.NullableAnnotation, metadataType.CustomModifiers);
            }
            return metadataType;
        }

        public static TypeSymbol DecodeTupleTypesIfApplicable(TypeSymbol metadataType, ImmutableArray<string?> elementNames)
        {
            return DecodeTupleTypesInternal(metadataType, elementNames, !elementNames.IsDefaultOrEmpty);
        }

        private static TypeSymbol DecodeTupleTypesInternal(TypeSymbol metadataType, ImmutableArray<string?> elementNames, bool hasTupleElementNamesAttribute)
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
                case SymbolKind.TypeParameter:
                    return type;
                case SymbolKind.FunctionPointerType:
                    return DecodeFunctionPointerType((FunctionPointerTypeSymbol)type);
                case SymbolKind.PointerType:
                    return DecodePointerType((PointerTypeSymbol)type);
                case SymbolKind.NamedType:
                    return DecodeNamedType((NamedTypeSymbol)type);
                case SymbolKind.ArrayType:
                    return DecodeArrayType((ArrayTypeSymbol)type);
                default:
                    throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
            }
        }

        private PointerTypeSymbol DecodePointerType(PointerTypeSymbol type)
        {
            return type.WithPointedAtType(DecodeTypeInternal(type.PointedAtTypeWithAnnotations));
        }

        private FunctionPointerTypeSymbol DecodeFunctionPointerType(FunctionPointerTypeSymbol type)
        {
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            bool flag = false;
            if (type.Signature.ParameterCount > 0)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(type.Signature.ParameterCount);
                for (int num = type.Signature.ParameterCount - 1; num >= 0; num--)
                {
                    ParameterSymbol parameterSymbol = type.Signature.Parameters[num];
                    TypeWithAnnotations item = DecodeTypeInternal(parameterSymbol.TypeWithAnnotations);
                    flag = flag || !item.IsSameAs(parameterSymbol.TypeWithAnnotations);
                    instance.Add(item);
                }
                if (flag)
                {
                    instance.ReverseContents();
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                }
                else
                {
                    substitutedParameterTypes = type.Signature.ParameterTypesWithAnnotations;
                    instance.Free();
                }
            }
            TypeWithAnnotations substitutedReturnType = DecodeTypeInternal(type.Signature.ReturnTypeWithAnnotations);
            if (flag || !substitutedReturnType.IsSameAs(type.Signature.ReturnTypeWithAnnotations))
            {
                return type.SubstituteTypeSymbol(substitutedReturnType, substitutedParameterTypes, default(ImmutableArray<CustomModifier>), default(ImmutableArray<ImmutableArray<CustomModifier>>));
            }
            return type;
        }

        private NamedTypeSymbol DecodeNamedType(NamedTypeSymbol type)
        {
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            ImmutableArray<TypeWithAnnotations> immutableArray = DecodeTypeArguments(typeArgumentsWithAnnotationsNoUseSiteDiagnostics);
            NamedTypeSymbol namedTypeSymbol = type;
            NamedTypeSymbol containingType = type.ContainingType;
            NamedTypeSymbol namedTypeSymbol2 = (((object)containingType == null || !containingType.IsGenericType) ? containingType : DecodeNamedType(containingType));
            bool flag = (object)namedTypeSymbol2 != containingType;
            if (typeArgumentsWithAnnotationsNoUseSiteDiagnostics != immutableArray || flag)
            {
                if (flag)
                {
                    namedTypeSymbol = namedTypeSymbol.OriginalDefinition.AsMember(namedTypeSymbol2);
                    return namedTypeSymbol.ConstructIfGeneric(immutableArray);
                }
                namedTypeSymbol = type.ConstructedFrom.Construct(immutableArray, unbound: false);
            }
            if (namedTypeSymbol.IsTupleType)
            {
                int length = namedTypeSymbol.TupleElementTypesWithAnnotations.Length;
                if (length > 0)
                {
                    ImmutableArray<string> elementNames = EatElementNamesIfAvailable(length);
                    namedTypeSymbol = NamedTypeSymbol.CreateTuple(namedTypeSymbol, elementNames);
                }
            }
            return namedTypeSymbol;
        }

        private ImmutableArray<TypeWithAnnotations> DecodeTypeArguments(ImmutableArray<TypeWithAnnotations> typeArgs)
        {
            if (typeArgs.IsEmpty)
            {
                return typeArgs;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(typeArgs.Length);
            bool flag = false;
            for (int num = typeArgs.Length - 1; num >= 0; num--)
            {
                TypeWithAnnotations typeWithAnnotations = typeArgs[num];
                TypeWithAnnotations item = DecodeTypeInternal(typeWithAnnotations);
                flag |= !item.IsSameAs(typeWithAnnotations);
                instance.Add(item);
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
            TypeWithAnnotations elementTypeWithAnnotations = DecodeTypeInternal(type.ElementTypeWithAnnotations);
            return type.WithElementType(elementTypeWithAnnotations);
        }

        private TypeWithAnnotations DecodeTypeInternal(TypeWithAnnotations typeWithAnnotations)
        {
            TypeSymbol type = typeWithAnnotations.Type;
            TypeSymbol typeSymbol = DecodeType(type);
            if ((object)typeSymbol != type)
            {
                return TypeWithAnnotations.Create(typeSymbol, typeWithAnnotations.NullableAnnotation, typeWithAnnotations.CustomModifiers);
            }
            return typeWithAnnotations;
        }

        private ImmutableArray<string?> EatElementNamesIfAvailable(int numberOfElements)
        {
            if (_elementNames.IsDefault)
            {
                return _elementNames;
            }
            if (numberOfElements > _namesIndex)
            {
                _namesIndex = 0;
                _decodingFailed = true;
                return default(ImmutableArray<string>);
            }
            int num = (_namesIndex -= numberOfElements);
            bool flag = true;
            for (int i = 0; i < numberOfElements; i++)
            {
                if (_elementNames[num + i] != null)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                return default(ImmutableArray<string>);
            }
            return ImmutableArray.Create(_elementNames, num, numberOfElements);
        }
    }
}
