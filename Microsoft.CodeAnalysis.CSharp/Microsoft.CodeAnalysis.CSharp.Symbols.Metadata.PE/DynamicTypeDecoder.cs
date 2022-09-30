using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal struct DynamicTypeDecoder
    {
        private readonly ImmutableArray<bool> _dynamicTransformFlags;

        private readonly AssemblySymbol _containingAssembly;

        private readonly bool _haveCustomModifierFlags;

        private readonly bool _checkLength;

        private int _index;

        private bool HasFlag
        {
            get
            {
                if (_index >= _dynamicTransformFlags.Length)
                {
                    return !_checkLength;
                }
                return true;
            }
        }

        private DynamicTypeDecoder(ImmutableArray<bool> dynamicTransformFlags, bool haveCustomModifierFlags, bool checkLength, AssemblySymbol containingAssembly)
        {
            _dynamicTransformFlags = dynamicTransformFlags;
            _containingAssembly = containingAssembly;
            _haveCustomModifierFlags = haveCustomModifierFlags;
            _checkLength = checkLength;
            _index = 0;
        }

        internal static TypeSymbol TransformType(TypeSymbol metadataType, int targetSymbolCustomModifierCount, EntityHandle targetSymbolToken, PEModuleSymbol containingModule, RefKind targetSymbolRefKind = RefKind.None)
        {
            if (containingModule.Module.HasDynamicAttribute(targetSymbolToken, out var transformFlags))
            {
                return TransformTypeInternal(metadataType, containingModule.ContainingAssembly, targetSymbolCustomModifierCount, targetSymbolRefKind, transformFlags, haveCustomModifierFlags: true, checkLength: true);
            }
            return metadataType;
        }

        internal static TypeSymbol TransformTypeWithoutCustomModifierFlags(TypeSymbol type, AssemblySymbol containingAssembly, RefKind targetSymbolRefKind, ImmutableArray<bool> dynamicTransformFlags, bool checkLength = true)
        {
            return TransformTypeInternal(type, containingAssembly, 0, targetSymbolRefKind, dynamicTransformFlags, haveCustomModifierFlags: false, checkLength);
        }

        private static TypeSymbol TransformTypeInternal(TypeSymbol metadataType, AssemblySymbol containingAssembly, int targetSymbolCustomModifierCount, RefKind targetSymbolRefKind, ImmutableArray<bool> dynamicTransformFlags, bool haveCustomModifierFlags, bool checkLength)
        {
            if (dynamicTransformFlags.Length == 0)
            {
                return new UnsupportedMetadataTypeSymbol();
            }
            DynamicTypeDecoder dynamicTypeDecoder = new DynamicTypeDecoder(dynamicTransformFlags, haveCustomModifierFlags, checkLength, containingAssembly);
            if (dynamicTypeDecoder.HandleCustomModifiers(targetSymbolCustomModifierCount) && dynamicTypeDecoder.HandleRefKind(targetSymbolRefKind))
            {
                TypeSymbol typeSymbol = dynamicTypeDecoder.TransformType(metadataType);
                if ((object)typeSymbol != null && (!checkLength || dynamicTypeDecoder._index == dynamicTransformFlags.Length))
                {
                    return typeSymbol;
                }
            }
            return metadataType;
        }

        private TypeSymbol TransformType(TypeSymbol type)
        {
            if (!HasFlag || (PeekFlag() && type.SpecialType != SpecialType.System_Object && !type.IsDynamic()))
            {
                return null;
            }
            switch (type.Kind)
            {
                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    if (type.SpecialType == SpecialType.System_Object)
                    {
                        if (!ConsumeFlag())
                        {
                            return type;
                        }
                        return DynamicTypeSymbol.Instance;
                    }
                    return TransformNamedType((NamedTypeSymbol)type);
                case SymbolKind.ArrayType:
                    return TransformArrayType((ArrayTypeSymbol)type);
                case SymbolKind.PointerType:
                    return TransformPointerType((PointerTypeSymbol)type);
                case SymbolKind.FunctionPointerType:
                    return TransformFunctionPointerType((FunctionPointerTypeSymbol)type);
                case SymbolKind.DynamicType:
                    if (!ConsumeFlag())
                    {
                        return _containingAssembly.GetSpecialType(SpecialType.System_Object);
                    }
                    return type;
                default:
                    ConsumeFlag();
                    if (!HandleCustomModifiers(type.CustomModifierCount()))
                    {
                        return null;
                    }
                    return type;
            }
        }

        private bool HandleCustomModifiers(int customModifiersCount)
        {
            if (!_haveCustomModifierFlags)
            {
                return true;
            }
            for (int i = 0; i < customModifiersCount; i++)
            {
                if (!HasFlag || ConsumeFlag())
                {
                    return false;
                }
            }
            return true;
        }

        private bool HandleRefKind(RefKind refKind)
        {
            if (refKind != 0)
            {
                return !ConsumeFlag();
            }
            return true;
        }

        private NamedTypeSymbol TransformNamedType(NamedTypeSymbol namedType, bool isContaining = false)
        {
            if (!isContaining)
            {
                ConsumeFlag();
            }
            NamedTypeSymbol containingType = namedType.ContainingType;
            NamedTypeSymbol namedTypeSymbol;
            if ((object)containingType != null && containingType.IsGenericType)
            {
                namedTypeSymbol = TransformNamedType(namedType.ContainingType, isContaining: true);
                if ((object)namedTypeSymbol == null)
                {
                    return null;
                }
            }
            else
            {
                namedTypeSymbol = containingType;
            }
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = namedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            ImmutableArray<TypeWithAnnotations> immutableArray = TransformTypeArguments(typeArgumentsWithAnnotationsNoUseSiteDiagnostics);
            if (immutableArray.IsDefault)
            {
                return null;
            }
            bool flag = !TypeSymbol.Equals(namedTypeSymbol, containingType, TypeCompareKind.ConsiderEverything);
            if (flag || immutableArray != typeArgumentsWithAnnotationsNoUseSiteDiagnostics)
            {
                if (flag)
                {
                    namedType = namedType.OriginalDefinition.AsMember(namedTypeSymbol);
                    return namedType.ConstructIfGeneric(immutableArray);
                }
                return namedType.ConstructedFrom.Construct(immutableArray, unbound: false).WithTupleDataFrom(namedType);
            }
            return namedType;
        }

        private ImmutableArray<TypeWithAnnotations> TransformTypeArguments(ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            if (!typeArguments.Any())
            {
                return typeArguments;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            bool flag = false;
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations current = enumerator.Current;
                TypeSymbol typeSymbol = TransformType(current.Type);
                if ((object)typeSymbol == null)
                {
                    instance.Free();
                    return default(ImmutableArray<TypeWithAnnotations>);
                }
                instance.Add(current.WithTypeAndModifiers(typeSymbol, current.CustomModifiers));
                flag |= !TypeSymbol.Equals(typeSymbol, current.Type, TypeCompareKind.ConsiderEverything);
            }
            if (!flag)
            {
                instance.Free();
                return typeArguments;
            }
            return instance.ToImmutableAndFree();
        }

        private ArrayTypeSymbol TransformArrayType(ArrayTypeSymbol arrayType)
        {
            ConsumeFlag();
            if (!HandleCustomModifiers(arrayType.ElementTypeWithAnnotations.CustomModifiers.Length))
            {
                return null;
            }
            TypeSymbol typeSymbol = TransformType(arrayType.ElementType);
            if ((object)typeSymbol == null)
            {
                return null;
            }
            if (!TypeSymbol.Equals(typeSymbol, arrayType.ElementType, TypeCompareKind.ConsiderEverything))
            {
                if (!arrayType.IsSZArray)
                {
                    return ArrayTypeSymbol.CreateMDArray(_containingAssembly, arrayType.ElementTypeWithAnnotations.WithTypeAndModifiers(typeSymbol, arrayType.ElementTypeWithAnnotations.CustomModifiers), arrayType.Rank, arrayType.Sizes, arrayType.LowerBounds);
                }
                return ArrayTypeSymbol.CreateSZArray(_containingAssembly, arrayType.ElementTypeWithAnnotations.WithTypeAndModifiers(typeSymbol, arrayType.ElementTypeWithAnnotations.CustomModifiers));
            }
            return arrayType;
        }

        private PointerTypeSymbol TransformPointerType(PointerTypeSymbol pointerType)
        {
            ConsumeFlag();
            if (!HandleCustomModifiers(pointerType.PointedAtTypeWithAnnotations.CustomModifiers.Length))
            {
                return null;
            }
            TypeSymbol typeSymbol = TransformType(pointerType.PointedAtType);
            if ((object)typeSymbol == null)
            {
                return null;
            }
            if (!TypeSymbol.Equals(typeSymbol, pointerType.PointedAtType, TypeCompareKind.ConsiderEverything))
            {
                return new PointerTypeSymbol(pointerType.PointedAtTypeWithAnnotations.WithTypeAndModifiers(typeSymbol, pointerType.PointedAtTypeWithAnnotations.CustomModifiers));
            }
            return pointerType;
        }

        private FunctionPointerTypeSymbol? TransformFunctionPointerType(FunctionPointerTypeSymbol type)
        {
            ConsumeFlag();
            FunctionPointerMethodSymbol signature = type.Signature;
            var (substitutedReturnType, flag) = handle(ref this, signature.RefKind, signature.RefCustomModifiers, signature.ReturnTypeWithAnnotations);
            if (substitutedReturnType.IsDefault)
            {
                return null;
            }
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            if (signature.ParameterCount > 0)
            {
                bool flag2 = false;
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(signature.ParameterCount);
                try
                {
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator = signature.Parameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ParameterSymbol current = enumerator.Current;
                        var (item, flag3) = handle(ref this, current.RefKind, current.RefCustomModifiers, current.TypeWithAnnotations);
                        if (item.IsDefault)
                        {
                            return null;
                        }
                        instance.Add(item);
                        flag2 = flag2 || flag3;
                    }
                    substitutedParameterTypes = (flag2 ? instance.ToImmutable() : signature.ParameterTypesWithAnnotations);
                    flag = flag || flag2;
                }
                finally
                {
                    instance.Free();
                }
            }
            if (flag)
            {
                return type.SubstituteTypeSymbol(substitutedReturnType, substitutedParameterTypes, default(ImmutableArray<CustomModifier>), default(ImmutableArray<ImmutableArray<CustomModifier>>));
            }
            return type;
            static (TypeWithAnnotations, bool madeChanges) handle(ref DynamicTypeDecoder decoder, RefKind refKind, ImmutableArray<CustomModifier> refCustomModifiers, TypeWithAnnotations typeWithAnnotations)
            {
                if (!decoder.HandleCustomModifiers(refCustomModifiers.Length) || !decoder.HandleRefKind(refKind) || !decoder.HandleCustomModifiers(typeWithAnnotations.CustomModifiers.Length))
                {
                    return (default(TypeWithAnnotations), false);
                }
                TypeSymbol typeSymbol = decoder.TransformType(typeWithAnnotations.Type);
                if ((object)typeSymbol == null)
                {
                    return (default(TypeWithAnnotations), false);
                }
                if (typeSymbol.Equals(typeWithAnnotations.Type, TypeCompareKind.ConsiderEverything))
                {
                    return (typeWithAnnotations, false);
                }
                return (typeWithAnnotations.WithType(typeSymbol), true);
            }
        }

        private bool PeekFlag()
        {
            if (_index < _dynamicTransformFlags.Length)
            {
                return _dynamicTransformFlags[_index];
            }
            return false;
        }

        private bool ConsumeFlag()
        {
            bool result = PeekFlag();
            _index++;
            return result;
        }
    }
}
