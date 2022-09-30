using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class MemberRefMetadataDecoder : MetadataDecoder
	{
		private readonly TypeSymbol _containingType;

		public MemberRefMetadataDecoder(PEModuleSymbol moduleSymbol, TypeSymbol containingType)
			: base(moduleSymbol, containingType as PENamedTypeSymbol)
		{
			_containingType = containingType;
		}

		protected override TypeSymbol GetGenericMethodTypeParamSymbol(int position)
		{
			return IndexedTypeParameterSymbol.GetTypeParameter(position);
		}

		protected override TypeSymbol GetGenericTypeParamSymbol(int position)
		{
			PENamedTypeSymbol pENamedTypeSymbol = _containingType as PENamedTypeSymbol;
			if ((object)pENamedTypeSymbol != null)
			{
				while ((object)pENamedTypeSymbol != null && pENamedTypeSymbol.MetadataArity - pENamedTypeSymbol.Arity > position)
				{
					pENamedTypeSymbol = pENamedTypeSymbol.ContainingSymbol as PENamedTypeSymbol;
				}
				if ((object)pENamedTypeSymbol == null || pENamedTypeSymbol.MetadataArity <= position)
				{
					return new UnsupportedMetadataTypeSymbol(VBResources.PositionOfTypeParameterTooLarge);
				}
				position -= pENamedTypeSymbol.MetadataArity - pENamedTypeSymbol.Arity;
				return pENamedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics[position];
			}
			if (_containingType is NamedTypeSymbol namedType)
			{
				TypeSymbol typeArgument = null;
				int cumulativeArity = default(int);
				GetGenericTypeArgumentSymbol(position, namedType, ref cumulativeArity, ref typeArgument);
				if ((object)typeArgument != null)
				{
					return typeArgument;
				}
				return new UnsupportedMetadataTypeSymbol(VBResources.PositionOfTypeParameterTooLarge);
			}
			return new UnsupportedMetadataTypeSymbol(VBResources.AssociatedTypeDoesNotHaveTypeParameters);
		}

		private static void GetGenericTypeArgumentSymbol(int position, NamedTypeSymbol namedType, ref int cumulativeArity, ref TypeSymbol typeArgument)
		{
			cumulativeArity = namedType.Arity;
			typeArgument = null;
			int num = 0;
			NamedTypeSymbol containingType = namedType.ContainingType;
			if ((object)containingType != null)
			{
				int cumulativeArity2 = default(int);
				GetGenericTypeArgumentSymbol(position, containingType, ref cumulativeArity2, ref typeArgument);
				cumulativeArity += cumulativeArity2;
				num = cumulativeArity2;
			}
			if (num <= position && position < cumulativeArity)
			{
				typeArgument = namedType.TypeArgumentsNoUseSiteDiagnostics[position - num];
			}
		}

		internal Symbol FindMember(TypeSymbol targetTypeSymbol, MemberReferenceHandle memberRef, bool methodsOnly)
		{
			if ((object)targetTypeSymbol == null)
			{
				return null;
			}
			try
			{
				string memberRefNameOrThrow = Module.GetMemberRefNameOrThrow(memberRef);
				BlobHandle signatureOrThrow = Module.GetSignatureOrThrow(memberRef);
				SignatureHeader signatureHeader;
				BlobReader signatureReader = DecodeSignatureHeaderOrThrow(signatureOrThrow, out signatureHeader);
				switch (signatureHeader.RawValue & 0xF)
				{
				case 0:
				case 5:
				{
					int typeParameterCount;
					ParamInfo<TypeSymbol>[] targetParamInfo = DecodeSignatureParametersOrThrow(ref signatureReader, signatureHeader, out typeParameterCount);
					return FindMethodBySignature(targetTypeSymbol, memberRefNameOrThrow, signatureHeader, typeParameterCount, targetParamInfo);
				}
				case 6:
				{
					if (methodsOnly)
					{
						return null;
					}
					ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers = default(ImmutableArray<ModifierInfo<TypeSymbol>>);
					TypeSymbol type = DecodeFieldSignature(ref signatureReader, out customModifiers);
					return FindFieldBySignature(targetTypeSymbol, memberRefNameOrThrow, customModifiers, type);
				}
				default:
					return null;
				}
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				Symbol result = null;
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static FieldSymbol FindFieldBySignature(TypeSymbol targetTypeSymbol, string targetMemberName, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers, TypeSymbol type)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = targetTypeSymbol.GetMembers(targetMemberName).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is FieldSymbol fieldSymbol && TypeSymbol.Equals(fieldSymbol.Type, type, TypeCompareKind.AllIgnoreOptionsForVB) && CustomModifiersMatch(fieldSymbol.CustomModifiers, customModifiers))
				{
					return fieldSymbol;
				}
			}
			return null;
		}

		private static MethodSymbol FindMethodBySignature(TypeSymbol targetTypeSymbol, string targetMemberName, SignatureHeader targetMemberSignatureHeader, int targetMemberTypeParamCount, ParamInfo<TypeSymbol>[] targetParamInfo)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = targetTypeSymbol.GetMembers(targetMemberName).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is MethodSymbol methodSymbol && (byte)methodSymbol.CallingConvention == targetMemberSignatureHeader.RawValue && targetMemberTypeParamCount == methodSymbol.Arity && MethodSymbolMatchesParamInfo(methodSymbol, targetParamInfo))
				{
					return methodSymbol;
				}
			}
			return null;
		}

		private static bool MethodSymbolMatchesParamInfo(MethodSymbol candidateMethod, ParamInfo<TypeSymbol>[] targetParamInfo)
		{
			int num = targetParamInfo.Length - 1;
			if (candidateMethod.ParameterCount != num)
			{
				return false;
			}
			if (candidateMethod.Arity > 0)
			{
				candidateMethod = candidateMethod.Construct(StaticCast<TypeSymbol>.From(IndexedTypeParameterSymbol.Take(candidateMethod.Arity)));
			}
			if (!ReturnTypesMatch(candidateMethod, ref targetParamInfo[0]))
			{
				return false;
			}
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				if (!ParametersMatch(candidateMethod.Parameters[i], ref targetParamInfo[i + 1]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ParametersMatch(ParameterSymbol candidateParam, ref ParamInfo<TypeSymbol> targetParam)
		{
			if (candidateParam.IsByRef != targetParam.IsByRef)
			{
				return false;
			}
			if (!TypeSymbol.Equals(candidateParam.Type, targetParam.Type, TypeCompareKind.AllIgnoreOptionsForVB))
			{
				return false;
			}
			if (!CustomModifiersMatch(candidateParam.CustomModifiers, targetParam.CustomModifiers) || !CustomModifiersMatch(candidateParam.RefCustomModifiers, targetParam.RefCustomModifiers))
			{
				return false;
			}
			return true;
		}

		private static bool ReturnTypesMatch(MethodSymbol candidateMethod, ref ParamInfo<TypeSymbol> targetReturnParam)
		{
			TypeSymbol returnType = candidateMethod.ReturnType;
			TypeSymbol type = targetReturnParam.Type;
			if (!TypeSymbol.Equals(returnType, type, TypeCompareKind.AllIgnoreOptionsForVB) || candidateMethod.ReturnsByRef != targetReturnParam.IsByRef)
			{
				return false;
			}
			if (!CustomModifiersMatch(candidateMethod.ReturnTypeCustomModifiers, targetReturnParam.CustomModifiers) || !CustomModifiersMatch(candidateMethod.RefCustomModifiers, targetReturnParam.RefCustomModifiers))
			{
				return false;
			}
			return true;
		}

		private static bool CustomModifiersMatch(ImmutableArray<CustomModifier> candidateReturnTypeCustomModifiers, ImmutableArray<ModifierInfo<TypeSymbol>> targetReturnTypeCustomModifiers)
		{
			if (targetReturnTypeCustomModifiers.IsDefault || targetReturnTypeCustomModifiers.IsEmpty)
			{
				return candidateReturnTypeCustomModifiers.IsDefault || candidateReturnTypeCustomModifiers.IsEmpty;
			}
			if (candidateReturnTypeCustomModifiers.IsDefault)
			{
				return false;
			}
			int length = candidateReturnTypeCustomModifiers.Length;
			if (targetReturnTypeCustomModifiers.Length != length)
			{
				return false;
			}
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				ModifierInfo<TypeSymbol> modifierInfo = targetReturnTypeCustomModifiers[i];
				CustomModifier customModifier = candidateReturnTypeCustomModifiers[i];
				if (modifierInfo.IsOptional != customModifier.IsOptional || !object.Equals(modifierInfo.Modifier, customModifier.Modifier))
				{
					return false;
				}
			}
			return true;
		}
	}
}
