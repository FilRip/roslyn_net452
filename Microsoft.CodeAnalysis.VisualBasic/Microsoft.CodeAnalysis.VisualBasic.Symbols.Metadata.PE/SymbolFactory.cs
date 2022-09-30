using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class SymbolFactory : SymbolFactory<PEModuleSymbol, TypeSymbol>
	{
		internal static readonly SymbolFactory Instance = new SymbolFactory();

		internal override TypeSymbol GetMDArrayTypeSymbol(PEModuleSymbol moduleSymbol, int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds)
		{
			if (elementType is UnsupportedMetadataTypeSymbol)
			{
				return elementType;
			}
			return ArrayTypeSymbol.CreateMDArray(elementType, VisualBasicCustomModifier.Convert(customModifiers), rank, sizes, lowerBounds, moduleSymbol.ContainingAssembly);
		}

		internal override TypeSymbol GetSpecialType(PEModuleSymbol moduleSymbol, SpecialType specialType)
		{
			return moduleSymbol.ContainingAssembly.GetSpecialType(specialType);
		}

		internal override TypeSymbol GetSystemTypeSymbol(PEModuleSymbol moduleSymbol)
		{
			return moduleSymbol.SystemTypeSymbol;
		}

		internal override TypeSymbol GetEnumUnderlyingType(PEModuleSymbol moduleSymbol, TypeSymbol type)
		{
			return TypeSymbolExtensions.GetEnumUnderlyingType(type);
		}

		internal override PrimitiveTypeCode GetPrimitiveTypeCode(PEModuleSymbol moduleSymbol, TypeSymbol type)
		{
			return type.PrimitiveTypeCode;
		}

		internal override TypeSymbol GetSZArrayTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
		{
			if (elementType is UnsupportedMetadataTypeSymbol)
			{
				return elementType;
			}
			return ArrayTypeSymbol.CreateSZArray(elementType, VisualBasicCustomModifier.Convert(customModifiers), moduleSymbol.ContainingAssembly);
		}

		internal override TypeSymbol GetUnsupportedMetadataTypeSymbol(PEModuleSymbol moduleSymbol, BadImageFormatException exception)
		{
			return new UnsupportedMetadataTypeSymbol(exception);
		}

		internal override TypeSymbol MakePointerTypeSymbol(PEModuleSymbol moduleSymbol, TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers)
		{
			return new PointerTypeSymbol(type, VisualBasicCustomModifier.Convert(customModifiers));
		}

		internal override TypeSymbol SubstituteTypeParameters(PEModuleSymbol moduleSymbol, TypeSymbol genericTypeDef, ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments, ImmutableArray<bool> refersToNoPiaLocalType)
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
				for (int i = num; i >= 0; i += -1)
				{
					if (refersToNoPiaLocalType[i] || (!linkedReferencedAssemblies.IsDefaultOrEmpty && MetadataDecoder.IsOrClosedOverATypeFromAssemblies(arguments[i].Key, linkedReferencedAssemblies)))
					{
						flag = true;
						break;
					}
				}
			}
			ImmutableArray<TypeParameterSymbol> allTypeParameters = TypeSymbolExtensions.GetAllTypeParameters(namedTypeSymbol);
			if (allTypeParameters.Length != arguments.Length)
			{
				return new UnsupportedMetadataTypeSymbol();
			}
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(genericTypeDef, allTypeParameters, arguments.SelectAsArray((KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>> pair) => new TypeWithModifiers(pair.Key, VisualBasicCustomModifier.Convert(pair.Value))));
			if (typeSubstitution == null)
			{
				return genericTypeDef;
			}
			NamedTypeSymbol namedTypeSymbol3 = namedTypeSymbol.Construct(typeSubstitution);
			if (flag)
			{
				namedTypeSymbol3 = new NoPiaIllegalGenericInstantiationSymbol(namedTypeSymbol3);
			}
			return namedTypeSymbol3;
		}

		internal override TypeSymbol MakeUnboundIfGeneric(PEModuleSymbol moduleSymbol, TypeSymbol type)
		{
			if (!(type is NamedTypeSymbol namedTypeSymbol) || !namedTypeSymbol.IsGenericType)
			{
				return type;
			}
			return UnboundGenericType.Create(namedTypeSymbol);
		}

		internal override TypeSymbol MakeFunctionPointerTypeSymbol(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
		{
			return new UnsupportedMetadataTypeSymbol();
		}
	}
}
