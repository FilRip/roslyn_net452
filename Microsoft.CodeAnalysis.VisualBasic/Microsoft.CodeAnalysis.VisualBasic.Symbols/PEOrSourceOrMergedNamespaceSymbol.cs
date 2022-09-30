using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class PEOrSourceOrMergedNamespaceSymbol : NamespaceSymbol
	{
		private Dictionary<string, ImmutableArray<Symbol>> _lazyExtensionMethodsMap;

		private int _extQueryCnt;

		private static readonly Dictionary<string, ImmutableArray<Symbol>> s_emptyDictionary = new Dictionary<string, ImmutableArray<Symbol>>();

		private byte _lazyDeclaredAccessibilityOfMostAccessibleDescendantType;

		internal Accessibility RawLazyDeclaredAccessibilityOfMostAccessibleDescendantType => (Accessibility)_lazyDeclaredAccessibilityOfMostAccessibleDescendantType;

		internal abstract override EmbeddedSymbolKind EmbeddedSymbolKind { get; }

		internal sealed override Accessibility DeclaredAccessibilityOfMostAccessibleDescendantType
		{
			get
			{
				if (_lazyDeclaredAccessibilityOfMostAccessibleDescendantType == 1)
				{
					_lazyDeclaredAccessibilityOfMostAccessibleDescendantType = (byte)GetDeclaredAccessibilityOfMostAccessibleDescendantType();
					if (_lazyDeclaredAccessibilityOfMostAccessibleDescendantType == 6)
					{
						PEOrSourceOrMergedNamespaceSymbol pEOrSourceOrMergedNamespaceSymbol = ContainingSymbol as PEOrSourceOrMergedNamespaceSymbol;
						while ((object)pEOrSourceOrMergedNamespaceSymbol != null && pEOrSourceOrMergedNamespaceSymbol._lazyDeclaredAccessibilityOfMostAccessibleDescendantType == 1)
						{
							pEOrSourceOrMergedNamespaceSymbol._lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 6;
							pEOrSourceOrMergedNamespaceSymbol = pEOrSourceOrMergedNamespaceSymbol.ContainingSymbol as PEOrSourceOrMergedNamespaceSymbol;
						}
					}
				}
				return (Accessibility)_lazyDeclaredAccessibilityOfMostAccessibleDescendantType;
			}
		}

		protected PEOrSourceOrMergedNamespaceSymbol()
		{
			_lazyDeclaredAccessibilityOfMostAccessibleDescendantType = 1;
		}

		internal override void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods)
		{
			ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
			if (_lazyExtensionMethodsMap == null)
			{
				if (Interlocked.Increment(ref _extQueryCnt) != 40)
				{
					GetExtensionMethods(methods, name);
					return;
				}
				EnsureExtensionMethodsAreCollected();
			}
			if (_lazyExtensionMethodsMap.TryGetValue(name, out value))
			{
				methods.AddRange(value.As<MethodSymbol>());
			}
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamespaceSymbol appendThrough)
		{
			EnsureExtensionMethodsAreCollected();
			appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, _lazyExtensionMethodsMap);
		}

		private void EnsureExtensionMethodsAreCollected()
		{
			if (_lazyExtensionMethodsMap != null)
			{
				return;
			}
			Dictionary<string, ArrayBuilder<MethodSymbol>> dictionary = new Dictionary<string, ArrayBuilder<MethodSymbol>>(CaseInsensitiveComparison.Comparer);
			BuildExtensionMethodsMap(dictionary);
			if (dictionary.Count == 0)
			{
				_lazyExtensionMethodsMap = s_emptyDictionary;
				return;
			}
			Dictionary<string, ImmutableArray<Symbol>> dictionary2 = new Dictionary<string, ImmutableArray<Symbol>>(dictionary.Count, CaseInsensitiveComparison.Comparer);
			foreach (KeyValuePair<string, ArrayBuilder<MethodSymbol>> item in dictionary)
			{
				dictionary2.Add(item.Key, StaticCast<Symbol>.From(item.Value.ToImmutableAndFree()));
			}
			_lazyExtensionMethodsMap = dictionary2;
		}
	}
}
