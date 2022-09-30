using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedParameterSymbolWithCustomModifiers : SynthesizedParameterSymbol
	{
		private readonly ImmutableArray<CustomModifier> _customModifiers;

		private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

		public sealed override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

		public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

		public SynthesizedParameterSymbolWithCustomModifiers(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
			: base(container, type, ordinal, isByRef, name, isOptional: false, null)
		{
			_customModifiers = customModifiers;
			_refCustomModifiers = refCustomModifiers;
		}
	}
}
