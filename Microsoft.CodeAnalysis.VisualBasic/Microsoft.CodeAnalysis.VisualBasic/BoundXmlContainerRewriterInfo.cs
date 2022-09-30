using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlContainerRewriterInfo
	{
		public readonly bool IsRoot;

		public readonly BoundRValuePlaceholder Placeholder;

		public readonly BoundExpression ObjectCreation;

		public readonly BoundRValuePlaceholder XmlnsAttributesPlaceholder;

		public readonly BoundExpression XmlnsAttributes;

		public readonly BoundRValuePlaceholder PrefixesPlaceholder;

		public readonly BoundRValuePlaceholder NamespacesPlaceholder;

		public readonly ImmutableArray<KeyValuePair<string, string>> ImportedNamespaces;

		public readonly ImmutableArray<KeyValuePair<string, string>> InScopeXmlNamespaces;

		public readonly ImmutableArray<BoundExpression> SideEffects;

		public readonly bool HasErrors;

		public BoundXmlContainerRewriterInfo(BoundExpression objectCreation)
		{
			ObjectCreation = objectCreation;
			SideEffects = ImmutableArray<BoundExpression>.Empty;
			HasErrors = objectCreation.HasErrors;
		}

		public BoundXmlContainerRewriterInfo(bool isRoot, BoundRValuePlaceholder placeholder, BoundExpression objectCreation, BoundRValuePlaceholder xmlnsAttributesPlaceholder, BoundExpression xmlnsAttributes, BoundRValuePlaceholder prefixesPlaceholder, BoundRValuePlaceholder namespacesPlaceholder, ImmutableArray<KeyValuePair<string, string>> importedNamespaces, ImmutableArray<KeyValuePair<string, string>> inScopeXmlNamespaces, ImmutableArray<BoundExpression> sideEffects)
		{
			IsRoot = isRoot;
			Placeholder = placeholder;
			ObjectCreation = objectCreation;
			XmlnsAttributesPlaceholder = xmlnsAttributesPlaceholder;
			XmlnsAttributes = xmlnsAttributes;
			PrefixesPlaceholder = prefixesPlaceholder;
			NamespacesPlaceholder = namespacesPlaceholder;
			ImportedNamespaces = importedNamespaces;
			InScopeXmlNamespaces = inScopeXmlNamespaces;
			SideEffects = sideEffects;
			HasErrors = objectCreation.HasErrors || sideEffects.Any((BoundExpression s) => s.HasErrors);
		}
	}
}
