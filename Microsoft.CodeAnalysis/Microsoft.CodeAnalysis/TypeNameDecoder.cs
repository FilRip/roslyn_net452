using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis
{
    public abstract class TypeNameDecoder<ModuleSymbol, TypeSymbol> where ModuleSymbol : class where TypeSymbol : class
    {
        private readonly SymbolFactory<ModuleSymbol, TypeSymbol> _factory;

        protected readonly ModuleSymbol moduleSymbol;

        protected TypeSymbol SystemTypeSymbol => _factory.GetSystemTypeSymbol(moduleSymbol);

        internal TypeNameDecoder(SymbolFactory<ModuleSymbol, TypeSymbol> factory, ModuleSymbol moduleSymbol)
        {
            _factory = factory;
            this.moduleSymbol = moduleSymbol;
        }

        protected abstract bool IsContainingAssembly(AssemblyIdentity identity);

        protected abstract TypeSymbol LookupTopLevelTypeDefSymbol(ref MetadataTypeName emittedName, out bool isNoPiaLocalType);

        protected abstract TypeSymbol LookupTopLevelTypeDefSymbol(int referencedAssemblyIndex, ref MetadataTypeName emittedName);

        protected abstract TypeSymbol LookupNestedTypeDefSymbol(TypeSymbol container, ref MetadataTypeName emittedName);

        protected abstract int GetIndexOfReferencedAssembly(AssemblyIdentity identity);

        public TypeSymbol GetTypeSymbolForSerializedType(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return GetUnsupportedMetadataTypeSymbol();
            }
            MetadataHelpers.AssemblyQualifiedTypeName fullName = MetadataHelpers.DecodeTypeName(s);
            return GetTypeSymbol(fullName, out bool refersToNoPiaLocalType);
        }

        protected TypeSymbol GetUnsupportedMetadataTypeSymbol(BadImageFormatException exception = null)
        {
            return _factory.GetUnsupportedMetadataTypeSymbol(moduleSymbol, exception);
        }

        protected TypeSymbol GetSZArrayTypeSymbol(TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            return _factory.GetSZArrayTypeSymbol(moduleSymbol, elementType, customModifiers);
        }

        protected TypeSymbol GetMDArrayTypeSymbol(int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
        {
            return _factory.GetMDArrayTypeSymbol(moduleSymbol, rank, elementType, customModifiers, sizes, lowerBounds);
        }

        protected TypeSymbol MakePointerTypeSymbol(TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
        {
            return _factory.MakePointerTypeSymbol(moduleSymbol, type, customModifiers);
        }

        protected TypeSymbol MakeFunctionPointerTypeSymbol(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamInfos)
        {
            return _factory.MakeFunctionPointerTypeSymbol(callingConvention, retAndParamInfos);
        }

        protected TypeSymbol GetSpecialType(SpecialType specialType)
        {
            return _factory.GetSpecialType(moduleSymbol, specialType);
        }

        protected TypeSymbol GetEnumUnderlyingType(TypeSymbol type)
        {
            return _factory.GetEnumUnderlyingType(moduleSymbol, type);
        }

        protected PrimitiveTypeCode GetPrimitiveTypeCode(TypeSymbol type)
        {
            return _factory.GetPrimitiveTypeCode(moduleSymbol, type);
        }

        protected TypeSymbol SubstituteWithUnboundIfGeneric(TypeSymbol type)
        {
            return _factory.MakeUnboundIfGeneric(moduleSymbol, type);
        }

        protected TypeSymbol SubstituteTypeParameters(TypeSymbol genericType, ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments, ImmutableArray<bool> refersToNoPiaLocalType)
        {
            return _factory.SubstituteTypeParameters(moduleSymbol, genericType, arguments, refersToNoPiaLocalType);
        }

        internal TypeSymbol GetTypeSymbol(MetadataHelpers.AssemblyQualifiedTypeName fullName, out bool refersToNoPiaLocalType)
        {
            int num;
            if (fullName.AssemblyName != null)
            {
                if (!AssemblyIdentity.TryParseDisplayName(fullName.AssemblyName, out var identity))
                {
                    refersToNoPiaLocalType = false;
                    return GetUnsupportedMetadataTypeSymbol();
                }
                num = GetIndexOfReferencedAssembly(identity);
                if (num == -1 && !IsContainingAssembly(identity))
                {
                    refersToNoPiaLocalType = false;
                    return GetUnsupportedMetadataTypeSymbol();
                }
            }
            else
            {
                num = -1;
            }
            MetadataTypeName emittedName = MetadataTypeName.FromFullName(fullName.TopLevelType);
            TypeSymbol val = LookupTopLevelTypeDefSymbol(ref emittedName, num, out refersToNoPiaLocalType);
            if (fullName.NestedTypes != null)
            {
                if (refersToNoPiaLocalType)
                {
                    refersToNoPiaLocalType = false;
                    return GetUnsupportedMetadataTypeSymbol();
                }
                for (int i = 0; i < fullName.NestedTypes.Length; i++)
                {
                    emittedName = MetadataTypeName.FromTypeName(fullName.NestedTypes[i]);
                    val = LookupNestedTypeDefSymbol(val, ref emittedName);
                }
            }
            if (fullName.TypeArguments != null)
            {
                ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments = ResolveTypeArguments(fullName.TypeArguments, out ImmutableArray<bool> refersToNoPiaLocalType2);
                val = SubstituteTypeParameters(val, arguments, refersToNoPiaLocalType2);
                ImmutableArray<bool>.Enumerator enumerator = refersToNoPiaLocalType2.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current)
                    {
                        refersToNoPiaLocalType = true;
                        break;
                    }
                }
            }
            else
            {
                val = SubstituteWithUnboundIfGeneric(val);
            }
            for (int j = 0; j < fullName.PointerCount; j++)
            {
                val = MakePointerTypeSymbol(val, ImmutableArray<ModifierInfo<TypeSymbol>>.Empty);
            }
            if (fullName.ArrayRanks != null)
            {
                int[] arrayRanks = fullName.ArrayRanks;
                foreach (int num2 in arrayRanks)
                {
                    val = ((num2 == 0) ? GetSZArrayTypeSymbol(val, default(ImmutableArray<ModifierInfo<TypeSymbol>>)) : GetMDArrayTypeSymbol(num2, val, default(ImmutableArray<ModifierInfo<TypeSymbol>>), ImmutableArray<int>.Empty, default(ImmutableArray<int>)));
                }
            }
            return val;
        }

        private ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> ResolveTypeArguments(MetadataHelpers.AssemblyQualifiedTypeName[] arguments, out ImmutableArray<bool> refersToNoPiaLocalType)
        {
            int capacity = arguments.Length;
            ArrayBuilder<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> instance = ArrayBuilder<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>>.GetInstance(capacity);
            ArrayBuilder<bool> instance2 = ArrayBuilder<bool>.GetInstance(capacity);
            foreach (MetadataHelpers.AssemblyQualifiedTypeName fullName in arguments)
            {
                instance.Add(new KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>(GetTypeSymbol(fullName, out var refersToNoPiaLocalType2), ImmutableArray<ModifierInfo<TypeSymbol>>.Empty));
                instance2.Add(refersToNoPiaLocalType2);
            }
            refersToNoPiaLocalType = instance2.ToImmutableAndFree();
            return instance.ToImmutableAndFree();
        }

        private TypeSymbol LookupTopLevelTypeDefSymbol(ref MetadataTypeName emittedName, int referencedAssemblyIndex, out bool isNoPiaLocalType)
        {
            if (referencedAssemblyIndex >= 0)
            {
                isNoPiaLocalType = false;
                return LookupTopLevelTypeDefSymbol(referencedAssemblyIndex, ref emittedName);
            }
            return LookupTopLevelTypeDefSymbol(ref emittedName, out isNoPiaLocalType);
        }
    }
}
