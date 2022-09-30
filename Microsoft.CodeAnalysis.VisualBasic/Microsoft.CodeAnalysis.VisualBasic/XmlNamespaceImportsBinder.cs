using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class XmlNamespaceImportsBinder : Binder
	{
		private readonly IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> _namespaces;

		internal override bool HasImportedXmlNamespaces => true;

		public XmlNamespaceImportsBinder(Binder containingBinder, IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> namespaces)
			: base(containingBinder)
		{
			_namespaces = namespaces;
		}

		internal override bool LookupXmlNamespace(string prefix, bool ignoreXmlNodes, out string @namespace, out bool fromImports)
		{
			XmlNamespaceAndImportsClausePosition value = default(XmlNamespaceAndImportsClausePosition);
			if (_namespaces.TryGetValue(prefix, out value))
			{
				@namespace = value.XmlNamespace;
				base.Compilation.MarkImportDirectiveAsUsed(base.SyntaxTree, value.ImportsClausePosition);
				fromImports = true;
				return true;
			}
			return base.LookupXmlNamespace(prefix, ignoreXmlNodes, out @namespace, out fromImports);
		}
	}
}
