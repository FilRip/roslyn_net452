using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratorInfo
    {
        internal class Builder
        {
            internal EditCallback<AdditionalFileEdit>? EditCallback { get; set; }

            internal SyntaxContextReceiverCreator? SyntaxContextReceiverCreator { get; set; }

            internal Action<GeneratorPostInitializationContext>? PostInitCallback { get; set; }

            public GeneratorInfo ToImmutable()
            {
                return new GeneratorInfo(EditCallback, SyntaxContextReceiverCreator, PostInitCallback);
            }
        }

        internal EditCallback<AdditionalFileEdit>? EditCallback { get; }

        internal SyntaxContextReceiverCreator? SyntaxContextReceiverCreator { get; }

        internal Action<GeneratorPostInitializationContext>? PostInitCallback { get; }

        internal bool Initialized { get; }

        public GeneratorInfo(EditCallback<AdditionalFileEdit>? editCallback, SyntaxContextReceiverCreator? receiverCreator, Action<GeneratorPostInitializationContext>? postInitCallback)
        {
            EditCallback = editCallback;
            SyntaxContextReceiverCreator = receiverCreator;
            PostInitCallback = postInitCallback;
            Initialized = true;
        }
    }
}
