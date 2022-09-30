using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundByRefArgumentWithCopyBack : BoundExpression
	{
		private readonly BoundExpression _OriginalArgument;

		private readonly BoundExpression _InConversion;

		private readonly BoundByRefArgumentPlaceholder _InPlaceholder;

		private readonly BoundExpression _OutConversion;

		private readonly BoundRValuePlaceholder _OutPlaceholder;

		public BoundExpression OriginalArgument => _OriginalArgument;

		public BoundExpression InConversion => _InConversion;

		public BoundByRefArgumentPlaceholder InPlaceholder => _InPlaceholder;

		public BoundExpression OutConversion => _OutConversion;

		public BoundRValuePlaceholder OutPlaceholder => _OutPlaceholder;

		public BoundByRefArgumentWithCopyBack(SyntaxNode syntax, BoundExpression originalArgument, BoundExpression inConversion, BoundByRefArgumentPlaceholder inPlaceholder, BoundExpression outConversion, BoundRValuePlaceholder outPlaceholder, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ByRefArgumentWithCopyBack, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(originalArgument) || BoundNodeExtensions.NonNullAndHasErrors(inConversion) || BoundNodeExtensions.NonNullAndHasErrors(inPlaceholder) || BoundNodeExtensions.NonNullAndHasErrors(outConversion) || BoundNodeExtensions.NonNullAndHasErrors(outPlaceholder))
		{
			_OriginalArgument = originalArgument;
			_InConversion = inConversion;
			_InPlaceholder = inPlaceholder;
			_OutConversion = outConversion;
			_OutPlaceholder = outPlaceholder;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitByRefArgumentWithCopyBack(this);
		}

		public BoundByRefArgumentWithCopyBack Update(BoundExpression originalArgument, BoundExpression inConversion, BoundByRefArgumentPlaceholder inPlaceholder, BoundExpression outConversion, BoundRValuePlaceholder outPlaceholder, TypeSymbol type)
		{
			if (originalArgument != OriginalArgument || inConversion != InConversion || inPlaceholder != InPlaceholder || outConversion != OutConversion || outPlaceholder != OutPlaceholder || (object)type != base.Type)
			{
				BoundByRefArgumentWithCopyBack boundByRefArgumentWithCopyBack = new BoundByRefArgumentWithCopyBack(base.Syntax, originalArgument, inConversion, inPlaceholder, outConversion, outPlaceholder, type, base.HasErrors);
				boundByRefArgumentWithCopyBack.CopyAttributes(this);
				return boundByRefArgumentWithCopyBack;
			}
			return this;
		}
	}
}
