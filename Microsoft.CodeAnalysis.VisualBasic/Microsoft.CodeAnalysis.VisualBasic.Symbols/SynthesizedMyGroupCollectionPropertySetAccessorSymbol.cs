using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedMyGroupCollectionPropertySetAccessorSymbol : SynthesizedMyGroupCollectionPropertyAccessorSymbol
	{
		private readonly ImmutableArray<ParameterSymbol> _parameters;

		public override bool IsSub => true;

		public override MethodKind MethodKind => MethodKind.PropertySet;

		public override TypeSymbol ReturnType => ContainingAssembly.GetSpecialType(SpecialType.System_Void);

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public SynthesizedMyGroupCollectionPropertySetAccessorSymbol(SourceNamedTypeSymbol container, SynthesizedMyGroupCollectionPropertySymbol property, string disposeMethod)
			: base(container, property, disposeMethod)
		{
			ParameterSymbol[] items = new ParameterSymbol[1] { SynthesizedParameterSymbol.CreateSetAccessorValueParameter(this, property, "Value") };
			_parameters = items.AsImmutableOrNull();
		}

		protected override string GetMethodBlock(string fieldName, string disposeMethodName, string targetTypeName)
		{
			return "Set(ByVal Value As " + targetTypeName + ")\r\nIf Value Is " + fieldName + "\r\nReturn\r\nEnd If\r\nIf Value IsNot Nothing Then\r\nThrow New Global.System.ArgumentException(\"Property can only be set to Nothing\")\r\nEnd If\r\n" + disposeMethodName + "(Of " + targetTypeName + ")(" + fieldName + ")\r\nEnd Set\r\n";
		}
	}
}
