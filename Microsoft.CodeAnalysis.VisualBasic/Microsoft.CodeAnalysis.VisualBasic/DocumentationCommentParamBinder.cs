using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DocumentationCommentParamBinder : DocumentationCommentBinder
	{
		private const LookupOptions s_invalidLookupOptions = LookupOptions.AttributeTypeOnly | LookupOptions.LabelsOnly | LookupOptions.MustBeInstance | LookupOptions.MustNotBeInstance | LookupOptions.MustNotBeLocalOrParameter;

		private ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if ((object)CommentedSymbol != null)
				{
					switch (CommentedSymbol.Kind)
					{
					case SymbolKind.NamedType:
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)CommentedSymbol;
						if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
						{
							MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
							if ((object)delegateInvokeMethod != null)
							{
								return delegateInvokeMethod.Parameters;
							}
						}
						break;
					}
					case SymbolKind.Method:
						return ((MethodSymbol)CommentedSymbol).Parameters;
					case SymbolKind.Property:
						return ((PropertySymbol)CommentedSymbol).Parameters;
					case SymbolKind.Event:
						return ((EventSymbol)CommentedSymbol).DelegateParameters;
					}
				}
				return ImmutableArray<ParameterSymbol>.Empty;
			}
		}

		public DocumentationCommentParamBinder(Binder containingBinder, Symbol commentedSymbol)
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
			return DocumentationCommentBinder.FindSymbolInSymbolArray(valueText, Parameters);
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if ((options & (LookupOptions.AttributeTypeOnly | LookupOptions.LabelsOnly | LookupOptions.MustBeInstance | LookupOptions.MustNotBeInstance | LookupOptions.MustNotBeLocalOrParameter)) != 0)
			{
				return;
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, null))
				{
					nameSet.AddSymbol(current, current.Name, 0);
				}
			}
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((options & (LookupOptions.AttributeTypeOnly | LookupOptions.LabelsOnly | LookupOptions.MustBeInstance | LookupOptions.MustNotBeInstance | LookupOptions.MustNotBeLocalOrParameter)) != 0 || arity > 0)
			{
				return;
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (CaseInsensitiveComparison.Equals(current.Name, name))
				{
					lookupResult.SetFrom(CheckViability(current, arity, options, null, ref useSiteInfo));
				}
			}
		}
	}
}
