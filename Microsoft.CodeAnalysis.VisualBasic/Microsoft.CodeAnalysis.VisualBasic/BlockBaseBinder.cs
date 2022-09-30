using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BlockBaseBinder : BlockBaseBinder<LocalSymbol>
	{
		public BlockBaseBinder(Binder enclosing)
			: base(enclosing)
		{
		}
	}
	internal abstract class BlockBaseBinder<T> : Binder where T : Symbol
	{
		private Dictionary<string, T> _lazyLocalsMap;

		internal abstract ImmutableArray<T> Locals { get; }

		private Dictionary<string, T> LocalsMap
		{
			get
			{
				if (_lazyLocalsMap == null && !Locals.IsEmpty)
				{
					Interlocked.CompareExchange(ref _lazyLocalsMap, BuildMap(Locals), null);
				}
				return _lazyLocalsMap;
			}
		}

		public BlockBaseBinder(Binder enclosing)
			: base(enclosing)
		{
		}

		private Dictionary<string, T> BuildMap(ImmutableArray<T> locals)
		{
			Dictionary<string, T> dictionary = new Dictionary<string, T>(locals.Length, CaseInsensitiveComparison.Comparer);
			ImmutableArray<T>.Enumerator enumerator = locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (!dictionary.ContainsKey(current.Name))
				{
					dictionary[current.Name] = current;
				}
			}
			return dictionary;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<T> locals = Locals;
			T value = null;
			if (locals.IsEmpty || (options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly | LookupOptions.MustNotBeLocalOrParameter)) != 0)
			{
				return;
			}
			if (locals.Length < 6)
			{
				ImmutableArray<T>.Enumerator enumerator = locals.GetEnumerator();
				while (enumerator.MoveNext())
				{
					value = enumerator.Current;
					string name2 = value.Name;
					if ((object)name2 == name || ((name2.Length == name.Length) & CaseInsensitiveComparison.Equals(name2, name)))
					{
						lookupResult.SetFrom(CheckViability(value, arity, options, null, ref useSiteInfo));
						break;
					}
				}
			}
			else if (LocalsMap.TryGetValue(name, out value))
			{
				lookupResult.SetFrom(CheckViability(value, arity, options, null, ref useSiteInfo));
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			ImmutableArray<T> locals = Locals;
			if (locals.IsEmpty || (options & (LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly)) != 0)
			{
				return;
			}
			ImmutableArray<T>.Enumerator enumerator = locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, null))
				{
					nameSet.AddSymbol(current, current.Name, 0);
				}
			}
		}
	}
}
