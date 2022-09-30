using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class SubOrFunctionBodyBinder : ExecutableCodeBinder
	{
		private readonly MethodSymbol _methodSymbol;

		protected readonly Dictionary<string, Symbol> _parameterMap;

		public override Symbol ContainingMember => _methodSymbol;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public SubOrFunctionBodyBinder(MethodSymbol methodOrLambdaSymbol, SyntaxNode root, Binder containingBinder)
			: base(root, containingBinder)
		{
			_methodSymbol = methodOrLambdaSymbol;
			ImmutableArray<ParameterSymbol> parameters = methodOrLambdaSymbol.Parameters;
			int num;
			int num2 = (num = parameters.Length);
			if (!methodOrLambdaSymbol.IsSub)
			{
				num++;
			}
			_parameterMap = new Dictionary<string, Symbol>(num, CaseInsensitiveComparison.Comparer);
			int num3 = num2 - 1;
			for (int i = 0; i <= num3; i++)
			{
				ParameterSymbol parameterSymbol = parameters[i];
				if (!_parameterMap.ContainsKey(parameterSymbol.Name))
				{
					_parameterMap[parameterSymbol.Name] = parameterSymbol;
				}
			}
		}

		public abstract override LocalSymbol GetLocalForFunctionValue();

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly | LookupOptions.MustNotBeLocalOrParameter)) == 0)
			{
				Symbol value = null;
				if (_parameterMap.TryGetValue(name, out value))
				{
					lookupResult.SetFrom(CheckViability(value, arity, options, null, ref useSiteInfo));
				}
			}
			else
			{
				base.LookupInSingleBinder(lookupResult, name, arity, options, originalBinder, ref useSiteInfo);
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if ((options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly)) == 0)
			{
				foreach (Symbol value in _parameterMap.Values)
				{
					if (originalBinder.CanAddLookupSymbolInfo(value, options, nameSet, null))
					{
						nameSet.AddSymbol(value, value.Name, 0);
					}
				}
				return;
			}
			base.AddLookupSymbolsInfoInSingleBinder(nameSet, options, originalBinder);
		}
	}
}
