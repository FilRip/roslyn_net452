using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUnstructuredExceptionResumeSwitch : BoundStatement
	{
		private readonly BoundLocal _ResumeTargetTemporary;

		private readonly BoundLabelStatement _ResumeLabel;

		private readonly BoundLabelStatement _ResumeNextLabel;

		private readonly ImmutableArray<BoundGotoStatement> _Jumps;

		public BoundLocal ResumeTargetTemporary => _ResumeTargetTemporary;

		public BoundLabelStatement ResumeLabel => _ResumeLabel;

		public BoundLabelStatement ResumeNextLabel => _ResumeNextLabel;

		public ImmutableArray<BoundGotoStatement> Jumps => _Jumps;

		public BoundUnstructuredExceptionResumeSwitch(SyntaxNode syntax, BoundLocal resumeTargetTemporary, BoundLabelStatement resumeLabel, BoundLabelStatement resumeNextLabel, ImmutableArray<BoundGotoStatement> jumps, bool hasErrors = false)
			: base(BoundKind.UnstructuredExceptionResumeSwitch, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(resumeTargetTemporary) || BoundNodeExtensions.NonNullAndHasErrors(resumeLabel) || BoundNodeExtensions.NonNullAndHasErrors(resumeNextLabel) || BoundNodeExtensions.NonNullAndHasErrors(jumps))
		{
			_ResumeTargetTemporary = resumeTargetTemporary;
			_ResumeLabel = resumeLabel;
			_ResumeNextLabel = resumeNextLabel;
			_Jumps = jumps;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnstructuredExceptionResumeSwitch(this);
		}

		public BoundUnstructuredExceptionResumeSwitch Update(BoundLocal resumeTargetTemporary, BoundLabelStatement resumeLabel, BoundLabelStatement resumeNextLabel, ImmutableArray<BoundGotoStatement> jumps)
		{
			if (resumeTargetTemporary != ResumeTargetTemporary || resumeLabel != ResumeLabel || resumeNextLabel != ResumeNextLabel || jumps != Jumps)
			{
				BoundUnstructuredExceptionResumeSwitch boundUnstructuredExceptionResumeSwitch = new BoundUnstructuredExceptionResumeSwitch(base.Syntax, resumeTargetTemporary, resumeLabel, resumeNextLabel, jumps, base.HasErrors);
				boundUnstructuredExceptionResumeSwitch.CopyAttributes(this);
				return boundUnstructuredExceptionResumeSwitch;
			}
			return this;
		}
	}
}
