// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal sealed class SymbolFactory : SymbolFactory<PEModuleSymbol, TypeSymbol>
    {
        internal static readonly SymbolFactory Instance = new SymbolFactory();

        public override TypeSymbol GetMDArrayTypeSymbol(PEModuleSymbol moduleSymbol, int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers,
                                                          ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            if (elementType is UnsupportedMetadataTypeSymbol)
            {
                return elementType;
            }

            return ArrayTypeSymbol.CreateMDArray(moduleSymbol.ContainingAssembly, CreateType(elementType, customModifiers), rank, sizes, lowerBounds);
        }

        public override TypeSymbol GetSpecialType(PEModuleSymbol moduleSymbol, SpecialType specialType)
        {
            return moduleSymbol.ContainingAssembly.GetSpecialType(specialType);
        }

        public override TypeSymbol GetSystemTypeSymbol(PEModuleSymbol moduleSymbol)
        {
            return moduleSymbol.SystemTypeSymbol;
        }

        public override TypeSymbol MakePointerTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            if (type is UnsupportedMetadataTypeSymbol)
            {
                return type;
            }

            return new PointerTypeSymbol(CreateType(type, customModifiers));
        }

        public override TypeSymbol MakeFunctionPointerTypeSymbol(Cci.CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
        {
            return FunctionPointerTypeSymbol.CreateFromMetadata(callingConvention, retAndParamTypes);
        }

        public override TypeSymbol GetEnumUnderlyingType(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            return type.GetEnumUnderlyingType();
        }

        public override Cci.PrimitiveTypeCode GetPrimitiveTypeCode(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            return type.PrimitiveTypeCode;
        }

        public override TypeSymbol GetSZArrayTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            if (elementType is UnsupportedMetadataTypeSymbol)
            {
                return elementType;
            }

            return ArrayTypeSymbol.CreateSZArray(moduleSymbol.ContainingAssembly, CreateType(elementType, customModifiers));
        }

        public override TypeSymbol GetUnsupportedMetadataTypeSymbol(PEModuleSymbol moduleSymbol, BadImageFormatException exception)
        {
            return new UnsupportedMetadataTypeSymbol(exception);
        }

        public override TypeSymbol SubstituteTypeParameters(
            PEModuleSymbol moduleSymbol,
            TypeSymbol genericTypeDef,
            ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments,
            ImmutableArray<bool> refersToNoPiaLocalType)
        {
            if (genericTypeDef is UnsupportedMetadataTypeSymbol)
            {
                return genericTypeDef;
            }

            // Let's return unsupported metadata type if any argument is unsupported metadata type 
            foreach (var arg in arguments)
            {
                if (arg.Key.Kind == SymbolKind.ErrorType &&
                    arg.Key is UnsupportedMetadataTypeSymbol)
                {
                    return new UnsupportedMetadataTypeSymbol();
                }
            }

            NamedTypeSymbol genericType = (NamedTypeSymbol)genericTypeDef;

            // See if it is or its enclosing type is a non-interface closed over NoPia local types. 
            ImmutableArray<AssemblySymbol> linkedAssemblies = moduleSymbol.ContainingAssembly.GetLinkedReferencedAssemblies();

            bool noPiaIllegalGenericInstantiation = false;

            if (!linkedAssemblies.IsDefaultOrEmpty || moduleSymbol.Module.ContainsNoPiaLocalTypes())
            {
                NamedTypeSymbol typeToCheck = genericType;
                int argumentIndex = refersToNoPiaLocalType.Length - 1;

                do
                {
                    if (!typeToCheck.IsInterface)
                    {
                        break;
                    }
                    else
                    {
                        argumentIndex -= typeToCheck.Arity;
                    }

                    typeToCheck = typeToCheck.ContainingType;
                }
                while ((object)typeToCheck != null);

                for (int i = argumentIndex; i >= 0; i--)
                {
                    if (refersToNoPiaLocalType[i] ||
                        (!linkedAssemblies.IsDefaultOrEmpty &&
                        MetadataDecoder.IsOrClosedOverATypeFromAssemblies(arguments[i].Key, linkedAssemblies)))
                    {
                        noPiaIllegalGenericInstantiation = true;
                        break;
                    }
                }
            }

            // Collect generic parameters for the type and its containers in the order
            // that matches passed in arguments, i.e. sorted by the nesting.
            ImmutableArray<TypeParameterSymbol> typeParameters = genericType.GetAllTypeParameters();

            if (typeParameters.Length != arguments.Length)
            {
                return new UnsupportedMetadataTypeSymbol();
            }

            TypeMap substitution = new TypeMap(typeParameters, arguments.SelectAsArray(arg => CreateType(arg.Key, arg.Value)));

            NamedTypeSymbol constructedType = substitution.SubstituteNamedType(genericType);

            if (noPiaIllegalGenericInstantiation)
            {
                constructedType = new NoPiaIllegalGenericInstantiationSymbol(moduleSymbol, constructedType);
            }

            return constructedType;
        }

        public override TypeSymbol MakeUnboundIfGeneric(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            var namedType = type as NamedTypeSymbol;
            return ((object)namedType != null && namedType.IsGenericType) ? namedType.AsUnboundGenericType() : type;
        }

        private static TypeWithAnnotations CreateType(TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            // The actual annotation will be set when these types are transformed by the caller.
            return TypeWithAnnotations.Create(type, NullableAnnotation.Oblivious, CSharpCustomModifier.Convert(customModifiers));
        }
    }
}
