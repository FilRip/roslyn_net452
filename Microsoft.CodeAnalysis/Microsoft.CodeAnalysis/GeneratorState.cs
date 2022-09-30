using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorState
    {
        internal static GeneratorState Uninitialized;

        internal ImmutableArray<GeneratedSyntaxTree> PostInitTrees { get; }

        internal ImmutableArray<GeneratedSyntaxTree> GeneratedTrees { get; }

        internal GeneratorInfo Info { get; }

        internal ISyntaxContextReceiver? SyntaxReceiver { get; }

        internal Exception? Exception { get; }

        internal ImmutableArray<Diagnostic> Diagnostics { get; }

        public GeneratorState(GeneratorInfo info)
            : this(info, ImmutableArray<GeneratedSyntaxTree>.Empty, ImmutableArray<GeneratedSyntaxTree>.Empty, ImmutableArray<Diagnostic>.Empty, null, null)
        {
        }

        public GeneratorState(GeneratorInfo info, ImmutableArray<GeneratedSyntaxTree> postInitTrees)
            : this(info, postInitTrees, ImmutableArray<GeneratedSyntaxTree>.Empty, ImmutableArray<Diagnostic>.Empty, null, null)
        {
        }

        public GeneratorState(GeneratorInfo info, Exception e, Diagnostic error)
            : this(info, ImmutableArray<GeneratedSyntaxTree>.Empty, ImmutableArray<GeneratedSyntaxTree>.Empty, ImmutableArray.Create(error), null, e)
        {
        }

        public GeneratorState(GeneratorInfo info, ImmutableArray<GeneratedSyntaxTree> postInitTrees, ImmutableArray<GeneratedSyntaxTree> generatedTrees, ImmutableArray<Diagnostic> diagnostics)
            : this(info, postInitTrees, generatedTrees, diagnostics, null, null)
        {
        }

        private GeneratorState(GeneratorInfo info, ImmutableArray<GeneratedSyntaxTree> postInitTrees, ImmutableArray<GeneratedSyntaxTree> generatedTrees, ImmutableArray<Diagnostic> diagnostics, ISyntaxContextReceiver? syntaxReceiver, Exception? exception)
        {
            PostInitTrees = postInitTrees;
            GeneratedTrees = generatedTrees;
            Info = info;
            Diagnostics = diagnostics;
            SyntaxReceiver = syntaxReceiver;
            Exception = exception;
        }

        internal GeneratorState WithReceiver(ISyntaxContextReceiver syntaxReceiver)
        {
            return new GeneratorState(Info, PostInitTrees, GeneratedTrees, Diagnostics, syntaxReceiver, null);
        }
    }
}
