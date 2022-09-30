using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class DocumentationCommentBinder : Binder
	{
		internal enum BinderType
		{
			None,
			Cref,
			NameInTypeParamRef,
			NameInTypeParam,
			NameInParamOrParamRef
		}

		protected readonly Symbol CommentedSymbol;

		protected DocumentationCommentBinder(Binder containingBinder, Symbol commentedSymbol)
			: base(containingBinder)
		{
			CommentedSymbol = commentedSymbol;
		}

		[Conditional("DEBUG")]
		private static void CheckBinderSymbolRelationship(Binder containingBinder, Symbol commentedSymbol)
		{
			if ((object)commentedSymbol != null)
			{
				NamedTypeSymbol namedTypeSymbol = commentedSymbol as NamedTypeSymbol;
				_ = containingBinder.ContainingMember;
				if ((object)namedTypeSymbol == null || namedTypeSymbol.TypeKind == TypeKind.Delegate)
				{
					_ = commentedSymbol.ContainingType;
				}
			}
		}

		public static bool IsIntrinsicTypeForDocumentationComment(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
				return true;
			default:
				return false;
			}
		}

		internal static BinderType GetBinderTypeForNameAttribute(BaseXmlAttributeSyntax node)
		{
			return GetBinderTypeForNameAttribute(GetParentXmlElementName(node));
		}

		internal static BinderType GetBinderTypeForNameAttribute(string parentNodeName)
		{
			if (parentNodeName != null)
			{
				if (DocumentationCommentXmlNames.ElementEquals(parentNodeName, "param", fromVb: true) || DocumentationCommentXmlNames.ElementEquals(parentNodeName, "paramref", fromVb: true))
				{
					return BinderType.NameInParamOrParamRef;
				}
				if (DocumentationCommentXmlNames.ElementEquals(parentNodeName, "typeparam", fromVb: true))
				{
					return BinderType.NameInTypeParam;
				}
				if (DocumentationCommentXmlNames.ElementEquals(parentNodeName, "typeparamref", fromVb: true))
				{
					return BinderType.NameInTypeParamRef;
				}
			}
			return BinderType.None;
		}

		internal static string GetParentXmlElementName(BaseXmlAttributeSyntax attr)
		{
			VisualBasicSyntaxNode parent = attr.Parent;
			if (parent == null)
			{
				return null;
			}
			switch (parent.Kind())
			{
			case SyntaxKind.XmlEmptyElement:
			{
				XmlEmptyElementSyntax xmlEmptyElementSyntax = (XmlEmptyElementSyntax)parent;
				if (xmlEmptyElementSyntax.Name.Kind() != SyntaxKind.XmlName)
				{
					return null;
				}
				return ((XmlNameSyntax)xmlEmptyElementSyntax.Name).LocalName.ValueText;
			}
			case SyntaxKind.XmlElementStartTag:
			{
				XmlElementStartTagSyntax xmlElementStartTagSyntax = (XmlElementStartTagSyntax)parent;
				if (xmlElementStartTagSyntax.Name.Kind() != SyntaxKind.XmlName)
				{
					return null;
				}
				return ((XmlNameSyntax)xmlElementStartTagSyntax.Name).LocalName.ValueText;
			}
			default:
				return null;
			}
		}

		internal override ImmutableArray<Symbol> BindXmlNameAttributeValue(IdentifierNameSyntax identifier, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(TypeSyntax name, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(CrefReferenceSyntax reference, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected static ImmutableArray<Symbol> FindSymbolInSymbolArray<TSymbol>(string name, ImmutableArray<TSymbol> symbols) where TSymbol : Symbol
		{
			if (!symbols.IsEmpty)
			{
				ImmutableArray<TSymbol>.Enumerator enumerator = symbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TSymbol current = enumerator.Current;
					if (CaseInsensitiveComparison.Equals(name, current.Name))
					{
						return ImmutableArray.Create((Symbol)current);
					}
				}
			}
			return ImmutableArray<Symbol>.Empty;
		}

		internal override LookupOptions BinderSpecificLookupOptions(LookupOptions options)
		{
			return base.ContainingBinder.BinderSpecificLookupOptions(options) | LookupOptions.UseBaseReferenceAccessibility;
		}

		protected static void RemoveOverriddenMethodsAndProperties(ArrayBuilder<Symbol> symbols)
		{
			if (symbols == null || symbols.Count < 2)
			{
				return;
			}
			Dictionary<Symbol, int> dictionary = null;
			int num = symbols.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Symbol symbol = symbols[i];
				SymbolKind kind = symbol.Kind;
				if (kind == SymbolKind.Method || kind == SymbolKind.Property)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<Symbol, int>();
					}
					dictionary.Add(symbol.OriginalDefinition, i);
				}
			}
			if (dictionary == null)
			{
				return;
			}
			ArrayBuilder<int> arrayBuilder = null;
			int num2 = symbols.Count - 1;
			for (int j = 0; j <= num2; j++)
			{
				int value = -1;
				Symbol symbol2 = symbols[j];
				switch (symbol2.Kind)
				{
				case SymbolKind.Method:
				{
					MethodSymbol methodSymbol = (MethodSymbol)symbol2.OriginalDefinition;
					while (true)
					{
						methodSymbol = methodSymbol.OverriddenMethod;
						if ((object)methodSymbol == null)
						{
							break;
						}
						if (dictionary.TryGetValue(methodSymbol, out value))
						{
							if (arrayBuilder == null)
							{
								arrayBuilder = ArrayBuilder<int>.GetInstance();
							}
							arrayBuilder.Add(value);
						}
					}
					break;
				}
				case SymbolKind.Property:
				{
					PropertySymbol propertySymbol = (PropertySymbol)symbol2.OriginalDefinition;
					while (true)
					{
						propertySymbol = propertySymbol.OverriddenProperty;
						if ((object)propertySymbol == null)
						{
							break;
						}
						if (dictionary.TryGetValue(propertySymbol, out value))
						{
							if (arrayBuilder == null)
							{
								arrayBuilder = ArrayBuilder<int>.GetInstance();
							}
							arrayBuilder.Add(value);
						}
					}
					break;
				}
				}
			}
			if (arrayBuilder == null)
			{
				return;
			}
			int num3 = arrayBuilder.Count - 1;
			for (int k = 0; k <= num3; k++)
			{
				symbols[arrayBuilder[k]] = null;
			}
			int num4 = 0;
			int num5 = symbols.Count - 1;
			for (int l = 0; l <= num5; l++)
			{
				Symbol symbol3 = symbols[l];
				if ((object)symbol3 != null)
				{
					symbols[num4] = symbol3;
					num4++;
				}
			}
			symbols.Clip(num4);
			arrayBuilder.Free();
		}
	}
}
