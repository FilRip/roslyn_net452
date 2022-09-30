using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal class CSharpLambdaSyntaxFacts : LambdaSyntaxFacts
    {
        public static readonly LambdaSyntaxFacts Instance = new CSharpLambdaSyntaxFacts();

        private CSharpLambdaSyntaxFacts()
        {
        }

        public override SyntaxNode GetLambda(SyntaxNode lambdaOrLambdaBodySyntax)
        {
            return LambdaUtilities.GetLambda(lambdaOrLambdaBodySyntax);
        }

        public override SyntaxNode? TryGetCorrespondingLambdaBody(SyntaxNode previousLambdaSyntax, SyntaxNode lambdaOrLambdaBodySyntax)
        {
            return LambdaUtilities.TryGetCorrespondingLambdaBody(lambdaOrLambdaBodySyntax, previousLambdaSyntax);
        }

        public override int GetDeclaratorPosition(SyntaxNode node)
        {
            return LambdaUtilities.GetDeclaratorPosition(node);
        }
    }
}
