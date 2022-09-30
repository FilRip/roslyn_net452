using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal struct NativeIntegerTypeDecoder
    {
        private sealed class ErrorTypeException : Exception
        {
        }

        private readonly ImmutableArray<bool> _transformFlags;

        private int _index;

        internal static TypeSymbol TransformType(TypeSymbol type, EntityHandle handle, PEModuleSymbol containingModule)
        {
            if (!containingModule.Module.HasNativeIntegerAttribute(handle, out var transformFlags))
            {
                return type;
            }
            return TransformType(type, transformFlags);
        }

        internal static TypeSymbol TransformType(TypeSymbol type, ImmutableArray<bool> transformFlags)
        {
            NativeIntegerTypeDecoder nativeIntegerTypeDecoder = new NativeIntegerTypeDecoder(transformFlags);
            try
            {
                TypeSymbol result = nativeIntegerTypeDecoder.TransformType(type);
                if (nativeIntegerTypeDecoder._index == transformFlags.Length)
                {
                    return result;
                }
                return new UnsupportedMetadataTypeSymbol();
            }
            catch (UnsupportedSignatureContent)
            {
                return new UnsupportedMetadataTypeSymbol();
            }
            catch (ErrorTypeException)
            {
                return type;
            }
        }

        private NativeIntegerTypeDecoder(ImmutableArray<bool> transformFlags)
        {
            _transformFlags = transformFlags;
            _index = 0;
        }

        private TypeWithAnnotations TransformTypeWithAnnotations(TypeWithAnnotations type)
        {
            return type.WithTypeAndModifiers(TransformType(type.Type), type.CustomModifiers);
        }

        private TypeSymbol TransformType(TypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Array:
                    return TransformArrayType((ArrayTypeSymbol)type);
                case TypeKind.Pointer:
                    return TransformPointerType((PointerTypeSymbol)type);
                case TypeKind.FunctionPointer:
                    return TransformFunctionPointerType((FunctionPointerTypeSymbol)type);
                case TypeKind.Dynamic:
                case TypeKind.TypeParameter:
                    return type;
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    return TransformNamedType((NamedTypeSymbol)type);
                default:
                    throw new ErrorTypeException();
            }
        }

        private NamedTypeSymbol TransformNamedType(NamedTypeSymbol type)
        {
            if (!type.IsGenericType)
            {
                SpecialType specialType = type.SpecialType;
                if ((uint)(specialType - 21) <= 1u)
                {
                    if (_index >= _transformFlags.Length)
                    {
                        throw new UnsupportedSignatureContent();
                    }
                    bool num = _transformFlags[_index++];
                    bool isNativeIntegerType = type.IsNativeIntegerType;
                    if (!num)
                    {
                        if (isNativeIntegerType)
                        {
                            return type.NativeIntegerUnderlyingType;
                        }
                    }
                    else if (!isNativeIntegerType)
                    {
                        return type.AsNativeInteger();
                    }
                    return type;
                }
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            type.GetAllTypeArgumentsNoUseSiteDiagnostics(instance);
            bool flag = false;
            for (int i = 0; i < instance.Count; i++)
            {
                TypeWithAnnotations type2 = instance[i];
                TypeWithAnnotations typeWithAnnotations = TransformTypeWithAnnotations(type2);
                if (!type2.IsSameAs(typeWithAnnotations))
                {
                    instance[i] = typeWithAnnotations;
                    flag = true;
                }
            }
            NamedTypeSymbol result = (flag ? type.WithTypeArguments(instance.ToImmutable()) : type);
            instance.Free();
            return result;
        }

        private ArrayTypeSymbol TransformArrayType(ArrayTypeSymbol type)
        {
            return type.WithElementType(TransformTypeWithAnnotations(type.ElementTypeWithAnnotations));
        }

        private PointerTypeSymbol TransformPointerType(PointerTypeSymbol type)
        {
            return type.WithPointedAtType(TransformTypeWithAnnotations(type.PointedAtTypeWithAnnotations));
        }

        private FunctionPointerTypeSymbol TransformFunctionPointerType(FunctionPointerTypeSymbol type)
        {
            TypeWithAnnotations substitutedReturnType = TransformTypeWithAnnotations(type.Signature.ReturnTypeWithAnnotations);
            ImmutableArray<TypeWithAnnotations> substitutedParameterTypes = ImmutableArray<TypeWithAnnotations>.Empty;
            bool flag = false;
            if (type.Signature.ParameterCount > 0)
            {
                ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(type.Signature.ParameterCount);
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = type.Signature.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    TypeWithAnnotations item = TransformTypeWithAnnotations(current.TypeWithAnnotations);
                    flag = flag || !item.IsSameAs(current.TypeWithAnnotations);
                    instance.Add(item);
                }
                if (flag)
                {
                    substitutedParameterTypes = instance.ToImmutableAndFree();
                }
                else
                {
                    substitutedParameterTypes = type.Signature.ParameterTypesWithAnnotations;
                    instance.Free();
                }
            }
            if (flag || !substitutedReturnType.IsSameAs(type.Signature.ReturnTypeWithAnnotations))
            {
                return type.SubstituteTypeSymbol(substitutedReturnType, substitutedParameterTypes, default(ImmutableArray<CustomModifier>), default(ImmutableArray<ImmutableArray<CustomModifier>>));
            }
            return type;
        }
    }
}
