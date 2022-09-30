using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class XmlRootElementBinder : Binder
	{
		public XmlRootElementBinder(Binder containingBinder)
			: base(containingBinder)
		{
		}

		internal override void GetInScopeXmlNamespaces(ArrayBuilder<KeyValuePair<string, string>> builder)
		{
		}
	}
}
