using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class MethodSymbolExtensions
	{
		internal static bool CanBeCalledWithNoParameters(this MethodSymbol method)
		{
			int parameterCount = method.ParameterCount;
			if (parameterCount == 0)
			{
				return true;
			}
			ImmutableArray<ParameterSymbol> parameters = method.Parameters;
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

		internal static ParameterSymbol GetParameterSymbol(this ImmutableArray<ParameterSymbol> parameters, ParameterSyntax parameter)
		{
			SyntaxTree syntaxTree = parameter.SyntaxTree;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Location current2 = enumerator2.Current;
					if (current2.IsInSource && current2.SourceTree == syntaxTree && parameter.Span.Contains(current2.SourceSpan))
					{
						return current;
					}
				}
			}
			return null;
		}

		internal static bool IsPartial(this MethodSymbol method)
		{
			if (method is SourceMemberMethodSymbol sourceMemberMethodSymbol)
			{
				return sourceMemberMethodSymbol.IsPartial;
			}
			return false;
		}

		internal static bool IsPartialWithoutImplementation(this MethodSymbol method)
		{
			if (method is SourceMemberMethodSymbol sourceMemberMethodSymbol && sourceMemberMethodSymbol.IsPartial)
			{
				return (object)sourceMemberMethodSymbol.OtherPartOfPartial == null;
			}
			return false;
		}

		internal static bool IsUserDefinedOperator(this MethodSymbol method)
		{
			MethodKind methodKind = method.MethodKind;
			if (methodKind == MethodKind.Conversion || methodKind == MethodKind.UserDefinedOperator)
			{
				return true;
			}
			return false;
		}

		internal static MethodSymbol ConstructIfGeneric(this MethodSymbol method, ImmutableArray<TypeSymbol> typeArguments)
		{
			if (!method.IsGenericMethod)
			{
				return method;
			}
			return method.Construct(typeArguments);
		}
	}
}
