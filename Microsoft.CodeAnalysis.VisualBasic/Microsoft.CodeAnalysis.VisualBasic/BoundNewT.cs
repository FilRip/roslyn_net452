using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNewT : BoundObjectCreationExpressionBase
	{
		public BoundNewT(SyntaxNode syntax, BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.NewT, syntax, initializerOpt, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(initializerOpt))
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNewT(this);
		}

		public BoundNewT Update(BoundObjectInitializerExpressionBase initializerOpt, TypeSymbol type)
		{
			if (initializerOpt != base.InitializerOpt || (object)type != base.Type)
			{
				BoundNewT boundNewT = new BoundNewT(base.Syntax, initializerOpt, type, base.HasErrors);
				boundNewT.CopyAttributes(this);
				return boundNewT;
			}
			return this;
		}
	}
}
