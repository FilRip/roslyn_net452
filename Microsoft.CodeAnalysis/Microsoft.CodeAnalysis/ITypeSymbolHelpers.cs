#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class ITypeSymbolHelpers
    {
        public static bool IsNullableType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? typeOpt)
        {
            if (typeOpt == null)
            {
                return false;
            }
            return typeOpt!.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
        }

        public static bool IsNullableOfBoolean([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            if (IsNullableType(type))
            {
                return IsBooleanType(GetNullableUnderlyingType(type));
            }
            return false;
        }

        public static ITypeSymbol GetNullableUnderlyingType(ITypeSymbol type)
        {
            return ((INamedTypeSymbol)type).TypeArguments[0];
        }

        public static bool IsBooleanType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            if (type == null)
            {
                return false;
            }
            return type!.SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsObjectType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            if (type == null)
            {
                return false;
            }
            return type!.SpecialType == SpecialType.System_Object;
        }

        public static bool IsSignedIntegralType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            return type?.SpecialType.IsSignedIntegralType() ?? false;
        }

        public static bool IsUnsignedIntegralType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            return type?.SpecialType.IsUnsignedIntegralType() ?? false;
        }

        public static bool IsNumericType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            return type?.SpecialType.IsNumericType() ?? false;
        }

        public static ITypeSymbol? GetEnumUnderlyingType(ITypeSymbol? type)
        {
            return (type as INamedTypeSymbol)?.EnumUnderlyingType;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("type")]
        public static ITypeSymbol? GetEnumUnderlyingTypeOrSelf(ITypeSymbol? type)
        {
            return GetEnumUnderlyingType(type) ?? type;
        }

        public static bool IsDynamicType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
        {
            if (type == null)
            {
                return false;
            }
            return type!.Kind == SymbolKind.DynamicType;
        }
    }
}
