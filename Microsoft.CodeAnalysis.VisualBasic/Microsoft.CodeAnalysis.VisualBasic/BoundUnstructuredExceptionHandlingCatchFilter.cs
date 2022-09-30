using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUnstructuredExceptionHandlingCatchFilter : BoundExpression
	{
		private readonly BoundLocal _ActiveHandlerLocal;

		private readonly BoundLocal _ResumeTargetLocal;

		public BoundLocal ActiveHandlerLocal => _ActiveHandlerLocal;

		public BoundLocal ResumeTargetLocal => _ResumeTargetLocal;

		public BoundUnstructuredExceptionHandlingCatchFilter(SyntaxNode syntax, BoundLocal activeHandlerLocal, BoundLocal resumeTargetLocal, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UnstructuredExceptionHandlingCatchFilter, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(activeHandlerLocal) || BoundNodeExtensions.NonNullAndHasErrors(resumeTargetLocal))
		{
			_ActiveHandlerLocal = activeHandlerLocal;
			_ResumeTargetLocal = resumeTargetLocal;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnstructuredExceptionHandlingCatchFilter(this);
		}

		public BoundUnstructuredExceptionHandlingCatchFilter Update(BoundLocal activeHandlerLocal, BoundLocal resumeTargetLocal, TypeSymbol type)
		{
			if (activeHandlerLocal != ActiveHandlerLocal || resumeTargetLocal != ResumeTargetLocal || (object)type != base.Type)
			{
				BoundUnstructuredExceptionHandlingCatchFilter boundUnstructuredExceptionHandlingCatchFilter = new BoundUnstructuredExceptionHandlingCatchFilter(base.Syntax, activeHandlerLocal, resumeTargetLocal, type, base.HasErrors);
				boundUnstructuredExceptionHandlingCatchFilter.CopyAttributes(this);
				return boundUnstructuredExceptionHandlingCatchFilter;
			}
			return this;
		}
	}
}
