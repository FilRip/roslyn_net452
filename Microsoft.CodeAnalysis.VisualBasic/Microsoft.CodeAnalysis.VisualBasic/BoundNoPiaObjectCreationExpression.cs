using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNoPiaObjectCreationExpression : BoundObjectCreationExpressionBase
	{
		private readonly string _GuidString;

		public string GuidString => _GuidString;

		public BoundNoPiaObjectCreationExpression(SyntaxNode syntax, string guidString, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.NoPiaObjectCreationExpression, syntax, initializerOpt, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(initializerOpt))
		{
			_GuidString = guidString;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNoPiaObjectCreationExpression(this);
		}

		public BoundNoPiaObjectCreationExpression Update(string guidString, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type)
		{
			if ((object)guidString != GuidString || initializerOpt != base.InitializerOpt || (object)type != base.Type)
			{
				BoundNoPiaObjectCreationExpression boundNoPiaObjectCreationExpression = new BoundNoPiaObjectCreationExpression(base.Syntax, guidString, initializerOpt, type, base.HasErrors);
				boundNoPiaObjectCreationExpression.CopyAttributes(this);
				return boundNoPiaObjectCreationExpression;
			}
			return this;
		}
	}
}
