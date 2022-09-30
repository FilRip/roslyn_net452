using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis
{
    internal sealed class AdditionalFileAddedEdit : AdditionalFileEdit
    {
        public AdditionalText AddedText { get; }

        public AdditionalFileAddedEdit(AdditionalText addedText)
        {
            AddedText = addedText;
        }

        internal override GeneratorDriverState Commit(GeneratorDriverState state)
        {
            ImmutableArray<AdditionalText>? additionalTexts = state.AdditionalTexts.Add(AddedText);
            return state.With(null, null, additionalTexts);
        }

        internal override bool AcceptedBy(GeneratorInfo info)
        {
            return info.EditCallback != null;
        }

        internal override bool TryApply(GeneratorInfo info, GeneratorEditContext context)
        {
            return info.EditCallback!(context, this);
        }
    }
}
