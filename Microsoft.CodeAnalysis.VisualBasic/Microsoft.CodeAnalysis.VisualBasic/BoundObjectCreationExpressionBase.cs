using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundObjectCreationExpressionBase : BoundExpression
	{
		private readonly BoundObjectInitializerExpressionBase _InitializerOpt;

		public BoundObjectInitializerExpressionBase InitializerOpt => _InitializerOpt;

		protected BoundObjectCreationExpressionBase(BoundKind kind, SyntaxNode syntax, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type, bool hasErrors = false)
			: base(kind, syntax, type, hasErrors)
		{
			_InitializerOpt = initializerOpt;
		}
	}
}
