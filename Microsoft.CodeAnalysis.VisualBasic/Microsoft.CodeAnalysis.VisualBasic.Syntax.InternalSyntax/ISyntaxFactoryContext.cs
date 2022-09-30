namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal interface ISyntaxFactoryContext
	{
		bool IsWithinAsyncMethodOrLambda { get; }

		bool IsWithinIteratorContext { get; }
	}
}
