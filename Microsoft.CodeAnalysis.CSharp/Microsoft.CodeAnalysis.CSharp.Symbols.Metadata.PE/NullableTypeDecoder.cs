using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal static class NullableTypeDecoder
    {
        internal static TypeWithAnnotations TransformType(TypeWithAnnotations metadataType, EntityHandle targetSymbolToken, PEModuleSymbol containingModule, Symbol accessSymbol, Symbol nullableContext)
        {
            if (!containingModule.Module.HasNullableAttribute(targetSymbolToken, out var defaultTransform, out var nullableTransforms))
            {
                byte? nullableContextValue = nullableContext.GetNullableContextValue();
                if (!nullableContextValue.HasValue)
                {
                    return metadataType;
                }
                defaultTransform = nullableContextValue.GetValueOrDefault();
            }
            if (!containingModule.ShouldDecodeNullableAttributes(accessSymbol))
            {
                return metadataType;
            }
            return TransformType(metadataType, defaultTransform, nullableTransforms);
        }

        internal static TypeWithAnnotations TransformType(TypeWithAnnotations metadataType, byte defaultTransformFlag, ImmutableArray<byte> nullableTransformFlags)
        {
            if (nullableTransformFlags.IsDefault && defaultTransformFlag == 0)
            {
                return metadataType;
            }
            int position = 0;
            if (metadataType.ApplyNullableTransforms(defaultTransformFlag, nullableTransformFlags, ref position, out var result) && (nullableTransformFlags.IsDefault || position == nullableTransformFlags.Length))
            {
                return result;
            }
            return metadataType;
        }
    }
}
