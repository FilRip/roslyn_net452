using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLateBoundArgumentSupportingAssignmentWithCapture : BoundExpression
	{
		private readonly BoundExpression _OriginalArgument;

		private readonly SynthesizedLocal _LocalSymbol;

		public BoundExpression OriginalArgument => _OriginalArgument;

		public SynthesizedLocal LocalSymbol => _LocalSymbol;

		public BoundLateBoundArgumentSupportingAssignmentWithCapture(SyntaxNode syntax, BoundExpression originalArgument, SynthesizedLocal localSymbol, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LateBoundArgumentSupportingAssignmentWithCapture, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(originalArgument))
		{
			_OriginalArgument = originalArgument;
			_LocalSymbol = localSymbol;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLateBoundArgumentSupportingAssignmentWithCapture(this);
		}

		public BoundLateBoundArgumentSupportingAssignmentWithCapture Update(BoundExpression originalArgument, SynthesizedLocal localSymbol, TypeSymbol type)
		{
			if (originalArgument != OriginalArgument || (object)localSymbol != LocalSymbol || (object)type != base.Type)
			{
				BoundLateBoundArgumentSupportingAssignmentWithCapture boundLateBoundArgumentSupportingAssignmentWithCapture = new BoundLateBoundArgumentSupportingAssignmentWithCapture(base.Syntax, originalArgument, localSymbol, type, base.HasErrors);
				boundLateBoundArgumentSupportingAssignmentWithCapture.CopyAttributes(this);
				return boundLateBoundArgumentSupportingAssignmentWithCapture;
			}
			return this;
		}
	}
}
