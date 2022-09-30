using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class VarianceAmbiguity
	{
		public static bool HasVarianceAmbiguity(NamedTypeSymbol containingType, NamedTypeSymbol i1, NamedTypeSymbol i2, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol namedTypeSymbol = i1;
			NamedTypeSymbol namedTypeSymbol2 = i2;
			bool causesAmbiguity = false;
			bool preventsAmbiguity = false;
			do
			{
				int num = namedTypeSymbol.Arity - 1;
				for (int j = 0; j <= num; j++)
				{
					CheckCorrespondingTypeArguments(containingType, namedTypeSymbol.TypeParameters[j].Variance, namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(j, ref useSiteInfo), namedTypeSymbol2.TypeArgumentWithDefinitionUseSiteDiagnostics(j, ref useSiteInfo), ref causesAmbiguity, ref preventsAmbiguity, ref useSiteInfo);
				}
				namedTypeSymbol = namedTypeSymbol.ContainingType;
				namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
			}
			while ((object)namedTypeSymbol != null);
			return causesAmbiguity && !preventsAmbiguity;
		}

		private static void CheckCorrespondingTypeArguments(NamedTypeSymbol containingType, VarianceKind variance, TypeSymbol typeArgument1, TypeSymbol typeArgument2, ref bool causesAmbiguity, ref bool preventsAmbiguity, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(typeArgument1, typeArgument2))
			{
				return;
			}
			switch (variance)
			{
			case VarianceKind.In:
			{
				bool flag = TypeSymbolExtensions.IsClassType(typeArgument1) && TypeSymbolExtensions.IsClassType(typeArgument2);
				bool flag2 = flag && (Conversions.ClassifyDirectCastConversion(typeArgument1, typeArgument2, ref useSiteInfo) & ConversionKind.Reference) != 0;
				if (!TypeSymbolExtensions.IsNotInheritable(typeArgument1) && !TypeSymbolExtensions.IsNotInheritable(typeArgument2) && (!flag || flag2))
				{
					causesAmbiguity = true;
				}
				else if ((typeArgument1.IsValueType || typeArgument2.IsValueType || (flag && !flag2)) && !TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2))
				{
					preventsAmbiguity = true;
				}
				break;
			}
			case VarianceKind.Out:
				if (typeArgument1.SpecialType != SpecialType.System_Object && typeArgument2.SpecialType != SpecialType.System_Object && !typeArgument1.IsValueType && !typeArgument2.IsValueType)
				{
					causesAmbiguity = true;
				}
				else if ((typeArgument1.IsValueType || typeArgument2.IsValueType) && !TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2))
				{
					preventsAmbiguity = true;
				}
				break;
			case VarianceKind.None:
				if (!TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2))
				{
					preventsAmbiguity = true;
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(variance);
			}
		}
	}
}
