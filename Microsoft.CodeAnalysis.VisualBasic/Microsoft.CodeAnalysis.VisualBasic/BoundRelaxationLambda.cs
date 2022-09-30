using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRelaxationLambda : BoundExtendedConversionInfo
	{
		private readonly BoundLambda _Lambda;

		private readonly BoundRValuePlaceholder _ReceiverPlaceholderOpt;

		public BoundLambda Lambda => _Lambda;

		public BoundRValuePlaceholder ReceiverPlaceholderOpt => _ReceiverPlaceholderOpt;

		public BoundRelaxationLambda(SyntaxNode syntax, BoundLambda lambda, BoundRValuePlaceholder receiverPlaceholderOpt, bool hasErrors = false)
			: base(BoundKind.RelaxationLambda, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(lambda) || BoundNodeExtensions.NonNullAndHasErrors(receiverPlaceholderOpt))
		{
			_Lambda = lambda;
			_ReceiverPlaceholderOpt = receiverPlaceholderOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRelaxationLambda(this);
		}

		public BoundRelaxationLambda Update(BoundLambda lambda, BoundRValuePlaceholder receiverPlaceholderOpt)
		{
			if (lambda != Lambda || receiverPlaceholderOpt != ReceiverPlaceholderOpt)
			{
				BoundRelaxationLambda boundRelaxationLambda = new BoundRelaxationLambda(base.Syntax, lambda, receiverPlaceholderOpt, base.HasErrors);
				boundRelaxationLambda.CopyAttributes(this);
				return boundRelaxationLambda;
			}
			return this;
		}
	}
}
