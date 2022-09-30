using System;
using System.Threading;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct GeneratorInitializationContext
    {
        public CancellationToken CancellationToken { get; }

        internal GeneratorInfo.Builder InfoBuilder { get; }

        internal GeneratorInitializationContext(CancellationToken cancellationToken = default(CancellationToken))
        {
            CancellationToken = cancellationToken;
            InfoBuilder = new GeneratorInfo.Builder();
        }

        internal void RegisterForAdditionalFileChanges(EditCallback<AdditionalFileEdit> callback)
        {
            CheckIsEmpty(InfoBuilder.EditCallback);
            InfoBuilder.EditCallback = callback;
        }

        public void RegisterForSyntaxNotifications(SyntaxReceiverCreator receiverCreator)
        {
            CheckIsEmpty(InfoBuilder.SyntaxContextReceiverCreator, "SyntaxReceiverCreator / SyntaxContextReceiverCreator");
            InfoBuilder.SyntaxContextReceiverCreator = SyntaxContextReceiverAdaptor.Create(receiverCreator);
        }

        public void RegisterForSyntaxNotifications(SyntaxContextReceiverCreator receiverCreator)
        {
            CheckIsEmpty(InfoBuilder.SyntaxContextReceiverCreator, "SyntaxReceiverCreator / SyntaxContextReceiverCreator");
            InfoBuilder.SyntaxContextReceiverCreator = receiverCreator;
        }

        public void RegisterForPostInitialization(Action<GeneratorPostInitializationContext> callback)
        {
            CheckIsEmpty(InfoBuilder.PostInitCallback);
            InfoBuilder.PostInitCallback = callback;
        }

        private static void CheckIsEmpty<T>(T x, string? typeName = null) where T : class?
        {
            if (x != null)
            {
                throw new InvalidOperationException(string.Format(CodeAnalysisResources.Single_type_per_generator_0, typeName ?? typeof(T).Name));
            }
        }
    }
}
