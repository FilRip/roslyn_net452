using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DocumentationCommentTypeParamBinder : DocumentationCommentBinder
	{
		protected ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				if ((object)CommentedSymbol != null)
				{
					switch (CommentedSymbol.Kind)
					{
					case SymbolKind.NamedType:
						return ((NamedTypeSymbol)CommentedSymbol).TypeParameters;
					case SymbolKind.Method:
						return ((MethodSymbol)CommentedSymbol).TypeParameters;
					}
				}
				return ImmutableArray<TypeParameterSymbol>.Empty;
			}
		}

		public DocumentationCommentTypeParamBinder(Binder containingBinder, Symbol commentedSymbol)
			: base(containingBinder, commentedSymbol)
		{
		}

		internal override ImmutableArray<Symbol> BindXmlNameAttributeValue(IdentifierNameSyntax identifier, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((object)CommentedSymbol == null)
			{
				return ImmutableArray<Symbol>.Empty;
			}
			string valueText = identifier.Identifier.ValueText;
			if (string.IsNullOrEmpty(valueText))
			{
				return ImmutableArray<Symbol>.Empty;
			}
			return DocumentationCommentBinder.FindSymbolInSymbolArray(valueText, TypeParameters);
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if ((options & (LookupOptions.AttributeTypeOnly | LookupOptions.LabelsOnly | LookupOptions.MustBeInstance)) != 0)
			{
				return;
			}
			ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
			if (typeParameters.IsEmpty)
			{
				return;
			}
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				if (originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, null))
				{
					nameSet.AddSymbol(current, current.Name, 0);
				}
			}
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((options & (LookupOptions.AttributeTypeOnly | LookupOptions.LabelsOnly | LookupOptions.MustBeInstance)) != 0 || TypeParameters.IsEmpty)
			{
				return;
			}
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = TypeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				if (CaseInsensitiveComparison.Equals(current.Name, name))
				{
					lookupResult.SetFrom(CheckViability(current, arity, options, null, ref useSiteInfo));
				}
			}
		}
	}
}
