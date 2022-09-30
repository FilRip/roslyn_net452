#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal sealed class GeneratorSyntaxWalker : SyntaxWalker
    {
        private readonly ISyntaxContextReceiver _syntaxReceiver;

        private SemanticModel? _semanticModel;

        internal GeneratorSyntaxWalker(ISyntaxContextReceiver syntaxReceiver)
        {
            _syntaxReceiver = syntaxReceiver;
        }

        public void VisitWithModel(SemanticModel model, SyntaxNode node)
        {
            _semanticModel = model;
            Visit(node);
            _semanticModel = null;
        }

        public override void Visit(SyntaxNode node)
        {
            _syntaxReceiver.OnVisitSyntaxNode(new GeneratorSyntaxContext(node, _semanticModel));
            base.Visit(node);
        }
    }
}
