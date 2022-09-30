using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class EmbeddedSymbolManager
	{
		internal sealed class EmbeddedNamedTypeSymbol : SourceNamedTypeSymbol
		{
			private readonly EmbeddedSymbolKind _kind;

			public override bool IsImplicitlyDeclared => true;

			internal override bool AreMembersImplicitlyDeclared => true;

			internal override EmbeddedSymbolKind EmbeddedSymbolKind => _kind;

			public EmbeddedNamedTypeSymbol(MergedTypeDeclaration decl, NamespaceOrTypeSymbol containingSymbol, SourceModuleSymbol containingModule, EmbeddedSymbolKind kind)
				: base(decl, containingSymbol, containingModule)
			{
				_kind = kind;
			}

			internal override ImmutableArray<Symbol> GetMembersForCci()
			{
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
				EmbeddedSymbolManager embeddedSymbolManager = DeclaringCompilation.EmbeddedSymbolManager;
				ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (embeddedSymbolManager.IsSymbolReferenced(current))
					{
						instance.Add(current);
					}
				}
				return instance.ToImmutableAndFree();
			}
		}

		internal readonly Func<Symbol, bool> IsReferencedPredicate;

		private readonly EmbeddedSymbolKind _embedded;

		private readonly ConcurrentDictionary<Symbol, bool> _symbols;

		private int _sealed;

		private bool _standardModuleAttributeReferenced;

		private static SyntaxTree s_embeddedSyntax = null;

		private static SyntaxTree s_vbCoreSyntax = null;

		private static SyntaxTree s_internalXmlHelperSyntax = null;

		public EmbeddedSymbolKind Embedded => _embedded;

		public bool IsAnySymbolReferenced
		{
			get
			{
				if (_symbols != null)
				{
					return !_symbols.IsEmpty;
				}
				return false;
			}
		}

		public static SyntaxTree EmbeddedSyntax
		{
			get
			{
				if (s_embeddedSyntax == null)
				{
					Interlocked.CompareExchange(ref s_embeddedSyntax, VisualBasicSyntaxTree.ParseText(EmbeddedResources.Embedded), null);
					if (s_embeddedSyntax.GetDiagnostics().Any())
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
				return s_embeddedSyntax;
			}
		}

		public static SyntaxTree VbCoreSyntaxTree
		{
			get
			{
				if (s_vbCoreSyntax == null)
				{
					Interlocked.CompareExchange(ref s_vbCoreSyntax, VisualBasicSyntaxTree.ParseText(EmbeddedResources.VbCoreSourceText), null);
					if (s_vbCoreSyntax.GetDiagnostics().Any())
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
				return s_vbCoreSyntax;
			}
		}

		public static SyntaxTree InternalXmlHelperSyntax
		{
			get
			{
				if (s_internalXmlHelperSyntax == null)
				{
					Interlocked.CompareExchange(ref s_internalXmlHelperSyntax, VisualBasicSyntaxTree.ParseText(EmbeddedResources.InternalXmlHelper), null);
					if (s_internalXmlHelperSyntax.GetDiagnostics().Any())
					{
						throw ExceptionUtilities.Unreachable;
					}
				}
				return s_internalXmlHelperSyntax;
			}
		}

		public EmbeddedSymbolManager(EmbeddedSymbolKind embedded)
		{
			IsReferencedPredicate = (Symbol t) => !t.IsEmbedded || IsSymbolReferenced(t);
			_sealed = 0;
			_standardModuleAttributeReferenced = false;
			_embedded = embedded;
			if ((embedded & EmbeddedSymbolKind.All) != 0)
			{
				_symbols = new ConcurrentDictionary<Symbol, bool>(ReferenceEqualityComparer.Instance);
			}
		}

		public void RegisterModuleDeclaration()
		{
			if ((_embedded & EmbeddedSymbolKind.VbCore) != 0)
			{
				_standardModuleAttributeReferenced = true;
			}
		}

		public void MarkAllDeferredSymbolsAsReferenced(VisualBasicCompilation compilation)
		{
			if (_standardModuleAttributeReferenced)
			{
				MarkSymbolAsReferenced(compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute));
			}
		}

		[Conditional("DEBUG")]
		internal void AssertMarkAllDeferredSymbolsAsReferencedIsCalled()
		{
		}

		internal void GetCurrentReferencedSymbolsSnapshot(ArrayBuilder<Symbol> builder, ConcurrentSet<Symbol> filter)
		{
			KeyValuePair<Symbol, bool>[] array = _symbols.ToArray();
			for (int i = 0; i < array.Length; i = checked(i + 1))
			{
				KeyValuePair<Symbol, bool> keyValuePair = array[i];
				if (!filter.Contains(keyValuePair.Key))
				{
					builder.Add(keyValuePair.Key);
				}
			}
		}

		public void MarkSymbolAsReferenced(Symbol symbol, ConcurrentSet<Symbol> allSymbols)
		{
			if (_sealed == 0)
			{
				AddReferencedSymbolWithDependents(symbol, allSymbols);
			}
		}

		public void MarkSymbolAsReferenced(Symbol symbol)
		{
			MarkSymbolAsReferenced(symbol, new ConcurrentSet<Symbol>(ReferenceEqualityComparer.Instance));
		}

		public bool IsSymbolReferenced(Symbol symbol)
		{
			ConcurrentDictionary<Symbol, bool> symbols = _symbols;
			bool value = false;
			return symbols.TryGetValue(symbol, out value);
		}

		public void SealCollection()
		{
			Interlocked.CompareExchange(ref _sealed, 1, 0);
		}

		private void AddReferencedSymbolWithDependents(Symbol symbol, ConcurrentSet<Symbol> allSymbols)
		{
			if (!symbol.IsEmbedded || allSymbols.Contains(symbol))
			{
				return;
			}
			switch (symbol.Kind)
			{
			case SymbolKind.Field:
				AddReferencedSymbolRaw(symbol, allSymbols);
				AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols);
				break;
			case SymbolKind.Method:
			{
				AddReferencedSymbolRaw(symbol, allSymbols);
				AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols);
				MethodKind methodKind2 = ((MethodSymbol)symbol).MethodKind;
				switch (methodKind2)
				{
				case MethodKind.PropertyGet:
				case MethodKind.PropertySet:
					AddReferencedSymbolWithDependents(((MethodSymbol)symbol).AssociatedSymbol, allSymbols);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(methodKind2);
				case MethodKind.Constructor:
				case MethodKind.Ordinary:
				case MethodKind.StaticConstructor:
					break;
				}
				break;
			}
			case SymbolKind.Property:
			{
				AddReferencedSymbolRaw(symbol, allSymbols);
				AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols);
				PropertySymbol propertySymbol = (PropertySymbol)symbol;
				if ((object)propertySymbol.GetMethod != null)
				{
					AddReferencedSymbolWithDependents(propertySymbol.GetMethod, allSymbols);
				}
				if ((object)propertySymbol.SetMethod != null)
				{
					AddReferencedSymbolWithDependents(propertySymbol.SetMethod, allSymbols);
				}
				break;
			}
			case SymbolKind.NamedType:
			{
				AddReferencedSymbolRaw(symbol, allSymbols);
				ImmutableArray<Symbol>.Enumerator enumerator = ((NamedTypeSymbol)symbol).GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					switch (current.Kind)
					{
					case SymbolKind.Field:
						if (!((FieldSymbol)current).IsConst)
						{
							AddReferencedSymbolRaw(current, allSymbols);
						}
						break;
					case SymbolKind.Method:
					{
						MethodKind methodKind = ((MethodSymbol)current).MethodKind;
						if (methodKind == MethodKind.Constructor || methodKind == MethodKind.StaticConstructor)
						{
							AddReferencedSymbolRaw(current, allSymbols);
						}
						break;
					}
					}
				}
				if ((object)symbol.ContainingType != null)
				{
					AddReferencedSymbolWithDependents(symbol.ContainingType, allSymbols);
				}
				break;
			}
			}
		}

		private void AddReferencedSymbolRaw(Symbol symbol, ConcurrentSet<Symbol> allSymbols)
		{
			if (allSymbols.Add(symbol))
			{
				if (_sealed == 0)
				{
					_symbols.TryAdd(symbol, value: true);
				}
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = symbol.GetAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					AddReferencedSymbolWithDependents(current.AttributeClass, allSymbols);
				}
			}
		}

		[Conditional("DEBUG")]
		private static void ValidateType(NamedTypeSymbol type)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembers().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.Field:
				case SymbolKind.Method:
				case SymbolKind.NamedType:
				case SymbolKind.Property:
					continue;
				}
				throw ExceptionUtilities.UnexpectedValue(current.Kind);
			}
		}

		[Conditional("DEBUG")]
		private static void ValidateField(FieldSymbol field)
		{
			_ = field.Type;
		}

		[Conditional("DEBUG")]
		internal static void ValidateMethod(MethodSymbol method)
		{
			_ = method.MethodKind;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_ = enumerator.Current;
			}
		}

		internal static EmbeddedSymbolKind GetEmbeddedKind(SyntaxTree tree)
		{
			if (tree == null)
			{
				return EmbeddedSymbolKind.None;
			}
			if (tree == s_embeddedSyntax)
			{
				return EmbeddedSymbolKind.EmbeddedAttribute;
			}
			if (tree == s_vbCoreSyntax)
			{
				return EmbeddedSymbolKind.VbCore;
			}
			if (tree == s_internalXmlHelperSyntax)
			{
				return EmbeddedSymbolKind.XmlHelper;
			}
			return EmbeddedSymbolKind.None;
		}

		internal static SyntaxTree GetEmbeddedTree(EmbeddedSymbolKind kind)
		{
			return kind switch
			{
				EmbeddedSymbolKind.EmbeddedAttribute => EmbeddedSyntax, 
				EmbeddedSymbolKind.VbCore => VbCoreSyntaxTree, 
				EmbeddedSymbolKind.XmlHelper => InternalXmlHelperSyntax, 
				_ => throw ExceptionUtilities.UnexpectedValue(kind), 
			};
		}
	}
}
