namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct XmlNamespaceAndImportsClausePosition
	{
		public readonly string XmlNamespace;

		public readonly int ImportsClausePosition;

		public XmlNamespaceAndImportsClausePosition(string xmlNamespace, int importsClausePosition)
		{
			this = default(XmlNamespaceAndImportsClausePosition);
			XmlNamespace = xmlNamespace;
			ImportsClausePosition = importsClausePosition;
		}
	}
}
