using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundHostObjectMemberReference : BoundExpression
	{
		public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
			: base(BoundKind.HostObjectMemberReference, syntax, type, hasErrors)
		{
		}

		public BoundHostObjectMemberReference(SyntaxNode syntax, TypeSymbol type)
			: base(BoundKind.HostObjectMemberReference, syntax, type)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitHostObjectMemberReference(this);
		}

		public BoundHostObjectMemberReference Update(TypeSymbol type)
		{
			if ((object)type != base.Type)
			{
				BoundHostObjectMemberReference boundHostObjectMemberReference = new BoundHostObjectMemberReference(base.Syntax, type, base.HasErrors);
				boundHostObjectMemberReference.CopyAttributes(this);
				return boundHostObjectMemberReference;
			}
			return this;
		}
	}
}
