#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class LambdaSyntaxFacts
    {
        public abstract SyntaxNode GetLambda(SyntaxNode lambdaOrLambdaBodySyntax);

        public abstract SyntaxNode? TryGetCorrespondingLambdaBody(SyntaxNode previousLambdaSyntax, SyntaxNode lambdaOrLambdaBodySyntax);

        public abstract int GetDeclaratorPosition(SyntaxNode node);
    }
}
