using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE
{
    internal sealed class SymbolFactory : SymbolFactory<PEModuleSymbol, TypeSymbol>
    {
        internal static readonly SymbolFactory Instance = new SymbolFactory();

        public override TypeSymbol GetMDArrayTypeSymbol(PEModuleSymbol moduleSymbol, int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
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

        public override TypeSymbol MakeFunctionPointerTypeSymbol(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
        {
            return FunctionPointerTypeSymbol.CreateFromMetadata(callingConvention, retAndParamTypes);
        }

        public override TypeSymbol GetEnumUnderlyingType(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            return type.GetEnumUnderlyingType();
        }

        public override PrimitiveTypeCode GetPrimitiveTypeCode(PEModuleSymbol moduleSymbol, TypeSymbol type)
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

        public override TypeSymbol SubstituteTypeParameters(PEModuleSymbol moduleSymbol, TypeSymbol genericTypeDef, ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments, ImmutableArray<bool> refersToNoPiaLocalType)
        {
            if (genericTypeDef is UnsupportedMetadataTypeSymbol)
            {
                return genericTypeDef;
            }
            ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>>.Enumerator enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>> current = enumerator.Current;
                if (current.Key.Kind == SymbolKind.ErrorType && current.Key is UnsupportedMetadataTypeSymbol)
                {
                    return new UnsupportedMetadataTypeSymbol();
                }
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)genericTypeDef;
            ImmutableArray<AssemblySymbol> linkedReferencedAssemblies = moduleSymbol.ContainingAssembly.GetLinkedReferencedAssemblies();
            bool flag = false;
            if (!linkedReferencedAssemblies.IsDefaultOrEmpty || moduleSymbol.Module.ContainsNoPiaLocalTypes())
            {
                NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol;
                int num = refersToNoPiaLocalType.Length - 1;
                while (namedTypeSymbol2.IsInterface)
                {
                    num -= namedTypeSymbol2.Arity;
                    namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
                    if ((object)namedTypeSymbol2 == null)
                    {
                        break;
                    }
                }
                for (int num2 = num; num2 >= 0; num2--)
                {
                    if (refersToNoPiaLocalType[num2] || (!linkedReferencedAssemblies.IsDefaultOrEmpty && MetadataDecoder.IsOrClosedOverATypeFromAssemblies(arguments[num2].Key, linkedReferencedAssemblies)))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            ImmutableArray<TypeParameterSymbol> allTypeParameters = namedTypeSymbol.GetAllTypeParameters();
            if (allTypeParameters.Length != arguments.Length)
            {
                return new UnsupportedMetadataTypeSymbol();
            }
            NamedTypeSymbol namedTypeSymbol3 = new TypeMap(allTypeParameters, arguments.SelectAsArray((KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>> arg) => CreateType(arg.Key, arg.Value))).SubstituteNamedType(namedTypeSymbol);
            if (flag)
            {
                namedTypeSymbol3 = new NoPiaIllegalGenericInstantiationSymbol(moduleSymbol, namedTypeSymbol3);
            }
            return namedTypeSymbol3;
        }

        public override TypeSymbol MakeUnboundIfGeneric(PEModuleSymbol moduleSymbol, TypeSymbol type)
        {
            if (!(type is NamedTypeSymbol namedTypeSymbol) || !namedTypeSymbol.IsGenericType)
            {
                return type;
            }
            return namedTypeSymbol.AsUnboundGenericType();
        }

        private static TypeWithAnnotations CreateType(TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            return TypeWithAnnotations.Create(type, NullableAnnotation.Oblivious, CSharpCustomModifier.Convert(customModifiers));
        }
    }
}
