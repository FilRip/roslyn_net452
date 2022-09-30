using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MethodTypeParametersBinder : Binder
	{
		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		public MethodTypeParametersBinder(Binder containingBinder, ImmutableArray<TypeParameterSymbol> typeParameters)
			: base(containingBinder)
		{
			_typeParameters = typeParameters;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = _typeParameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeParameterSymbol typeParameterSymbol = _typeParameters[i];
				if (CaseInsensitiveComparison.Equals(typeParameterSymbol.Name, name))
				{
					lookupResult.SetFrom(CheckViability(typeParameterSymbol, arity, options, null, ref useSiteInfo));
				}
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _typeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				if (originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, null))
				{
					nameSet.AddSymbol(current, current.Name, 0);
				}
			}
		}
	}
}
