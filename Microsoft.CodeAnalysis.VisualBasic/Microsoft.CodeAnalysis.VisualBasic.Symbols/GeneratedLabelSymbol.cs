using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class GeneratedLabelSymbol : LabelSymbol
	{
		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public GeneratedLabelSymbol(string name)
			: base(name)
		{
		}
	}
}
