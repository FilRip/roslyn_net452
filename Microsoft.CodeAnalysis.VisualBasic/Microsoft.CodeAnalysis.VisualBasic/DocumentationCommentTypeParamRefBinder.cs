using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DocumentationCommentTypeParamRefBinder : DocumentationCommentTypeParamBinder
	{
		public DocumentationCommentTypeParamRefBinder(Binder containingBinder, Symbol commentedSymbol)
			: base(containingBinder, commentedSymbol)
		{
		}

		internal override ImmutableArray<Symbol> BindXmlNameAttributeValue(IdentifierNameSyntax identifier, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<Symbol> result = base.BindXmlNameAttributeValue(identifier, ref useSiteInfo);
			if (!result.IsEmpty)
			{
				return result;
			}
			LookupResult instance = LookupResult.GetInstance();
			Lookup(instance, identifier.Identifier.ValueText, 0, LookupOptions.MustNotBeReturnValueVariable | LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility | LookupOptions.MustNotBeLocalOrParameter, ref useSiteInfo);
			if (!instance.HasSingleSymbol)
			{
				instance.Free();
				return default(ImmutableArray<Symbol>);
			}
			Symbol singleSymbol = instance.SingleSymbol;
			instance.Free();
			if (singleSymbol.Kind == SymbolKind.TypeParameter)
			{
				return ImmutableArray.Create(singleSymbol);
			}
			return ImmutableArray<Symbol>.Empty;
		}
	}
}
