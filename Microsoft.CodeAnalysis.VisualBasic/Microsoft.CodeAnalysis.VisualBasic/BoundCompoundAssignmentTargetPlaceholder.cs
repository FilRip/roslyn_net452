using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCompoundAssignmentTargetPlaceholder : BoundValuePlaceholderBase
	{
		public BoundCompoundAssignmentTargetPlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.CompoundAssignmentTargetPlaceholder, syntax, type, hasErrors)
		{
		}

		public BoundCompoundAssignmentTargetPlaceholder(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.CompoundAssignmentTargetPlaceholder, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCompoundAssignmentTargetPlaceholder(this);
		}

		public BoundCompoundAssignmentTargetPlaceholder Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundCompoundAssignmentTargetPlaceholder boundCompoundAssignmentTargetPlaceholder = new BoundCompoundAssignmentTargetPlaceholder(base.Syntax, type, base.HasErrors);
				boundCompoundAssignmentTargetPlaceholder.CopyAttributes(this);
				return boundCompoundAssignmentTargetPlaceholder;
			}
			return this;
		}
	}
}
