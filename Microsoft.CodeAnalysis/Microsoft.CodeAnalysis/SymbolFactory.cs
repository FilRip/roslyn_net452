using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis
{
    public abstract class SymbolFactory<ModuleSymbol, TypeSymbol> where TypeSymbol : class
    {
        public abstract TypeSymbol GetUnsupportedMetadataTypeSymbol(ModuleSymbol moduleSymbol, BadImageFormatException exception);

        public abstract TypeSymbol MakeUnboundIfGeneric(ModuleSymbol moduleSymbol, TypeSymbol type);

        public abstract TypeSymbol GetSZArrayTypeSymbol(ModuleSymbol moduleSymbol, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);

        public abstract TypeSymbol GetMDArrayTypeSymbol(ModuleSymbol moduleSymbol, int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds);

        public abstract TypeSymbol SubstituteTypeParameters(ModuleSymbol moduleSymbol, TypeSymbol generic, ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments, ImmutableArray<bool> refersToNoPiaLocalType);

        public abstract TypeSymbol MakePointerTypeSymbol(ModuleSymbol moduleSymbol, TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);

        public abstract TypeSymbol MakeFunctionPointerTypeSymbol(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> returnAndParamTypes);

        public abstract TypeSymbol GetSpecialType(ModuleSymbol moduleSymbol, SpecialType specialType);

        public abstract TypeSymbol GetSystemTypeSymbol(ModuleSymbol moduleSymbol);

        public abstract TypeSymbol GetEnumUnderlyingType(ModuleSymbol moduleSymbol, TypeSymbol type);

        public abstract PrimitiveTypeCode GetPrimitiveTypeCode(ModuleSymbol moduleSymbol, TypeSymbol type);
    }
}
