namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedMyGroupCollectionPropertyGetAccessorSymbol : SynthesizedMyGroupCollectionPropertyAccessorSymbol
	{
		public override bool IsSub => false;

		public override MethodKind MethodKind => MethodKind.PropertyGet;

		public override TypeSymbol ReturnType => base.PropertyOrEvent.Type;

		public SynthesizedMyGroupCollectionPropertyGetAccessorSymbol(SourceNamedTypeSymbol container, SynthesizedMyGroupCollectionPropertySymbol property, string createMethod)
			: base(container, property, createMethod)
		{
		}

		protected override string GetMethodBlock(string fieldName, string createMethodName, string targetTypeName)
		{
			return "Get\r\n" + fieldName + " = " + createMethodName + "(Of " + targetTypeName + ")(" + fieldName + ")\r\nReturn " + fieldName + "\r\nEnd Get\r\n";
		}
	}
}
