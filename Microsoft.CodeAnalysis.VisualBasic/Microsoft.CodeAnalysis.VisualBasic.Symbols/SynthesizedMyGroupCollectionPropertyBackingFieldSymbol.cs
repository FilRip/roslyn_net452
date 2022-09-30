using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedMyGroupCollectionPropertyBackingFieldSymbol : SynthesizedFieldSymbol
	{
		public SynthesizedMyGroupCollectionPropertyBackingFieldSymbol(NamedTypeSymbol containingType, Symbol implicitlyDefinedBy, TypeSymbol type, string name)
			: base(containingType, implicitlyDefinedBy, type, name, Accessibility.Public)
		{
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return LexicalSortKey.NotInSource;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeEditorBrowsableNeverAttribute());
		}
	}
}
