using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class XmlEmbeddedExpressionBinder : Binder
	{
		public XmlEmbeddedExpressionBinder(Binder containingBinder)
			: base(containingBinder)
		{
		}

		internal override bool LookupXmlNamespace(string prefix, bool ignoreXmlNodes, out string @namespace, out bool fromImports)
		{
			return base.LookupXmlNamespace(prefix, ignoreXmlNodes: true, out @namespace, out fromImports);
		}

		internal override void GetInScopeXmlNamespaces(ArrayBuilder<KeyValuePair<string, string>> builder)
		{
		}
	}
}
