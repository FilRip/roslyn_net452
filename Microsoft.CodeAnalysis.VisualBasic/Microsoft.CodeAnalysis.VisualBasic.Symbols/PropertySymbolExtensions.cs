using System.Collections.Immutable;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class PropertySymbolExtensions
	{
		internal static bool GetCanBeCalledWithNoParameters(this PropertySymbol prop)
		{
			int parameterCount = prop.ParameterCount;
			if (parameterCount == 0)
			{
				return true;
			}
			ImmutableArray<ParameterSymbol> parameters = prop.Parameters;
			int num = parameterCount - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = parameters[i];
				if (parameterSymbol.IsParamArray && i == parameterCount - 1)
				{
					TypeSymbol type = parameterSymbol.Type;
					if (!TypeSymbolExtensions.IsArrayType(type) || !((ArrayTypeSymbol)type).IsSZArray)
					{
						return false;
					}
				}
				else if (!parameterSymbol.IsOptional)
				{
					return false;
				}
			}
			return true;
		}

		public static TypeSymbol GetTypeFromGetMethod(this PropertySymbol property)
		{
			MethodSymbol getMethod = property.GetMethod;
			if ((object)getMethod != null)
			{
				return getMethod.ReturnType;
			}
			return property.Type;
		}

		public static TypeSymbol GetTypeFromSetMethod(this PropertySymbol property)
		{
			MethodSymbol setMethod = property.SetMethod;
			if ((object)setMethod == null)
			{
				return property.Type;
			}
			ImmutableArray<ParameterSymbol> parameters = setMethod.Parameters;
			return parameters[parameters.Length - 1].Type;
		}
	}
}
