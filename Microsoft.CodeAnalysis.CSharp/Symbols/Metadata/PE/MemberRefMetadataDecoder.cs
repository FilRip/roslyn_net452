// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    /// <summary>
    /// This subclass of MetadataDecoder is specifically for finding
    /// method symbols corresponding to method MemberRefs.  The parent 
    /// implementation is unsuitable because it requires a PEMethodSymbol
    /// for context when decoding method type parameters and no such
    /// context is available because it is precisely what we are trying
    /// to find.  Since we know in advance that there will be no context
    /// and that signatures decoded with this class will only be used
    /// for comparison (when searching through the methods of a known
    /// TypeSymbol), we can return indexed type parameters instead.
    /// </summary>
    internal sealed class MemberRefMetadataDecoder : MetadataDecoder
    {
        /// <summary>
        /// Type context for resolving generic type arguments.
        /// </summary>
        private readonly TypeSymbol _containingType;

        public MemberRefMetadataDecoder(
            PEModuleSymbol moduleSymbol,
            TypeSymbol containingType) :
            base(moduleSymbol, containingType as PENamedTypeSymbol)
        {
            _containingType = containingType;
        }

        /// <summary>
        /// We know that we'll never have a method context because that's what we're
        /// trying to find.  Instead, just return an indexed type parameter that will
        /// make comparison easier.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected override TypeSymbol GetGenericMethodTypeParamSymbol(int position)
        {
            // Note: technically this is a source symbol, but we only care about the position
            return IndexedTypeParameterSymbol.GetTypeParameter(position);
        }

        /// <summary>
        /// This override changes two things:
        ///     1) Return type arguments instead of type parameters.
        ///     2) Handle non-PE types.
        /// </summary>
        protected override TypeSymbol GetGenericTypeParamSymbol(int position)
        {
            if (_containingType is PENamedTypeSymbol peType)
            {
                while (peType is not null && (peType.MetadataArity - peType.Arity) > position)
                {
                    peType = peType.ContainingSymbol as PENamedTypeSymbol;
                }

                if (peType is null || peType.MetadataArity <= position)
                {
                    return new UnsupportedMetadataTypeSymbol(); // position of type parameter too large
                }

                position -= peType.MetadataArity - peType.Arity;

                return peType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[position].Type; //NB: args, not params
            }

            if (_containingType is NamedTypeSymbol namedType)
            {
                GetGenericTypeArgumentSymbol(position, namedType, out int _, out TypeSymbol typeArgument);
                if (typeArgument is not null)
                {
                    return typeArgument;
                }
                else
                {
                    return new UnsupportedMetadataTypeSymbol(); // position of type parameter too large
                }
            }

            return new UnsupportedMetadataTypeSymbol(); // associated type does not have type parameters
        }

        private static void GetGenericTypeArgumentSymbol(int position, NamedTypeSymbol namedType, out int cumulativeArity, out TypeSymbol typeArgument)
        {
            cumulativeArity = namedType.Arity;
            typeArgument = null;

            int arityOffset = 0;

            var containingType = namedType.ContainingType;
            if (containingType is not null)
            {
                GetGenericTypeArgumentSymbol(position, containingType, out int containingTypeCumulativeArity, out typeArgument);
                cumulativeArity += containingTypeCumulativeArity;
                arityOffset = containingTypeCumulativeArity;
            }

            if (arityOffset <= position && position < cumulativeArity)
            {

                typeArgument = namedType.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[position - arityOffset].Type;
            }
        }

        /// <summary>
        /// Search through the members of a given type symbol to find the method that matches a particular
        /// signature.
        /// </summary>
        /// <param name="targetTypeSymbol">Type containing the desired method symbol.</param>
        /// <param name="memberRef">A MemberRef handle that can be used to obtain the name and signature of the method</param>
        /// <param name="methodsOnly">True to only return a method.</param>
        /// <returns>The matching method symbol, or null if the inputs do not correspond to a valid method.</returns>
        internal Symbol FindMember(TypeSymbol targetTypeSymbol, MemberReferenceHandle memberRef, bool methodsOnly)
        {
            if (targetTypeSymbol is null)
            {
                return null;
            }

            try
            {
                string memberName = Module.GetMemberRefNameOrThrow(memberRef);
                BlobHandle signatureHandle = Module.GetSignatureOrThrow(memberRef);

                BlobReader signaturePointer = this.DecodeSignatureHeaderOrThrow(signatureHandle, out SignatureHeader signatureHeader);

                switch (signatureHeader.RawValue & SignatureHeader.CallingConventionOrKindMask)
                {
                    case (byte)SignatureCallingConvention.Default:
                    case (byte)SignatureCallingConvention.VarArgs:
                        int typeParamCount;
                        ParamInfo<TypeSymbol>[] targetParamInfo = this.DecodeSignatureParametersOrThrow(ref signaturePointer, signatureHeader, out typeParamCount);
                        return FindMethodBySignature(targetTypeSymbol, memberName, signatureHeader, typeParamCount, targetParamInfo);

                    case (byte)SignatureKind.Field:
                        if (methodsOnly)
                        {
                            // skip:
                            return null;
                        }

                        ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers;
                        TypeSymbol type = this.DecodeFieldSignature(ref signaturePointer, out customModifiers);
                        return FindFieldBySignature(targetTypeSymbol, memberName, customModifiers, type);

                    default:
                        // error: unexpected calling convention
                        return null;
                }
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }

        private static FieldSymbol FindFieldBySignature(TypeSymbol targetTypeSymbol, string targetMemberName, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, TypeSymbol type)
        {
            foreach (Symbol member in targetTypeSymbol.GetMembers(targetMemberName))
            {
                TypeWithAnnotations fieldType;

                if (member is FieldSymbol field &&
                    TypeSymbol.Equals((fieldType = field.TypeWithAnnotations).Type, type, TypeCompareKind.CLRSignatureCompareOptions) &&
                    CustomModifiersMatch(fieldType.CustomModifiers, customModifiers))
                {
                    // Behavior in the face of multiple matching signatures is
                    // implementation defined - we'll just pick the first one.
                    return field;
                }
            }

            return null;
        }

        private static MethodSymbol FindMethodBySignature(TypeSymbol targetTypeSymbol, string targetMemberName, SignatureHeader targetMemberSignatureHeader, int targetMemberTypeParamCount, ParamInfo<TypeSymbol>[] targetParamInfo)
        {
            foreach (Symbol member in targetTypeSymbol.GetMembers(targetMemberName))
            {
                if (member is MethodSymbol method &&
                    ((byte)method.CallingConvention == targetMemberSignatureHeader.RawValue) &&
                    (targetMemberTypeParamCount == method.Arity) &&
                    MethodSymbolMatchesParamInfo(method, targetParamInfo))
                {
                    // Behavior in the face of multiple matching signatures is
                    // implementation defined - we'll just pick the first one.
                    return method;
                }
            }

            return null;
        }

        private static bool MethodSymbolMatchesParamInfo(MethodSymbol candidateMethod, ParamInfo<TypeSymbol>[] targetParamInfo)
        {
            int numParams = targetParamInfo.Length - 1; //don't count return type

            if (candidateMethod.ParameterCount != numParams)
            {
                return false;
            }

            // IndexedTypeParameterSymbol is not going to be exposed anywhere,
            // so we'll cheat and use it here for comparison purposes.
            TypeMap candidateMethodTypeMap = new(
                candidateMethod.TypeParameters,
                IndexedTypeParameterSymbol.Take(candidateMethod.Arity), true);

            if (!ReturnTypesMatch(candidateMethod, candidateMethodTypeMap, ref targetParamInfo[0]))
            {
                return false;
            }

            for (int i = 0; i < numParams; i++)
            {
                if (!ParametersMatch(candidateMethod.Parameters[i], candidateMethodTypeMap, ref targetParamInfo[i + 1 /*for return type*/]))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ParametersMatch(ParameterSymbol candidateParam, TypeMap candidateMethodTypeMap, ref ParamInfo<TypeSymbol> targetParam)
        {

            // This could be combined into a single return statement with a more complicated expression, but that would
            // be harder to debug.

            if ((candidateParam.RefKind != RefKind.None) != targetParam.IsByRef)
            {
                return false;
            }

            // CONSIDER: Do we want to add special handling for error types?  Right now, we expect they'll just fail to match.
            var substituted = candidateParam.TypeWithAnnotations.SubstituteType(candidateMethodTypeMap);
            if (!TypeSymbol.Equals(substituted.Type, targetParam.Type, TypeCompareKind.CLRSignatureCompareOptions))
            {
                return false;
            }

            if (!CustomModifiersMatch(substituted.CustomModifiers, targetParam.CustomModifiers) ||
                !CustomModifiersMatch(candidateMethodTypeMap.SubstituteCustomModifiers(candidateParam.RefCustomModifiers), targetParam.RefCustomModifiers))
            {
                return false;
            }

            return true;
        }

        private static bool ReturnTypesMatch(MethodSymbol candidateMethod, TypeMap candidateMethodTypeMap, ref ParamInfo<TypeSymbol> targetReturnParam)
        {

            if (candidateMethod.ReturnsByRef != targetReturnParam.IsByRef)
            {
                return false;
            }

            TypeWithAnnotations candidateMethodType = candidateMethod.ReturnTypeWithAnnotations;
            TypeSymbol targetReturnType = targetReturnParam.Type;

            // CONSIDER: Do we want to add special handling for error types?  Right now, we expect they'll just fail to match.
            var substituted = candidateMethodType.SubstituteType(candidateMethodTypeMap);
            if (!TypeSymbol.Equals(substituted.Type, targetReturnType, TypeCompareKind.CLRSignatureCompareOptions))
            {
                return false;
            }

            if (!CustomModifiersMatch(substituted.CustomModifiers, targetReturnParam.CustomModifiers) ||
                !CustomModifiersMatch(candidateMethodTypeMap.SubstituteCustomModifiers(candidateMethod.RefCustomModifiers), targetReturnParam.RefCustomModifiers))
            {
                return false;
            }

            return true;
        }

        private static bool CustomModifiersMatch(ImmutableArray<CustomModifier> candidateCustomModifiers, ImmutableArray<ModifierInfo<TypeSymbol>> targetCustomModifiers)
        {
            if (targetCustomModifiers.IsDefault || targetCustomModifiers.IsEmpty)
            {
                return candidateCustomModifiers.IsDefault || candidateCustomModifiers.IsEmpty;
            }
            else if (candidateCustomModifiers.IsDefault)
            {
                return false;
            }

            var n = candidateCustomModifiers.Length;
            if (targetCustomModifiers.Length != n)
            {
                return false;
            }

            for (int i = 0; i < n; i++)
            {
                var targetCustomModifier = targetCustomModifiers[i];
                CustomModifier candidateCustomModifier = candidateCustomModifiers[i];

                if (targetCustomModifier.IsOptional != candidateCustomModifier.IsOptional ||
                    !object.Equals(targetCustomModifier.Modifier, ((CSharpCustomModifier)candidateCustomModifier).ModifierSymbol))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
