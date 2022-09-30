using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CompilationStartedEvent : CompilationEvent
    {
        public ImmutableArray<AdditionalText> AdditionalFiles { get; }

        private CompilationStartedEvent(Compilation compilation, ImmutableArray<AdditionalText> additionalFiles)
            : base(compilation)
        {
            AdditionalFiles = additionalFiles;
        }

        public CompilationStartedEvent(Compilation compilation)
            : this(compilation, ImmutableArray<AdditionalText>.Empty)
        {
        }

        public override string ToString()
        {
            return "CompilationStartedEvent";
        }

        public CompilationStartedEvent WithAdditionalFiles(ImmutableArray<AdditionalText> additionalFiles)
        {
            return new CompilationStartedEvent(base.Compilation, additionalFiles);
        }
    }
}
