using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUnstructuredExceptionOnErrorSwitch : BoundStatement
	{
		private readonly BoundExpression _Value;

		private readonly ImmutableArray<BoundGotoStatement> _Jumps;

		public BoundExpression Value => _Value;

		public ImmutableArray<BoundGotoStatement> Jumps => _Jumps;

		public BoundUnstructuredExceptionOnErrorSwitch(SyntaxNode syntax, BoundExpression value, ImmutableArray<BoundGotoStatement> jumps, bool hasErrors = false)
			: base(BoundKind.UnstructuredExceptionOnErrorSwitch, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value) || BoundNodeExtensions.NonNullAndHasErrors(jumps))
		{
			_Value = value;
			_Jumps = jumps;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnstructuredExceptionOnErrorSwitch(this);
		}

		public BoundUnstructuredExceptionOnErrorSwitch Update(BoundExpression value, ImmutableArray<BoundGotoStatement> jumps)
		{
			if (value != Value || jumps != Jumps)
			{
				BoundUnstructuredExceptionOnErrorSwitch boundUnstructuredExceptionOnErrorSwitch = new BoundUnstructuredExceptionOnErrorSwitch(base.Syntax, value, jumps, base.HasErrors);
				boundUnstructuredExceptionOnErrorSwitch.CopyAttributes(this);
				return boundUnstructuredExceptionOnErrorSwitch;
			}
			return this;
		}
	}
}
