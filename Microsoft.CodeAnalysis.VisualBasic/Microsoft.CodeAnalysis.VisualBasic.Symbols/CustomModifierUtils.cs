using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class CustomModifierUtils
	{
		internal static void CopyMethodCustomModifiers(MethodSymbol sourceMethod, ImmutableArray<TypeSymbol> destinationTypeParameters, [In][Out] ref TypeSymbol destinationReturnType, [In][Out] ref ImmutableArray<ParameterSymbol> parameters)
		{
			MethodSymbol methodSymbol = MethodSymbolExtensions.ConstructIfGeneric(sourceMethod, destinationTypeParameters);
			parameters = CopyParameterCustomModifiers(methodSymbol.Parameters, parameters);
			TypeSymbol returnType = methodSymbol.ReturnType;
			if (TypeSymbolExtensions.IsSameType(destinationReturnType, returnType, TypeCompareKind.AllIgnoreOptionsForVB))
			{
				destinationReturnType = CopyTypeCustomModifiers(returnType, destinationReturnType);
			}
		}

		internal static TypeSymbol CopyTypeCustomModifiers(TypeSymbol sourceType, TypeSymbol destinationType)
		{
			if (TypeSymbolExtensions.ContainsTuple(destinationType) && !TypeSymbolExtensions.IsSameType(sourceType, destinationType, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds))
			{
				ImmutableArray<string> elementNames = VisualBasicCompilation.TupleNamesEncoder.Encode(destinationType);
				return TupleTypeDecoder.DecodeTupleTypesIfApplicable(sourceType, elementNames);
			}
			return sourceType;
		}

		public static ImmutableArray<ParameterSymbol> CopyParameterCustomModifiers(ImmutableArray<ParameterSymbol> overriddenMemberParameters, ImmutableArray<ParameterSymbol> parameters)
		{
			ArrayBuilder<ParameterSymbol> arrayBuilder = null;
			int num = parameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol thisParam = parameters[i];
				if (CopyParameterCustomModifiers(overriddenMemberParameters[i], ref thisParam))
				{
					if (arrayBuilder == null)
					{
						arrayBuilder = ArrayBuilder<ParameterSymbol>.GetInstance();
						arrayBuilder.AddRange(parameters, i);
					}
					arrayBuilder.Add(thisParam);
				}
				else
				{
					arrayBuilder?.Add(thisParam);
				}
			}
			return arrayBuilder?.ToImmutableAndFree() ?? parameters;
		}

		public static bool CopyParameterCustomModifiers(ParameterSymbol overriddenParam, [In][Out] ref ParameterSymbol thisParam)
		{
			if (!overriddenParam.CustomModifiers.SequenceEqual(thisParam.CustomModifiers) || (overriddenParam.IsByRef && thisParam.IsByRef && !overriddenParam.RefCustomModifiers.SequenceEqual(thisParam.RefCustomModifiers)) || !TypeSymbolExtensions.IsSameType(thisParam.Type, overriddenParam.Type, TypeCompareKind.IgnoreTupleNames))
			{
				TypeSymbol type = thisParam.Type;
				TypeSymbol typeSymbol = overriddenParam.Type;
				if (TypeSymbolExtensions.ContainsTuple(type) && !TypeSymbolExtensions.IsSameType(overriddenParam.Type, type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds))
				{
					ImmutableArray<string> elementNames = VisualBasicCompilation.TupleNamesEncoder.Encode(type);
					typeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeSymbol, elementNames);
				}
				thisParam = ((SourceParameterSymbolBase)thisParam).WithTypeAndCustomModifiers(typeSymbol, overriddenParam.CustomModifiers, thisParam.IsByRef ? overriddenParam.RefCustomModifiers : ImmutableArray<CustomModifier>.Empty);
				return true;
			}
			return false;
		}

		internal static bool HasIsExternalInitModifier(ImmutableArray<CustomModifier> modifiers)
		{
			return modifiers.Any((CustomModifier modifier) => !modifier.IsOptional && TypeSymbolExtensions.IsWellKnownTypeIsExternalInit(((VisualBasicCustomModifier)modifier).ModifierSymbol));
		}
	}
}
