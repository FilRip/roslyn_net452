using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal class VisualBasicLambdaSyntaxFacts : LambdaSyntaxFacts
	{
		public static readonly LambdaSyntaxFacts Instance = new VisualBasicLambdaSyntaxFacts();

		private VisualBasicLambdaSyntaxFacts()
		{
		}

		public override SyntaxNode GetLambda(SyntaxNode lambdaOrLambdaBodySyntax)
		{
			return LambdaUtilities.GetLambda(lambdaOrLambdaBodySyntax);
		}

		public override SyntaxNode TryGetCorrespondingLambdaBody(SyntaxNode previousLambdaSyntax, SyntaxNode lambdaOrLambdaBodySyntax)
		{
			return LambdaUtilities.GetCorrespondingLambdaBody(lambdaOrLambdaBodySyntax, previousLambdaSyntax);
		}

		public override int GetDeclaratorPosition(SyntaxNode node)
		{
			return node.SpanStart;
		}
	}
}
