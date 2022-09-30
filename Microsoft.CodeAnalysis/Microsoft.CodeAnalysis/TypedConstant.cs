using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct TypedConstant : IEquatable<TypedConstant>
    {
        private readonly TypedConstantKind _kind;

        private readonly ITypeSymbolInternal? _type;

        private readonly object? _value;

        public TypedConstantKind Kind => _kind;

        public ITypeSymbol? Type => _type?.GetITypeSymbol();

        public ITypeSymbolInternal? TypeInternal => _type;

        public bool IsNull => _value == null;

        public object? Value
        {
            get
            {
                object valueInternal = ValueInternal;
                if (valueInternal is ISymbolInternal symbolInternal)
                {
                    return symbolInternal.GetISymbol();
                }
                return valueInternal;
            }
        }

        public object? ValueInternal
        {
            get
            {
                if (Kind == TypedConstantKind.Array)
                {
                    throw new InvalidOperationException("TypedConstant is an array. Use Values property.");
                }
                return _value;
            }
        }

        public ImmutableArray<TypedConstant> Values
        {
            get
            {
                if (Kind != TypedConstantKind.Array)
                {
                    throw new InvalidOperationException("TypedConstant is not an array. Use Value property.");
                }
                if (IsNull)
                {
                    return default(ImmutableArray<TypedConstant>);
                }
                return (ImmutableArray<TypedConstant>)_value;
            }
        }

        public TypedConstant(ITypeSymbolInternal? type, TypedConstantKind kind, object? value)
        {
            _kind = kind;
            _type = type;
            _value = value;
        }

        public TypedConstant(ITypeSymbolInternal type, ImmutableArray<TypedConstant> array)
            : this(type, TypedConstantKind.Array, array.IsDefault ? null : array)
        {
        }

        public T? DecodeValue<T>(SpecialType specialType)
        {
            TryDecodeValue<T>(specialType, out var value);
            return value;
        }

        public bool TryDecodeValue<T>(SpecialType specialType, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T value)
        {
            if (_kind == TypedConstantKind.Error)
            {
                value = default(T);
                return false;
            }
            if (_type!.SpecialType == specialType || (_type!.TypeKind == TypeKind.Enum && specialType == SpecialType.System_Enum))
            {
                value = (T)_value!;
                return true;
            }
            value = default(T);
            return false;
        }

        public static TypedConstantKind GetTypedConstantKind(ITypeSymbolInternal type, Compilation compilation)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return TypedConstantKind.Primitive;
                default:
                    switch (type.TypeKind)
                    {
                        case TypeKind.Array:
                            return TypedConstantKind.Array;
                        case TypeKind.Enum:
                            return TypedConstantKind.Enum;
                        case TypeKind.Error:
                            return TypedConstantKind.Error;
                        default:
                            if (compilation != null && compilation.IsSystemTypeReference(type))
                            {
                                return TypedConstantKind.Type;
                            }
                            return TypedConstantKind.Error;
                    }
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypedConstant)
            {
                return Equals((TypedConstant)obj);
            }
            return false;
        }

        public bool Equals(TypedConstant other)
        {
            if (_kind == other._kind && object.Equals(_value, other._value))
            {
                return object.Equals(_type, other._type);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_value, Hash.Combine(_type, (int)Kind));
        }
    }
}
