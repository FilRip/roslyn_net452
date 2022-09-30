using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class LookupResult
	{
		private enum SymbolLocation
		{
			FromSourceModule,
			FromReferencedAssembly,
			FromCorLibrary
		}

		private LookupResultKind _kind;

		private readonly ArrayBuilder<Symbol> _symList;

		private DiagnosticInfo _diagInfo;

		private readonly ObjectPool<LookupResult> _pool;

		private static readonly ObjectPool<LookupResult> s_poolInstance = CreatePool();

		private static readonly Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> s_ambiguousInTypeError = delegate(ImmutableArray<Symbol> syms)
		{
			string name = syms[0].Name;
			Symbol containingSymbol2 = syms[0].ContainingSymbol;
			string kindText = SymbolExtensions.GetKindText(containingSymbol2);
			return new AmbiguousSymbolDiagnostic(ERRID.ERR_MetadataMembersAmbiguous3, syms, name, kindText, containingSymbol2);
		};

		private static readonly Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> s_ambiguousInNSError = delegate(ImmutableArray<Symbol> syms)
		{
			Symbol containingSymbol = syms[0].ContainingSymbol;
			if (containingSymbol.Name.Length > 0)
			{
				IEnumerable<Symbol> enumerable = from @group in syms.Select((Symbol sym) => sym.ContainingSymbol).GroupBy((Symbol c) => c.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), CaseInsensitiveComparison.Comparer).OrderBy((IGrouping<string, Symbol> @group) => @group.Key, CaseInsensitiveComparison.Comparer)
					select @group.First();
				if (enumerable.Skip(1).Any())
				{
					return new AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInNamespaces2, syms, syms[0].Name, new FormattedSymbolList(enumerable));
				}
				return new AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInNamespace2, syms, syms[0].Name, containingSymbol);
			}
			return new AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInUnnamedNamespace1, syms, syms[0].Name);
		};

		public LookupResultKind Kind => _kind;

		public bool StopFurtherLookup => _kind >= LookupResultKind.EmptyAndStopLookup;

		public bool IsGood => _kind == LookupResultKind.Good;

		public bool IsGoodOrAmbiguous
		{
			get
			{
				if (_kind != LookupResultKind.Good)
				{
					return _kind == LookupResultKind.Ambiguous;
				}
				return true;
			}
		}

		public bool IsAmbiguous => _kind == LookupResultKind.Ambiguous;

		public bool IsWrongArity
		{
			get
			{
				if (_kind != LookupResultKind.WrongArity)
				{
					return _kind == LookupResultKind.WrongArityAndStopLookup;
				}
				return true;
			}
		}

		public bool HasDiagnostic => _diagInfo != null;

		public DiagnosticInfo Diagnostic => _diagInfo;

		public bool HasSymbol => _symList.Count > 0;

		public bool HasSingleSymbol => _symList.Count == 1;

		public ArrayBuilder<Symbol> Symbols => _symList;

		public Symbol SingleSymbol => _symList[0];

		public bool IsClear
		{
			get
			{
				if (_kind == LookupResultKind.Empty && _symList.Count == 0)
				{
					return _diagInfo == null;
				}
				return false;
			}
		}

		private static ObjectPool<LookupResult> CreatePool()
		{
			ObjectPool<LookupResult> objectPool = null;
			objectPool = new ObjectPool<LookupResult>(() => new LookupResult(objectPool), 128);
			return objectPool;
		}

		private LookupResult(ObjectPool<LookupResult> pool)
			: this()
		{
			_pool = pool;
		}

		internal LookupResult()
		{
			_kind = LookupResultKind.Empty;
			_symList = new ArrayBuilder<Symbol>();
			_diagInfo = null;
		}

		public static LookupResult GetInstance()
		{
			return s_poolInstance.Allocate();
		}

		public void Free()
		{
			Clear();
			if (_pool != null)
			{
				_pool.Free(this);
			}
		}

		public void Clear()
		{
			_kind = LookupResultKind.Empty;
			_symList.Clear();
			_diagInfo = null;
		}

		private void SetFrom(LookupResultKind kind, Symbol sym, DiagnosticInfo diagInfo)
		{
			_kind = kind;
			_symList.Clear();
			if ((object)sym != null)
			{
				_symList.Add(sym);
			}
			_diagInfo = diagInfo;
		}

		public void SetFrom(SingleLookupResult other)
		{
			SetFrom(other.Kind, other.Symbol, other.Diagnostic);
		}

		public void SetFrom(LookupResult other)
		{
			_kind = other._kind;
			_symList.Clear();
			_symList.AddRange(other._symList);
			_diagInfo = other._diagInfo;
		}

		public void SetFrom(Symbol s)
		{
			SetFrom(SingleLookupResult.Good(s));
		}

		public void SetFrom(ImmutableArray<Symbol> syms, Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> generateAmbiguityDiagnostic)
		{
			if (syms.Length == 0)
			{
				Clear();
			}
			else if (syms.Length > 1)
			{
				DiagnosticInfo diagInfo = generateAmbiguityDiagnostic(syms);
				SetFrom(LookupResultKind.Ambiguous, syms[0], diagInfo);
			}
			else
			{
				SetFrom(SingleLookupResult.Good(syms[0]));
			}
		}

		public void MergePrioritized(LookupResult other)
		{
			if (other.Kind > Kind && Kind < LookupResultKind.Ambiguous)
			{
				SetFrom(other);
			}
		}

		public void MergePrioritized(SingleLookupResult other)
		{
			if (other.Kind > Kind && Kind < LookupResultKind.Ambiguous)
			{
				SetFrom(other);
			}
		}

		public void MergeAmbiguous(LookupResult other, Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> generateAmbiguityDiagnostic)
		{
			if (IsGoodOrAmbiguous && other.IsGoodOrAmbiguous)
			{
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				if (Diagnostic is AmbiguousSymbolDiagnostic)
				{
					instance.AddRange(((AmbiguousSymbolDiagnostic)Diagnostic).AmbiguousSymbols);
				}
				else
				{
					instance.AddRange(Symbols);
				}
				if (other.Diagnostic is AmbiguousSymbolDiagnostic)
				{
					instance.AddRange(((AmbiguousSymbolDiagnostic)other.Diagnostic).AmbiguousSymbols);
				}
				else
				{
					instance.AddRange(other.Symbols);
				}
				SetFrom(instance.ToImmutableAndFree(), generateAmbiguityDiagnostic);
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
		}

		public void MergeAmbiguous(SingleLookupResult other, Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> generateAmbiguityDiagnostic)
		{
			if (IsGoodOrAmbiguous && other.IsGoodOrAmbiguous)
			{
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				if (Diagnostic is AmbiguousSymbolDiagnostic)
				{
					instance.AddRange(((AmbiguousSymbolDiagnostic)Diagnostic).AmbiguousSymbols);
				}
				else
				{
					instance.AddRange(Symbols);
				}
				if (other.Diagnostic is AmbiguousSymbolDiagnostic)
				{
					instance.AddRange(((AmbiguousSymbolDiagnostic)other.Diagnostic).AmbiguousSymbols);
				}
				else
				{
					instance.Add(other.Symbol);
				}
				SetFrom(instance.ToImmutableAndFree(), generateAmbiguityDiagnostic);
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
		}

		public static bool CanOverload(Symbol sym1, Symbol sym2)
		{
			if (sym1.Kind == sym2.Kind && SymbolExtensions.IsOverloadable(sym1))
			{
				return SymbolExtensions.IsOverloadable(sym2);
			}
			return false;
		}

		public bool AllSymbolsHaveOverloads()
		{
			ArrayBuilder<Symbol>.Enumerator enumerator = Symbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.Method:
					if (!((MethodSymbol)current).IsOverloads)
					{
						return false;
					}
					break;
				case SymbolKind.Property:
					if (!((PropertySymbol)current).IsOverloads)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}

		public void MergeOverloadedOrPrioritizedExtensionMethods(SingleLookupResult other)
		{
			if (IsGood && other.IsGood)
			{
				_symList.Add(other.Symbol);
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
			else if (Kind == LookupResultKind.Inaccessible && Kind <= other.Kind)
			{
				_symList.Add(other.Symbol);
			}
		}

		public void MergeOverloadedOrPrioritized(LookupResult other, bool checkIfCurrentHasOverloads)
		{
			if (IsGoodOrAmbiguous && other.IsGoodOrAmbiguous)
			{
				if (IsGood && other.IsGood && CanOverload(Symbols[0], other.Symbols[0]) && (!checkIfCurrentHasOverloads || AllSymbolsHaveOverloads()))
				{
					_symList.AddRange(other.Symbols);
				}
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
			else if (Kind == LookupResultKind.Inaccessible && Kind <= other.Kind && CanOverload(Symbols[0], other.Symbols[0]) && (!checkIfCurrentHasOverloads || AllSymbolsHaveOverloads()))
			{
				_symList.AddRange(other.Symbols);
			}
		}

		public void MergeOverloadedOrPrioritized(SingleLookupResult other, bool checkIfCurrentHasOverloads)
		{
			if (IsGoodOrAmbiguous && other.IsGoodOrAmbiguous)
			{
				if (IsGood && other.IsGood && CanOverload(Symbols[0], other.Symbol) && (!checkIfCurrentHasOverloads || AllSymbolsHaveOverloads()))
				{
					_symList.Add(other.Symbol);
				}
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
			else if (Kind == LookupResultKind.Inaccessible && Kind <= other.Kind && CanOverload(Symbols[0], other.Symbol) && (!checkIfCurrentHasOverloads || AllSymbolsHaveOverloads()))
			{
				_symList.Add(other.Symbol);
			}
		}

		public void MergeMembersOfTheSameType(SingleLookupResult other, bool imported)
		{
			if (IsGoodOrAmbiguous && other.IsGood)
			{
				MergeOverloadedOrAmbiguousInTheSameType(other, imported);
			}
			else if (other.Kind > Kind)
			{
				SetFrom(other);
			}
			else
			{
				if (Kind != LookupResultKind.Inaccessible || Kind > other.Kind)
				{
					return;
				}
				if (!CanOverload(Symbols[0], other.Symbol))
				{
					if (Symbols.All((Symbol candidate, Symbol otherSymbol) => candidate.DeclaredAccessibility < otherSymbol.DeclaredAccessibility, other.Symbol))
					{
						SetFrom(other);
					}
				}
				else
				{
					_symList.Add(other.Symbol);
				}
			}
		}

		private void MergeOverloadedOrAmbiguousInTheSameType(SingleLookupResult other, bool imported)
		{
			if (IsGood)
			{
				if (CanOverload(Symbols[0], other.Symbol))
				{
					_symList.Add(other.Symbol);
					return;
				}
				if (imported)
				{
					int num = 0;
					int num2 = 0;
					bool flag = false;
					int num3 = _symList.Count - 1;
					for (int i = 0; i <= num3; i++)
					{
						int num4 = CompareAccessibilityOfSymbolsConflictingInSameContainer(_symList[i], other.Symbol);
						if (num4 == 0)
						{
							num2++;
						}
						else if (num4 < 0)
						{
							num++;
							_symList[i] = null;
						}
						else
						{
							flag = true;
						}
					}
					if (num == _symList.Count)
					{
						SetFrom(other);
						return;
					}
					if (flag && num2 > 0)
					{
						num += num2;
						num2 = RemoveAmbiguousSymbols(other.Symbol, num2);
					}
					CompactSymbols(num);
					if (flag || (_symList.Count == 1 && num2 == 1 && AreEquivalentEnumConstants(_symList[0], other.Symbol)))
					{
						return;
					}
				}
			}
			else if (imported)
			{
				int num5 = CompareAccessibilityOfSymbolsConflictingInSameContainer(_symList[0], other.Symbol);
				if (num5 < 0)
				{
					SetFrom(other);
					return;
				}
				if (num5 > 0)
				{
					return;
				}
			}
			MergeAmbiguous(other, s_ambiguousInTypeError);
		}

		private static bool AreEquivalentEnumConstants(Symbol symbol1, Symbol symbol2)
		{
			if (symbol1.Kind != SymbolKind.Field || symbol2.Kind != SymbolKind.Field || symbol1.ContainingType.TypeKind != TypeKind.Enum)
			{
				return false;
			}
			FieldSymbol fieldSymbol = (FieldSymbol)symbol1;
			FieldSymbol fieldSymbol2 = (FieldSymbol)symbol2;
			return fieldSymbol.ConstantValue != null && fieldSymbol.ConstantValue.Equals(RuntimeHelpers.GetObjectValue(fieldSymbol2.ConstantValue));
		}

		private void CompactSymbols(int lost)
		{
			if (lost <= 0)
			{
				return;
			}
			int num = _symList.Count - 1;
			int i;
			for (i = 0; i <= num && (object)_symList[i] != null; i++)
			{
			}
			int num2 = _symList.Count - lost;
			if (num2 > i)
			{
				int num3 = i + 1;
				int num4 = _symList.Count - 1;
				for (int j = num3; j <= num4; j++)
				{
					if ((object)_symList[j] != null)
					{
						_symList[i] = _symList[j];
						i++;
						if (i == num2)
						{
							break;
						}
					}
				}
			}
			_symList.Clip(num2);
		}

		private int RemoveAmbiguousSymbols(Symbol other, int ambiguous)
		{
			int num = _symList.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if ((object)_symList[i] != null && _symList[i].DeclaredAccessibility == other.DeclaredAccessibility)
				{
					_symList[i] = null;
					ambiguous--;
					if (ambiguous == 0)
					{
						break;
					}
				}
			}
			return ambiguous;
		}

		public static int CompareAccessibilityOfSymbolsConflictingInSameContainer(Symbol first, Symbol second)
		{
			if (first.DeclaredAccessibility == second.DeclaredAccessibility)
			{
				return 0;
			}
			if (first.DeclaredAccessibility < second.DeclaredAccessibility)
			{
				if (first.DeclaredAccessibility == Accessibility.Protected && second.DeclaredAccessibility == Accessibility.Internal)
				{
					return 1;
				}
				return -1;
			}
			if (second.DeclaredAccessibility == Accessibility.Protected && first.DeclaredAccessibility == Accessibility.Internal)
			{
				return -1;
			}
			return 1;
		}

		public void MergeMembersOfTheSameNamespace(SingleLookupResult other, ModuleSymbol sourceModule, LookupOptions options)
		{
			int num = ResolveAmbiguityInTheSameNamespace(other, sourceModule, options);
			if (num <= 0)
			{
				if (num < 0)
				{
					SetFrom(other);
				}
				else
				{
					MergeAmbiguous(other, s_ambiguousInNSError);
				}
			}
		}

		private static SymbolLocation GetSymbolLocation(Symbol sym, ModuleSymbol sourceModule, LookupOptions options)
		{
			if (sym.Kind == SymbolKind.Namespace)
			{
				return (!((NamespaceSymbol)sym).IsDeclaredInSourceModule(sourceModule)) ? SymbolLocation.FromReferencedAssembly : SymbolLocation.FromSourceModule;
			}
			if ((object)sym.ContainingModule == sourceModule)
			{
				return SymbolLocation.FromSourceModule;
			}
			if (sourceModule.DeclaringCompilation.Options.IgnoreCorLibraryDuplicatedTypes)
			{
				AssemblySymbol containingAssembly = sym.ContainingAssembly;
				if ((object)containingAssembly == containingAssembly.CorLibrary)
				{
					return SymbolLocation.FromCorLibrary;
				}
			}
			return SymbolLocation.FromReferencedAssembly;
		}

		private int ResolveAmbiguityInTheSameNamespace(SingleLookupResult other, ModuleSymbol sourceModule, LookupOptions options)
		{
			if (other.StopFurtherLookup && StopFurtherLookup && Symbols.Count > 0)
			{
				SymbolLocation symbolLocation = GetSymbolLocation(other.Symbol, sourceModule, options);
				SymbolLocation symbolLocation2 = GetSymbolLocation(Symbols[0], sourceModule, options);
				int num = symbolLocation - symbolLocation2;
				if (num != 0)
				{
					return num;
				}
			}
			if (other.IsGood)
			{
				if (IsGood)
				{
					return ResolveAmbiguityInTheSameNamespace(Symbols[0], other.Symbol, sourceModule);
				}
				if (IsAmbiguous)
				{
					ImmutableArray<Symbol>.Enumerator enumerator = ((AmbiguousSymbolDiagnostic)Diagnostic).AmbiguousSymbols.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						if (current.Kind == SymbolKind.Namespace)
						{
							return 0;
						}
						if (ResolveAmbiguityInTheSameNamespace(current, other.Symbol, sourceModule) >= 0)
						{
							return 0;
						}
					}
					return -1;
				}
			}
			return 0;
		}

		private static int ResolveAmbiguityInTheSameNamespace(Symbol first, Symbol second, ModuleSymbol sourceModule)
		{
			if ((object)first.ContainingSymbol == second.ContainingSymbol)
			{
				if ((object)first.ContainingModule == sourceModule)
				{
					return 0;
				}
				return CompareAccessibilityOfSymbolsConflictingInSameContainer(first, second);
			}
			if (first.Kind == SymbolKind.Namespace)
			{
				if ((object)second.ContainingModule == sourceModule && first.IsEmbedded == second.IsEmbedded)
				{
					return 0;
				}
				return ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace((NamespaceSymbol)first, second);
			}
			if (second.Kind == SymbolKind.Namespace)
			{
				if ((object)first.ContainingModule == sourceModule && first.IsEmbedded == second.IsEmbedded)
				{
					return 0;
				}
				return -1 * ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace((NamespaceSymbol)second, first);
			}
			return 0;
		}

		private static int ResolveAmbiguityBetweenTypeAndMergedNamespaceInTheSameNamespace(NamespaceSymbol possiblyMergedNamespace, Symbol type)
		{
			if (type.DeclaredAccessibility < Accessibility.Public && possiblyMergedNamespace.Extent.Kind != NamespaceKind.Module)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = ((NamespaceSymbol)type.ContainingSymbol).GetMembers(type.Name).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Kind == SymbolKind.Namespace)
					{
						return 1;
					}
				}
			}
			return 0;
		}

		public void ReplaceSymbol(Symbol newSym)
		{
			_symList.Clear();
			_symList.Add(newSym);
		}

		internal static LookupResultKind WorseResultKind(LookupResultKind resultKind1, LookupResultKind resultKind2)
		{
			if (resultKind1 == LookupResultKind.Empty)
			{
				return (resultKind2 != LookupResultKind.Good) ? resultKind2 : LookupResultKind.Empty;
			}
			if (resultKind2 == LookupResultKind.Empty)
			{
				return (resultKind1 != LookupResultKind.Good) ? resultKind1 : LookupResultKind.Empty;
			}
			if (resultKind1 < resultKind2)
			{
				return resultKind1;
			}
			return resultKind2;
		}
	}
}
