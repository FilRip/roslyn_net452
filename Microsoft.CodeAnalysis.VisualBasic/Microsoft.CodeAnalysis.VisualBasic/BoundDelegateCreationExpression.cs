using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundDelegateCreationExpression : BoundExpression
	{
		private readonly BoundExpression _ReceiverOpt;

		private readonly MethodSymbol _Method;

		private readonly BoundLambda _RelaxationLambdaOpt;

		private readonly BoundRValuePlaceholder _RelaxationReceiverPlaceholderOpt;

		private readonly BoundMethodGroup _MethodGroupOpt;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)ReceiverOpt);

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public MethodSymbol Method => _Method;

		public BoundLambda RelaxationLambdaOpt => _RelaxationLambdaOpt;

		public BoundRValuePlaceholder RelaxationReceiverPlaceholderOpt => _RelaxationReceiverPlaceholderOpt;

		public BoundMethodGroup MethodGroupOpt => _MethodGroupOpt;

		public BoundDelegateCreationExpression(SyntaxNode syntax, BoundExpression receiverOpt, MethodSymbol method, BoundLambda relaxationLambdaOpt, BoundRValuePlaceholder relaxationReceiverPlaceholderOpt, BoundMethodGroup methodGroupOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.DelegateCreationExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt) || BoundNodeExtensions.NonNullAndHasErrors(relaxationLambdaOpt) || BoundNodeExtensions.NonNullAndHasErrors(relaxationReceiverPlaceholderOpt) || BoundNodeExtensions.NonNullAndHasErrors(methodGroupOpt))
		{
			_ReceiverOpt = receiverOpt;
			_Method = method;
			_RelaxationLambdaOpt = relaxationLambdaOpt;
			_RelaxationReceiverPlaceholderOpt = relaxationReceiverPlaceholderOpt;
			_MethodGroupOpt = methodGroupOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitDelegateCreationExpression(this);
		}

		public BoundDelegateCreationExpression Update(BoundExpression receiverOpt, MethodSymbol method, BoundLambda relaxationLambdaOpt, BoundRValuePlaceholder relaxationReceiverPlaceholderOpt, BoundMethodGroup methodGroupOpt, TypeSymbol type)
		{
			if (receiverOpt != ReceiverOpt || (object)method != Method || relaxationLambdaOpt != RelaxationLambdaOpt || relaxationReceiverPlaceholderOpt != RelaxationReceiverPlaceholderOpt || methodGroupOpt != MethodGroupOpt || (object)type != base.Type)
			{
				BoundDelegateCreationExpression boundDelegateCreationExpression = new BoundDelegateCreationExpression(base.Syntax, receiverOpt, method, relaxationLambdaOpt, relaxationReceiverPlaceholderOpt, methodGroupOpt, type, base.HasErrors);
				boundDelegateCreationExpression.CopyAttributes(this);
				return boundDelegateCreationExpression;
			}
			return this;
		}
	}
}
