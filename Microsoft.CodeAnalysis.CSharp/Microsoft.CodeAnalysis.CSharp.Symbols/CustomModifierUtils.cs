using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class CustomModifierUtils
    {
        internal static void CopyMethodCustomModifiers(MethodSymbol sourceMethod, MethodSymbol destinationMethod, out TypeWithAnnotations returnType, out ImmutableArray<CustomModifier> customModifiers, out ImmutableArray<ParameterSymbol> parameters, bool alsoCopyParamsModifier)
        {
            MethodSymbol methodSymbol = sourceMethod.ConstructIfGeneric(destinationMethod.TypeArgumentsWithAnnotations);
            customModifiers = ((destinationMethod.RefKind != 0) ? methodSymbol.RefCustomModifiers : ImmutableArray<CustomModifier>.Empty);
            parameters = CopyParameterCustomModifiers(methodSymbol.Parameters, destinationMethod.Parameters, alsoCopyParamsModifier);
            returnType = destinationMethod.ReturnTypeWithAnnotations;
            TypeSymbol type = returnType.Type;
            TypeWithAnnotations returnTypeWithAnnotations = methodSymbol.ReturnTypeWithAnnotations;
            TypeSymbol type2 = returnTypeWithAnnotations.Type;
            if (type.Equals(type2, TypeCompareKind.AllIgnoreOptions))
            {
                returnType = returnType.WithTypeAndModifiers(CopyTypeCustomModifiers(type2, type, destinationMethod.ContainingAssembly), returnTypeWithAnnotations.CustomModifiers);
            }
        }

        internal static TypeSymbol CopyTypeCustomModifiers(TypeSymbol sourceType, TypeSymbol destinationType, AssemblySymbol containingAssembly)
        {
            ImmutableArray<bool> dynamicTransformFlags = CSharpCompilation.DynamicTransformsEncoder.EncodeWithoutCustomModifierFlags(destinationType, RefKind.None);
            TypeSymbol type = DynamicTypeDecoder.TransformTypeWithoutCustomModifierFlags(sourceType, containingAssembly, RefKind.None, dynamicTransformFlags);
            ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance();
            CSharpCompilation.NativeIntegerTransformsEncoder.Encode(instance, destinationType);
            type = NativeIntegerTypeDecoder.TransformType(type, instance.ToImmutableAndFree());
            if (destinationType.ContainsTuple() && !sourceType.Equals(destinationType, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.IgnoreDynamic | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                ImmutableArray<string> elementNames = CSharpCompilation.TupleNamesEncoder.Encode(destinationType);
                type = TupleTypeDecoder.DecodeTupleTypesIfApplicable(type, elementNames);
            }
            ArrayBuilder<byte> instance2 = ArrayBuilder<byte>.GetInstance();
            destinationType.AddNullableTransforms(instance2);
            int position = 0;
            _ = instance2.Count;
            type.ApplyNullableTransforms(0, instance2.ToImmutableAndFree(), ref position, out type);
            return type;
        }

        internal static ImmutableArray<ParameterSymbol> CopyParameterCustomModifiers(ImmutableArray<ParameterSymbol> sourceParameters, ImmutableArray<ParameterSymbol> destinationParameters, bool alsoCopyParamsModifier)
        {
            ArrayBuilder<ParameterSymbol> arrayBuilder = null;
            int length = destinationParameters.Length;
            for (int i = 0; i < length; i++)
            {
                SourceParameterSymbolBase sourceParameterSymbolBase = (SourceParameterSymbolBase)destinationParameters[i];
                ParameterSymbol parameterSymbol = sourceParameters[i];
                if (parameterSymbol.TypeWithAnnotations.CustomModifiers.Any() || parameterSymbol.RefCustomModifiers.Any() || parameterSymbol.Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds: true) || sourceParameterSymbolBase.TypeWithAnnotations.CustomModifiers.Any() || sourceParameterSymbolBase.RefCustomModifiers.Any() || sourceParameterSymbolBase.Type.HasCustomModifiers(flagNonDefaultArraySizesOrLowerBounds: true) || (alsoCopyParamsModifier && parameterSymbol.IsParams != sourceParameterSymbolBase.IsParams))
                {
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = ArrayBuilder<ParameterSymbol>.GetInstance();
                        arrayBuilder.AddRange(destinationParameters, i);
                    }
                    bool newIsParams = (alsoCopyParamsModifier ? parameterSymbol.IsParams : sourceParameterSymbolBase.IsParams);
                    arrayBuilder.Add(sourceParameterSymbolBase.WithCustomModifiersAndParams(parameterSymbol.Type, parameterSymbol.TypeWithAnnotations.CustomModifiers, (sourceParameterSymbolBase.RefKind != 0) ? parameterSymbol.RefCustomModifiers : ImmutableArray<CustomModifier>.Empty, newIsParams));
                }
                else
                {
                    arrayBuilder?.Add(sourceParameterSymbolBase);
                }
            }
            return arrayBuilder?.ToImmutableAndFree() ?? destinationParameters;
        }

        internal static bool HasInAttributeModifier(this ImmutableArray<CustomModifier> modifiers)
        {
            return modifiers.Any((CustomModifier modifier) => !modifier.IsOptional && ((CSharpCustomModifier)modifier).ModifierSymbol.IsWellKnownTypeInAttribute());
        }

        internal static bool HasIsExternalInitModifier(this ImmutableArray<CustomModifier> modifiers)
        {
            return modifiers.Any((CustomModifier modifier) => !modifier.IsOptional && ((CSharpCustomModifier)modifier).ModifierSymbol.IsWellKnownTypeIsExternalInit());
        }

        internal static bool HasOutAttributeModifier(this ImmutableArray<CustomModifier> modifiers)
        {
            return modifiers.Any((CustomModifier modifier) => !modifier.IsOptional && ((CSharpCustomModifier)modifier).ModifierSymbol.IsWellKnownTypeOutAttribute());
        }
    }
}
