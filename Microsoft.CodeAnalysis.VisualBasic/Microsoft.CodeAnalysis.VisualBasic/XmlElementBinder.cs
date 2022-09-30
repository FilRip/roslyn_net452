using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class XmlElementBinder : Binder
	{
		private readonly Dictionary<string, string> _namespaces;

		public XmlElementBinder(Binder containingBinder, Dictionary<string, string> namespaces)
			: base(containingBinder)
		{
			_namespaces = namespaces;
		}

		internal override bool LookupXmlNamespace(string prefix, bool ignoreXmlNodes, out string @namespace, out bool fromImports)
		{
			if (!ignoreXmlNodes && _namespaces.TryGetValue(prefix, out @namespace))
			{
				fromImports = false;
				return true;
			}
			return base.LookupXmlNamespace(prefix, ignoreXmlNodes, out @namespace, out fromImports);
		}

		internal override void GetInScopeXmlNamespaces(ArrayBuilder<KeyValuePair<string, string>> builder)
		{
			builder.AddRange(_namespaces);
			base.ContainingBinder.GetInScopeXmlNamespaces(builder);
		}
	}
}
