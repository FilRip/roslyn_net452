using System;

namespace Microsoft.CodeAnalysis
{
    internal sealed class SyntaxContextReceiverAdaptor : ISyntaxContextReceiver
    {
        public ISyntaxReceiver Receiver { get; }

        private SyntaxContextReceiverAdaptor(ISyntaxReceiver receiver)
        {
            Receiver = receiver ?? throw new ArgumentNullException("receiver");
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            Receiver.OnVisitSyntaxNode(context.Node);
        }

        public static SyntaxContextReceiverCreator Create(SyntaxReceiverCreator creator)
        {
            SyntaxReceiverCreator creator2 = creator;
            return delegate
            {
                ISyntaxReceiver syntaxReceiver = creator2();
                return (syntaxReceiver != null) ? new SyntaxContextReceiverAdaptor(syntaxReceiver) : null;
            };
        }
    }
}
