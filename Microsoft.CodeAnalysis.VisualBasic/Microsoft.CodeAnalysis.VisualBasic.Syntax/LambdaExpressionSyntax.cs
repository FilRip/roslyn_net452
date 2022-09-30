using System;
using System.ComponentModel;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class LambdaExpressionSyntax : ExpressionSyntax
	{
		internal LambdaHeaderSyntax _subOrFunctionHeader;

		public LambdaHeaderSyntax SubOrFunctionHeader => GetSubOrFunctionHeaderCore();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use SubOrFunctionHeader instead.", true)]
		public LambdaHeaderSyntax Begin => SubOrFunctionHeader;

		internal LambdaExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual LambdaHeaderSyntax GetSubOrFunctionHeaderCore()
		{
			return GetRedAtZero(ref _subOrFunctionHeader);
		}

		public LambdaExpressionSyntax WithSubOrFunctionHeader(LambdaHeaderSyntax subOrFunctionHeader)
		{
			return WithSubOrFunctionHeaderCore(subOrFunctionHeader);
		}

		internal abstract LambdaExpressionSyntax WithSubOrFunctionHeaderCore(LambdaHeaderSyntax subOrFunctionHeader);
	}
}
